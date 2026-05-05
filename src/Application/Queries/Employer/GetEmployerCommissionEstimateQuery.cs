namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Returns commission estimate metrics for a single employer.
    /// </summary>
    public class GetEmployerCommissionEstimateQuery :
        QueryBase<EmployerCommissionEstimateModel>
    {
        #region Properties

        /// <summary>
        /// Employer identifier.
        /// </summary>
        public int EmployerId { get; set; }

        #endregion Properties
    }

    internal class GetEmployerCommissionEstimateQueryValidator : IRequestValidator<GetEmployerCommissionEstimateQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetEmployerCommissionEstimateQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.GetEmployerCommissionEstimateEmployerId.ForField(nameof(GetEmployerCommissionEstimateQuery.EmployerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class GetEmployerCommissionEstimateQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetEmployerCommissionEstimateQuery, EmployerCommissionEstimateModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<EmployerCommissionEstimateModel> HandleAsync(
            GetEmployerCommissionEstimateQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.EmployerCommissionEstimateKey(query.EmployerId);
            EmployerCommissionEstimateModel? cached = await CacheService.GetAsync<EmployerCommissionEstimateModel>(cacheKey, cancellationToken);
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

            decimal estimatedGrossTransactionVolume = (await UnitOfWork
                    .GetRepository<JobApplication>()
                    .Filter(x => x.JobPosting.EmployerId == query.EmployerId && x.Status == JobApplicationStatus.Accepted)
                    .ToListAsync(x => x.JobPosting.Wage.Amount, cancellationToken))
                .Sum();

            int acceptedApplicationCount = await UnitOfWork
                .GetRepository<JobApplication>()
                .CountAsync(x => x.JobPosting.EmployerId == query.EmployerId && x.Status == JobApplicationStatus.Accepted, cancellationToken);

            decimal estimatedCommissionAmount = decimal.Round(
                estimatedGrossTransactionVolume * employer.CommissionRate,
                2,
                MidpointRounding.AwayFromZero);

            EmployerCommissionEstimateModel model = new(
                acceptedApplicationCount,
                employer.CommissionRate,
                employer.Id,
                estimatedCommissionAmount,
                estimatedGrossTransactionVolume);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.EmployerDependency(query.EmployerId),
                    AdaIsCacheKeys.EmployerCommissionPolicyDependency(query.EmployerId),
                    AdaIsCacheKeys.JobApplicationAllDependency(),
                    AdaIsCacheKeys.JobPostingAllDependency()),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
