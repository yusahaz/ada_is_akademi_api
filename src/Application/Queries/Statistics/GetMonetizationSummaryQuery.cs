namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Persistence;

    /// <summary>
    /// Returns monetization baseline statistics for management reporting.
    /// </summary>
    public class GetMonetizationSummaryQuery :
        QueryBase<MonetizationSummaryModel>;

    internal class GetMonetizationSummaryQueryValidator : IRequestValidator<GetMonetizationSummaryQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetMonetizationSummaryQuery request)
            => new();

        #endregion Methods
    }

    internal class GetMonetizationSummaryQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetMonetizationSummaryQuery, MonetizationSummaryModel>(serviceProvider)
    {
        #region Fields

        private const decimal EstimatedCommissionRate = 0.10m;

        #endregion Fields

        #region Utils

        /// <inheritdoc />
        protected override async Task<MonetizationSummaryModel> HandleAsync(
            GetMonetizationSummaryQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.DashboardMonetizationSummaryKey();
            MonetizationSummaryModel? cached = await CacheService.GetAsync<MonetizationSummaryModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            IRepository<JobApplication> jobApplicationRepository = UnitOfWork.GetRepository<JobApplication>();
            IRepository<JobPosting> jobPostingRepository = UnitOfWork.GetRepository<JobPosting>();
            IRepository<Employer> employerRepository = UnitOfWork.GetRepository<Employer>();

            int acceptedJobApplicationCount = await jobApplicationRepository.CountAsync(
                x => x.Status == JobApplicationStatus.Accepted,
                cancellationToken);

            int filledOrCompletedJobPostingCount = await jobPostingRepository.CountAsync(
                x => x.Status == JobPostingStatus.Filled || x.Status == JobPostingStatus.Completed,
                cancellationToken);

            int activeEmployerCount = await employerRepository.CountAsync(
                x => x.Status == EmployerStatus.Active,
                cancellationToken);

            decimal estimatedGrossTransactionVolume = acceptedJobApplicationCount * 100m;
            decimal estimatedCommissionAmount = decimal.Round(
                estimatedGrossTransactionVolume * EstimatedCommissionRate,
                2,
                MidpointRounding.AwayFromZero);

            MonetizationSummaryModel model = new(
                acceptedJobApplicationCount,
                activeEmployerCount,
                estimatedCommissionAmount,
                estimatedGrossTransactionVolume,
                filledOrCompletedJobPostingCount);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.JobApplicationAllDependency(),
                    AdaIsCacheKeys.JobPostingAllDependency(),
                    AdaIsCacheKeys.EmployerAllDependency()),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
