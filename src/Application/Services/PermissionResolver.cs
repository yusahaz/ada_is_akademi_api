namespace Azoxia.AdaIsAkademi.Application.Services
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Persistence;
    using Azoxia.Core.Exceptions;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Permission resolver backed by SystemUserGroupMembership + SystemUserGroupPermission rules.
    /// </summary>
    internal sealed class PermissionResolver :
        IPermissionResolver
    {
        #region Ctors

        public PermissionResolver(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<PermissionResolver> logger)
        {
            UnitOfWork = unitOfWork;
            CacheService = cacheService;
            Logger = logger;
        }

        #endregion Ctors

        #region Properties

        private IUnitOfWork UnitOfWork { get; }
        private ICacheService CacheService { get; }
        private ILogger<PermissionResolver> Logger { get; }

        #endregion Properties

        #region Utils

        private static CacheEntryOptions EntryOptions() =>
            new()
            {
                DistributedTtl = TimeSpan.FromMinutes(5),
                MemoryTtl = TimeSpan.FromMinutes(1),
            };

        private record struct PermissionNode(int Id, string Name, int? ParentId);

        private record struct UserPermissionRule(int PermissionId, PermissionEffect Effect);

        private sealed class PermissionResolverCacheModel
        {
            public IReadOnlyDictionary<int, PermissionEffect> EffectByPermissionId { get; set; } =
                new Dictionary<int, PermissionEffect>();

            public IReadOnlyDictionary<string, int> PermissionIdByName { get; set; } =
                new Dictionary<string, int>(StringComparer.Ordinal);

            public IReadOnlyDictionary<int, int?> ParentIdByPermissionId { get; set; } =
                new Dictionary<int, int?>();
        }

        #endregion Utils

        #region Methods

        public async Task<bool> HasPermissionAsync(
            int systemUserId,
            int? employerId,
            string permission,
            CancellationToken cancellationToken = default)
        {
            systemUserId.ThrowIfOutOfRange(1, int.MaxValue);
            permission.ThrowIfNullOrWhiteSpace(AzoxiaErrorCodes.StringNullOrWhiteSpace);

            CacheKey cacheKey = AdaIsCacheKeys.PermissionResolverCacheKey(systemUserId, employerId);
            PermissionResolverCacheModel? cached =
                await CacheService.GetAsync<PermissionResolverCacheModel>(cacheKey, cancellationToken).ConfigureAwait(false);

            PermissionResolverCacheModel model = cached ?? await BuildModelAsync(
                    systemUserId,
                    employerId,
                    cancellationToken)
                .ConfigureAwait(false);

            if (cached is null)
            {
                CacheEntryOptions options = EntryOptions();
                options.Dependencies = new[]
                {
                    AdaIsCacheKeys.PermissionResolverMembershipAllDependency(),
                    AdaIsCacheKeys.PermissionResolverGroupPermissionAllDependency(),
                    AdaIsCacheKeys.PermissionResolverGroupAllDependency()
                };

                await CacheService.SetAsync(cacheKey, model, options, cancellationToken).ConfigureAwait(false);
            }

            if (!model.PermissionIdByName.TryGetValue(permission, out int permissionId))
            {
                // Backward compatibility: when permission definitions are absent/misconfigured,
                // do not block existing endpoints.
                return true;
            }

            bool hasAllow = false;
            int? currentId = permissionId;
            while (currentId is int nodeId)
            {
                if (model.EffectByPermissionId.TryGetValue(nodeId, out PermissionEffect effect))
                {
                    if (effect == PermissionEffect.Deny)
                    {
                        return false;
                    }

                    if (effect == PermissionEffect.Allow)
                    {
                        hasAllow = true;
                    }
                }

                currentId = model.ParentIdByPermissionId.TryGetValue(nodeId, out int? parentId)
                    ? parentId
                    : null;
            }

            return hasAllow;
        }

        private async Task<PermissionResolverCacheModel> BuildModelAsync(
            int systemUserId,
            int? employerId,
            CancellationToken cancellationToken)
        {
            Logger.LogDebug(
                "Building permission resolver model for systemUserId={SystemUserId}, employerId={EmployerId}.",
                systemUserId,
                employerId);

            int employerIdValue = employerId ?? 0;
            bool hasEmployerScope = employerIdValue > 0;

            // Build permission hierarchy once per cache miss.
            IReadOnlyDictionary<int, int?> parentById;
            IReadOnlyDictionary<string, int> idByName;

            List<PermissionNode> permissionNodes = (await UnitOfWork
                .GetRepository<Permission>()
                .Filter()
                .AsNoTracking()
                .ToListAsync(
                    selector: p => new PermissionNode(p.Id, p.Name, p.ParentId),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)).ToList();

            Dictionary<int, int?> parentTmp = new(permissionNodes.Count);
            Dictionary<string, int> idByNameTmp = new(StringComparer.Ordinal);
            foreach (PermissionNode node in permissionNodes)
            {
                parentTmp[node.Id] = node.ParentId;
                idByNameTmp[node.Name] = node.Id;
            }

            parentById = parentTmp;
            idByName = idByNameTmp;

            // Collect active group ids visible within the given scopes.
            List<int> groupIds = (await UnitOfWork
                .GetRepository<SystemUserGroupMembership>()
                .Filter(m =>
                    m.SystemUserId == systemUserId &&
                    m.IsActive &&
                    (m.ScopeType == MembershipScopeType.Global ||
                        (hasEmployerScope &&
                            m.ScopeType == MembershipScopeType.EmployerScoped &&
                            m.ScopeId == employerIdValue)))
                .AsNoTracking()
                .ToListAsync(
                    selector: m => m.SystemUserGroupId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)).ToList();

            Dictionary<int, PermissionEffect> effectById = new();
            if (groupIds.Count > 0)
            {
                List<UserPermissionRule> rules = (await UnitOfWork
                    .GetRepository<SystemUserGroupPermission>()
                    .Filter(p => groupIds.Contains(p.SystemUserGroupId))
                    .AsNoTracking()
                    .ToListAsync(
                        selector: p => new UserPermissionRule(p.PermissionId, p.Effect),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false)).ToList();

                foreach (UserPermissionRule rule in rules)
                {
                    if (!effectById.TryGetValue(rule.PermissionId, out PermissionEffect existing))
                    {
                        effectById[rule.PermissionId] = rule.Effect;
                        continue;
                    }

                    if (existing == PermissionEffect.Allow &&
                        rule.Effect == PermissionEffect.Deny)
                    {
                        effectById[rule.PermissionId] = PermissionEffect.Deny;
                    }
                    else if (existing == PermissionEffect.Deny)
                    {
                        // Deny always wins; keep existing.
                        continue;
                    }
                }
            }

            return new PermissionResolverCacheModel
            {
                EffectByPermissionId = effectById,
                PermissionIdByName = idByName,
                ParentIdByPermissionId = parentById
            };
        }

        #endregion Methods
    }
}

