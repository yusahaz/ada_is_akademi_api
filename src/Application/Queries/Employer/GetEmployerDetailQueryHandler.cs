namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;
    using System;

    internal class GetEmployerDetailQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetEmployerDetailQuery, EmployerFullDetailModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<EmployerFullDetailModel> HandleAsync(GetEmployerDetailQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.EmployerFullDetailKey(query.EmployerId);
            EmployerFullDetailModel? cached = await CacheService.GetAsync<EmployerFullDetailModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            Employer? entity = await UnitOfWork
                .GetRepository<Employer>()
                .Filter(x => x.Id == query.EmployerId)
                .AsNoTracking()
                .Include(x => x.Locations)
                .Include(x => x.Supervisors)
                .FirstOrDefaultAsync(cancellationToken);

            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            IReadOnlyDictionary<int, SystemUser> systemUsersById = await LoadSupervisorUsersById(entity, cancellationToken);

            EmployerFullDetailModel model = new(
                entity.Id,
                entity.Name,
                entity.Description,
                entity.Status,
                entity.TaxNumber.Value,
                entity.CommissionRate,
                entity.LogoObjectKey,
                MapContact(entity),
                entity.Locations
                    .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(x => new EmployerLocationDetailModel(
                        x.Id,
                        x.Name,
                        x.Description,
                        x.GeofenceRadiusMetres))
                    .ToList(),
                entity.Supervisors
                    .OrderByDescending(x => x.IsActive)
                    .ThenBy(x => GetSupervisorEmail(systemUsersById, x.SystemUserId), StringComparer.OrdinalIgnoreCase)
                    .Select(x => new EmployerSupervisorDetailModel(
                        x.Id,
                        x.SystemUserId,
                        x.LocationId,
                        x.IsActive,
                        MapSupervisorUser(systemUsersById, x.SystemUserId)))
                    .ToList());

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.EmployerDependency(entity.Id)),
                cancellationToken);

            return model;
        }

        private static EmployerContactModel? MapContact(Employer entity)
        {
            Contact c = entity.Contact;
            if (string.IsNullOrWhiteSpace(c.Email) && string.IsNullOrWhiteSpace(c.Phone))
            {
                return null;
            }

            return new EmployerContactModel(
                c.FirstName,
                c.LastName,
                c.Email,
                c.Phone);
        }

        private async Task<IReadOnlyDictionary<int, SystemUser>> LoadSupervisorUsersById(Employer employer, CancellationToken cancellationToken)
        {
            int[] systemUserIds = employer.Supervisors
                .Select(x => x.SystemUserId)
                .Distinct()
                .ToArray();

            if (systemUserIds.Length == 0)
            {
                return new Dictionary<int, SystemUser>();
            }

            IEnumerable<SystemUser> users = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x => systemUserIds.Contains(x.Id))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return users.ToDictionary(x => x.Id);
        }

        private static string GetSupervisorEmail(IReadOnlyDictionary<int, SystemUser> usersById, int systemUserId) =>
            usersById.TryGetValue(systemUserId, out SystemUser? user)
                ? user.Email
                : string.Empty;

        private static EmployerSupervisorUserSummaryModel MapSupervisorUser(IReadOnlyDictionary<int, SystemUser> usersById, int systemUserId)
        {
            usersById.TryGetValue(systemUserId, out SystemUser? user);
            user = user.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            return new EmployerSupervisorUserSummaryModel(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Phone,
                user.AccountStatus);
        }

        #endregion Utils
    }
}
