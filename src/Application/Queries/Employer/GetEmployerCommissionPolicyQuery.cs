namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Returns commission policy details for an employer.
    /// </summary>
    public class GetEmployerCommissionPolicyQuery :
        QueryBase<EmployerCommissionPolicyModel>
    {
        #region Properties

        /// <summary>
        /// Employer identifier.
        /// </summary>
        public int EmployerId { get; set; }

        #endregion Properties
    }

    internal class GetEmployerCommissionPolicyQueryValidator : IRequestValidator<GetEmployerCommissionPolicyQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetEmployerCommissionPolicyQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GetEmployerCommissionPolicyEmployerId.ForField(nameof(GetEmployerCommissionPolicyQuery.EmployerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class GetEmployerCommissionPolicyQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetEmployerCommissionPolicyQuery, EmployerCommissionPolicyModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<EmployerCommissionPolicyModel> HandleAsync(GetEmployerCommissionPolicyQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.EmployerCommissionPolicyKey(query.EmployerId);
            EmployerCommissionPolicyModel? cached = await CacheService.GetAsync<EmployerCommissionPolicyModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            Employer? employer = await UnitOfWork
                .GetRepository<Employer>()
                .Filter(x => x.Id == query.EmployerId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            EmployerCommissionPolicyModel model = new(
                employer.CommissionRate,
                employer.Id);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.EmployerDependency(employer.Id),
                    AdaIsCacheKeys.EmployerCommissionPolicyDependency(employer.Id)),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
