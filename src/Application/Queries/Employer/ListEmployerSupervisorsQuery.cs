namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lists supervisors for authenticated employer.
    /// </summary>
    public class ListEmployerSupervisorsQuery :
        QueryBase<IReadOnlyList<EmployerSupervisorListItemModel>>;

    internal class ListEmployerSupervisorsQueryValidator : IRequestValidator<ListEmployerSupervisorsQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(ListEmployerSupervisorsQuery request)
            => new([]);
    }

    internal class ListEmployerSupervisorsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListEmployerSupervisorsQuery, IReadOnlyList<EmployerSupervisorListItemModel>>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<IReadOnlyList<EmployerSupervisorListItemModel>> HandleAsync(
            ListEmployerSupervisorsQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            CacheKey cacheKey = new("query", "EmployerSupervisors", employerId.ToString());
            IReadOnlyList<EmployerSupervisorListItemModel>? cached =
                await CacheService.GetAsync<IReadOnlyList<EmployerSupervisorListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            List<ShiftSupervisor> supervisors = (await UnitOfWork
                .GetRepository<ShiftSupervisor>()
                .Filter(x => x.EmployerId == employerId && x.IsActive)
                .AsNoTracking()
                .Include(x => x.SystemUser)
                .ToListAsync(cancellationToken))
                .ToList();

            int[] supervisorUserIds = supervisors.Select(x => x.SystemUserId).Distinct().ToArray();
            List<SystemUserGroupMembership> memberships = supervisorUserIds.Length == 0
                ? []
                : (await UnitOfWork
                    .GetRepository<SystemUserGroupMembership>()
                    .Filter(x => supervisorUserIds.Contains(x.SystemUserId) && x.IsActive)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken))
                .ToList();

            IReadOnlyList<EmployerSupervisorListItemModel> rows = supervisors
                .GroupBy(x => x.SystemUserId)
                .Select(group =>
                {
                    ShiftSupervisor sample = group.First();
                    List<int> assignedLocationIds = group.Where(x => x.LocationId.HasValue).Select(x => x.LocationId!.Value).Distinct().ToList();
                    List<int> groupIds = memberships
                        .Where(x => x.SystemUserId == group.Key)
                        .Select(x => x.SystemUserGroupId)
                        .Distinct()
                        .ToList();
                    MembershipScopeType scopeType = assignedLocationIds.Count > 0
                        ? MembershipScopeType.LocationScoped
                        : MembershipScopeType.EmployerScoped;
                    return new EmployerSupervisorListItemModel(
                        group.Key,
                        $"{sample.SystemUser.FirstName ?? string.Empty} {sample.SystemUser.LastName ?? string.Empty}".Trim(),
                        sample.SystemUser.Email,
                        assignedLocationIds,
                        groupIds,
                        scopeType);
                })
                .OrderBy(x => x.FullName)
                .ToList();

            await CacheService.SetAsync(
                cacheKey,
                rows,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.EmployerDependency(employerId),
                    AdaIsCacheKeys.SystemUserGroupAllDependency()),
                cancellationToken);

            return rows;
        }
    }
}
