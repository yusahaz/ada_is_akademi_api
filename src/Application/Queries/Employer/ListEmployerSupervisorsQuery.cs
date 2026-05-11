namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Identity;
    using Azoxia.Core.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using SystemUserType = Azoxia.AdaIsAkademi.Domain.SystemUserType;

    /// <summary>
    /// Lists supervisors for employer.
    /// </summary>
    public class ListEmployerSupervisorsQuery :
        QueryBase<IReadOnlyList<EmployerSupervisorListItemModel>>
    {
        /// <summary>
        /// Optional employer id for admin queries. When supplied, caller must be an admin.
        /// Otherwise the authenticated employer actor id claim is used.
        /// </summary>
        public int? EmployerId { get; set; }
    }

    internal class ListEmployerSupervisorsQueryValidator : IRequestValidator<ListEmployerSupervisorsQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(ListEmployerSupervisorsQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.EmployerId.HasValue && request.EmployerId.Value <= 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(ListEmployerSupervisorsQuery.EmployerId)));
            }

            return new ValidationResult(failures);
        }
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
            int employerId;
            if (query.EmployerId.HasValue)
            {
                bool isAdmin = executionContext.GetClaim("system_user_type") == ((int)SystemUserType.Admin).ToString();
                isAdmin.ThrowIfFalse(AzoxiaErrorCodes.RequestValidationFailed);
                employerId = query.EmployerId.Value;
            }
            else
            {
                employerId = executionContext.RequireAdaIsEmployerActorId();
            }

            CacheKey cacheKey = new("query", "EmployerSupervisors", employerId.ToString());
            IReadOnlyList<EmployerSupervisorListItemModel>? cached =
                await CacheService.GetAsync<IReadOnlyList<EmployerSupervisorListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            List<Supervisor> supervisors = (await UnitOfWork
                .GetRepository<Supervisor>()
                .Filter(x => x.EmployerId == employerId && x.IsActive)
                .AsNoTracking()
                .Include(x => x.SystemUser)
                .ToListAsync(cancellationToken))
                .ToList();

            IReadOnlyList<EmployerSupervisorListItemModel> rows = supervisors
                .GroupBy(x => x.SystemUserId)
                .Select(group =>
                {
                    Supervisor sample = group.First();
                    List<int> assignedLocationIds = group.Where(x => x.LocationId.HasValue).Select(x => x.LocationId!.Value).Distinct().ToList();
                    return new EmployerSupervisorListItemModel(
                        group.Key,
                        $"{sample.SystemUser.FirstName ?? string.Empty} {sample.SystemUser.LastName ?? string.Empty}".Trim(),
                        sample.SystemUser.Email,
                        assignedLocationIds);
                })
                .OrderBy(x => x.FullName)
                .ToList();

            await CacheService.SetAsync(
                cacheKey,
                rows,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.EmployerDependency(employerId)),
                cancellationToken);

            return rows;
        }
    }
}
