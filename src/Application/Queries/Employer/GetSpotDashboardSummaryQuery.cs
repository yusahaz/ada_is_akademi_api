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
    /// Returns employer spot dashboard summary.
    /// </summary>
    public class GetSpotDashboardSummaryQuery :
        QueryBase<SpotDashboardSummaryModel>;

    internal class GetSpotDashboardSummaryQueryValidator : IRequestValidator<GetSpotDashboardSummaryQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(GetSpotDashboardSummaryQuery request)
            => new([]);
    }

    internal class GetSpotDashboardSummaryQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetSpotDashboardSummaryQuery, SpotDashboardSummaryModel>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<SpotDashboardSummaryModel> HandleAsync(
            GetSpotDashboardSummaryQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            CacheKey cacheKey = new("query", "SpotDashboardSummary", employerId.ToString());
            SpotDashboardSummaryModel? cached = await CacheService.GetAsync<SpotDashboardSummaryModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            int openPostingCount = checked((int)await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.EmployerId == employerId && x.Status == JobPostingStatus.Open && !x.IsDeleted)
                .CountAsync(cancellationToken));

            int pendingApplicationCount = checked((int)await UnitOfWork
                .GetRepository<JobApplication>()
                .Filter(x => x.JobPosting.EmployerId == employerId && x.Status == JobApplicationStatus.Pending)
                .CountAsync(cancellationToken));

            int acceptedApplicationCount = checked((int)await UnitOfWork
                .GetRepository<JobApplication>()
                .Filter(x => x.JobPosting.EmployerId == employerId && x.Status == JobApplicationStatus.Accepted)
                .CountAsync(cancellationToken));

            List<int> employerWorkerIds = (await UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x =>
                    x.JobPosting.EmployerId == employerId
                    && !x.Worker.IsDeleted
                    && x.Worker.SystemUser.AccountStatus == AccountStatus.Active)
                .AsNoTracking()
                .ToListAsync(x => x.WorkerId, cancellationToken))
                .ToList();
            int activeWorkerCount = employerWorkerIds
                .Distinct()
                .Count();

            int activeAnomalyCount = checked((int)await UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x => x.JobPosting.EmployerId == employerId && x.IsAnomalyFlagged)
                .CountAsync(cancellationToken));

            int pendingPayoutCount = checked((int)await UnitOfWork
                .GetRepository<WorkerPayout>()
                .Filter(x => x.EmployerId == employerId && x.Status == WorkerPayoutStatus.Pending)
                .CountAsync(cancellationToken));

            decimal fillRateBase = openPostingCount + pendingApplicationCount;
            decimal dailyFillRatePercent = fillRateBase <= 0
                ? 0m
                : decimal.Round((acceptedApplicationCount / fillRateBase) * 100m, 2);

            SpotDashboardSummaryModel model = new(
                dailyFillRatePercent,
                activeWorkerCount,
                openPostingCount,
                pendingApplicationCount,
                activeAnomalyCount,
                pendingPayoutCount);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.EmployerDependency(employerId),
                    AdaIsCacheKeys.JobPostingAllDependency(),
                    AdaIsCacheKeys.JobApplicationAllDependency(),
                    AdaIsCacheKeys.WorkerPayoutAllDependency(),
                    AdaIsCacheKeys.ShiftAssignmentAllDependency()),
                cancellationToken);

            return model;
        }
    }
}
