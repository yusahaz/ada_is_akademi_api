namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Persistence;

    /// <summary>
    /// Returns aggregate dashboard counters used by web statistic cards.
    /// </summary>
    public class GetDashboardStatisticsQuery :
        QueryBase<DashboardStatisticsModel>;

    internal class GetDashboardStatisticsQueryValidator : IRequestValidator<GetDashboardStatisticsQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetDashboardStatisticsQuery request)
            => new();

        #endregion Methods
    }

    internal class GetDashboardStatisticsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetDashboardStatisticsQuery, DashboardStatisticsModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<DashboardStatisticsModel> HandleAsync(
            GetDashboardStatisticsQuery query,
            CancellationToken cancellationToken)
        {
            IRepository<SystemUser> systemUserRepository = UnitOfWork.GetRepository<SystemUser>();
            IRepository<Worker> workerRepository = UnitOfWork.GetRepository<Worker>();
            IRepository<Employer> employerRepository = UnitOfWork.GetRepository<Employer>();
            IRepository<JobPosting> jobPostingRepository = UnitOfWork.GetRepository<JobPosting>();
            IRepository<JobApplication> jobApplicationRepository = UnitOfWork.GetRepository<JobApplication>();

            DashboardSystemUserStatisticsModel systemUserStats = await GetSystemUserStatisticsAsync(
                systemUserRepository,
                cancellationToken);
            DashboardWorkerStatisticsModel workerStats = await GetWorkerStatisticsAsync(
                workerRepository,
                systemUserRepository,
                cancellationToken);
            DashboardEmployerStatisticsModel employerStats = await GetEmployerStatisticsAsync(
                employerRepository,
                cancellationToken);
            DashboardJobPostingStatisticsModel jobPostingStats = await GetJobPostingStatisticsAsync(
                jobPostingRepository,
                cancellationToken);
            DashboardJobApplicationStatisticsModel jobApplicationStats = await GetJobApplicationStatisticsAsync(
                jobApplicationRepository,
                cancellationToken);

            return new DashboardStatisticsModel(
                systemUserStats.TotalSystemUsers,
                systemUserStats.PendingSystemUserCount,
                systemUserStats.ActiveSystemUserCount,
                systemUserStats.SuspendedSystemUserCount,
                systemUserStats.BannedSystemUserCount,
                systemUserStats.ActivatedTodayCount,
                workerStats.TotalWorkerCount,
                workerStats.PendingWorkerCount,
                workerStats.ActiveWorkerCount,
                workerStats.SuspendedWorkerCount,
                workerStats.BannedWorkerCount,
                employerStats.TotalEmployerCount,
                employerStats.PendingEmployerCount,
                employerStats.ActiveEmployerCount,
                employerStats.SuspendedEmployerCount,
                employerStats.BannedEmployerCount,
                jobPostingStats.TotalJobPostingCount,
                jobPostingStats.DraftJobPostingCount,
                jobPostingStats.OpenJobPostingCount,
                jobPostingStats.FilledJobPostingCount,
                jobPostingStats.CompletedJobPostingCount,
                jobPostingStats.CancelledJobPostingCount,
                jobApplicationStats.TotalJobApplicationCount,
                jobApplicationStats.PendingJobApplicationCount,
                jobApplicationStats.AcceptedJobApplicationCount,
                jobApplicationStats.RejectedJobApplicationCount,
                jobApplicationStats.WithdrawnJobApplicationCount);
        }

        private async Task<DashboardEmployerStatisticsModel> GetEmployerStatisticsAsync(
            IRepository<Employer> employerRepository,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.DashboardEmployerStatisticsKey();
            DashboardEmployerStatisticsModel? cached = await CacheService.GetAsync<DashboardEmployerStatisticsModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            DashboardEmployerStatisticsModel model = new(
                await employerRepository.CountAsync(cancellationToken),
                await employerRepository.CountAsync(x => x.Status == EmployerStatus.Pending, cancellationToken),
                await employerRepository.CountAsync(x => x.Status == EmployerStatus.Active, cancellationToken),
                await employerRepository.CountAsync(x => x.Status == EmployerStatus.Suspended, cancellationToken),
                await employerRepository.CountAsync(x => x.Status == EmployerStatus.Banned, cancellationToken));

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.EmployerAllDependency()),
                cancellationToken);

            return model;
        }

        private async Task<DashboardJobApplicationStatisticsModel> GetJobApplicationStatisticsAsync(
            IRepository<JobApplication> jobApplicationRepository,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.DashboardJobApplicationStatisticsKey();
            DashboardJobApplicationStatisticsModel? cached = await CacheService.GetAsync<DashboardJobApplicationStatisticsModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            DashboardJobApplicationStatisticsModel model = new(
                await jobApplicationRepository.CountAsync(cancellationToken),
                await jobApplicationRepository.CountAsync(x => x.Status == JobApplicationStatus.Pending, cancellationToken),
                await jobApplicationRepository.CountAsync(x => x.Status == JobApplicationStatus.Accepted, cancellationToken),
                await jobApplicationRepository.CountAsync(x => x.Status == JobApplicationStatus.Rejected, cancellationToken),
                await jobApplicationRepository.CountAsync(x => x.Status == JobApplicationStatus.Withdrawn, cancellationToken));

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.JobApplicationAllDependency()),
                cancellationToken);

            return model;
        }

        private async Task<DashboardJobPostingStatisticsModel> GetJobPostingStatisticsAsync(
            IRepository<JobPosting> jobPostingRepository,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.DashboardJobPostingStatisticsKey();
            DashboardJobPostingStatisticsModel? cached = await CacheService.GetAsync<DashboardJobPostingStatisticsModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            DashboardJobPostingStatisticsModel model = new(
                await jobPostingRepository.CountAsync(cancellationToken),
                await jobPostingRepository.CountAsync(x => x.Status == JobPostingStatus.Draft, cancellationToken),
                await jobPostingRepository.CountAsync(x => x.Status == JobPostingStatus.Open, cancellationToken),
                await jobPostingRepository.CountAsync(x => x.Status == JobPostingStatus.Filled, cancellationToken),
                await jobPostingRepository.CountAsync(x => x.Status == JobPostingStatus.Completed, cancellationToken),
                await jobPostingRepository.CountAsync(x => x.Status == JobPostingStatus.Cancelled, cancellationToken));

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.JobPostingAllDependency()),
                cancellationToken);

            return model;
        }

        private async Task<DashboardSystemUserStatisticsModel> GetSystemUserStatisticsAsync(
            IRepository<SystemUser> systemUserRepository,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.DashboardSystemUserStatisticsKey();
            DashboardSystemUserStatisticsModel? cached = await CacheService.GetAsync<DashboardSystemUserStatisticsModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset startOfTodayUtc = new(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset startOfTomorrowUtc = startOfTodayUtc.AddDays(1);

            DashboardSystemUserStatisticsModel model = new(
                await systemUserRepository.CountAsync(cancellationToken),
                await systemUserRepository.CountAsync(x => x.AccountStatus == AccountStatus.Pending, cancellationToken),
                await systemUserRepository.CountAsync(x => x.AccountStatus == AccountStatus.Active, cancellationToken),
                await systemUserRepository.CountAsync(x => x.AccountStatus == AccountStatus.Suspended, cancellationToken),
                await systemUserRepository.CountAsync(x => x.AccountStatus == AccountStatus.Banned, cancellationToken),
                await systemUserRepository.CountAsync(
                    x => x.EmailVerifiedAt.HasValue
                         && x.EmailVerifiedAt.Value >= startOfTodayUtc
                         && x.EmailVerifiedAt.Value < startOfTomorrowUtc,
                    cancellationToken));

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.SystemUserAllDependency()),
                cancellationToken);

            return model;
        }

        private async Task<DashboardWorkerStatisticsModel> GetWorkerStatisticsAsync(
            IRepository<Worker> workerRepository,
            IRepository<SystemUser> systemUserRepository,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.DashboardWorkerStatisticsKey();
            DashboardWorkerStatisticsModel? cached = await CacheService.GetAsync<DashboardWorkerStatisticsModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            DashboardWorkerStatisticsModel model = new(
                await workerRepository.CountAsync(cancellationToken),
                await systemUserRepository.CountAsync(
                    x => x.Type == SystemUserType.Worker && x.AccountStatus == AccountStatus.Pending,
                    cancellationToken),
                await systemUserRepository.CountAsync(
                    x => x.Type == SystemUserType.Worker && x.AccountStatus == AccountStatus.Active,
                    cancellationToken),
                await systemUserRepository.CountAsync(
                    x => x.Type == SystemUserType.Worker && x.AccountStatus == AccountStatus.Suspended,
                    cancellationToken),
                await systemUserRepository.CountAsync(
                    x => x.Type == SystemUserType.Worker && x.AccountStatus == AccountStatus.Banned,
                    cancellationToken));

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerAllDependency(),
                    AdaIsCacheKeys.SystemUserAllDependency()),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
