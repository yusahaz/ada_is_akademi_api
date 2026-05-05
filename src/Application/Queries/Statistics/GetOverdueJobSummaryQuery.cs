namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Persistence;
    using System;

    /// <summary>
    /// Returns overdue posting and pending-application counters for scheduler/reporting views.
    /// </summary>
    public class GetOverdueJobSummaryQuery :
        QueryBase<OverdueJobSummaryModel>;

    internal class GetOverdueJobSummaryQueryValidator : IRequestValidator<GetOverdueJobSummaryQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetOverdueJobSummaryQuery request)
            => new();

        #endregion Methods
    }

    internal class GetOverdueJobSummaryQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetOverdueJobSummaryQuery, OverdueJobSummaryModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<OverdueJobSummaryModel> HandleAsync(
            GetOverdueJobSummaryQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.DashboardOverdueJobSummaryKey();
            OverdueJobSummaryModel? cached = await CacheService.GetAsync<OverdueJobSummaryModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            IRepository<JobPosting> jobPostingRepository = UnitOfWork.GetRepository<JobPosting>();
            IRepository<JobApplication> jobApplicationRepository = UnitOfWork.GetRepository<JobApplication>();

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            int overduePostingCount = await jobPostingRepository.CountAsync(
                x => (x.Status == JobPostingStatus.Open || x.Status == JobPostingStatus.Filled)
                     && x.ShiftDate < today,
                cancellationToken);

            int overduePendingApplicationCount = await jobApplicationRepository.CountAsync(
                x => x.Status == JobApplicationStatus.Pending
                     && (x.JobPosting.Status == JobPostingStatus.Open || x.JobPosting.Status == JobPostingStatus.Filled)
                     && x.JobPosting.ShiftDate < today,
                cancellationToken);

            var model = new OverdueJobSummaryModel(
                overduePendingApplicationCount,
                overduePostingCount,
                today);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.JobPostingAllDependency(),
                    AdaIsCacheKeys.JobApplicationAllDependency()),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
