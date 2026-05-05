namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using System.Globalization;

    /// <summary>
    /// Logical <see cref="CacheKey"/> values and <see cref="CacheDependency"/> tags for AdaIs read models.
    /// Query entries use namespace <c>query</c> and depend on domain aggregate names so command handlers can invalidate via <see cref="ICacheService.InvalidateByDependencyAsync"/>.
    /// </summary>
    internal static class AdaIsCacheKeys
    {
        #region Fields

        private const string QueryNamespace = "query";

        private const string DetailSuffix = "Detail";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Default TTLs for serialized detail DTOs (L1 short, L2 optional per Core defaults on <see cref="CacheEntryOptions"/>).
        /// </summary>
        /// <param name="dependencies">Aggregate roots that should evict this entry when invalidated.</param>
        internal static CacheEntryOptions DetailReadModelOptions(params CacheDependency[] dependencies) =>
            new()
            {
                Dependencies = dependencies,
            };

        /// <summary>
        /// Cache key for system-user counter block used by dashboard statistics.
        /// </summary>
        internal static CacheKey DashboardSystemUserStatisticsKey() =>
            new(QueryNamespace, "DashboardSystemUserStatistics", "default");

        /// <summary>
        /// Cache key for worker counter block used by dashboard statistics.
        /// </summary>
        internal static CacheKey DashboardWorkerStatisticsKey() =>
            new(QueryNamespace, "DashboardWorkerStatistics", "default");

        /// <summary>
        /// Cache key for employer counter block used by dashboard statistics.
        /// </summary>
        internal static CacheKey DashboardEmployerStatisticsKey() =>
            new(QueryNamespace, "DashboardEmployerStatistics", "default");

        /// <summary>
        /// Cache key for job-posting counter block used by dashboard statistics.
        /// </summary>
        internal static CacheKey DashboardJobPostingStatisticsKey() =>
            new(QueryNamespace, "DashboardJobPostingStatistics", "default");

        /// <summary>
        /// Cache key for job-application counter block used by dashboard statistics.
        /// </summary>
        internal static CacheKey DashboardJobApplicationStatisticsKey() =>
            new(QueryNamespace, "DashboardJobApplicationStatistics", "default");

        /// <summary>
        /// Cache key for overdue posting/application summary used by scheduler/reporting.
        /// </summary>
        internal static CacheKey DashboardOverdueJobSummaryKey() =>
            new(QueryNamespace, "DashboardOverdueJobSummary", "default");

        /// <summary>
        /// Cache key for monetization summary counters.
        /// </summary>
        internal static CacheKey DashboardMonetizationSummaryKey() =>
            new(QueryNamespace, "DashboardMonetizationSummary", "default");

        /// <summary>
        /// Cache key for overdue alarms CSV export package.
        /// </summary>
        internal static CacheKey OverdueAlarmExportPackageKey() =>
            new(QueryNamespace, "OverdueAlarmExportPackage", "default");

        /// <summary>
        /// Cache key for <see cref="EmployerDetailModel"/> by employer id.
        /// </summary>
        internal static CacheKey EmployerDetailKey(int employerId) =>
            new(QueryNamespace, nameof(Employer) + DetailSuffix, employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for employer commission policy detail.
        /// </summary>
        internal static CacheKey EmployerCommissionPolicyKey(int employerId) =>
            new(QueryNamespace, "EmployerCommissionPolicyDetail", employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for employer commission estimate detail.
        /// </summary>
        internal static CacheKey EmployerCommissionEstimateKey(int employerId) =>
            new(QueryNamespace, "EmployerCommissionEstimateDetail", employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for employer commission summary list by limit.
        /// </summary>
        internal static CacheKey EmployerCommissionSummaryListKey(int limit) =>
            new(QueryNamespace, "EmployerCommissionSummaryList", limit.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for employer commission policy CSV export package.
        /// </summary>
        internal static CacheKey EmployerCommissionPolicyExportPackageKey() =>
            new(QueryNamespace, "EmployerCommissionPolicyExportPackage", "default");

        /// <summary>
        /// Invalidation tag for an <see cref="Employer"/> aggregate instance.
        /// </summary>
        internal static CacheDependency EmployerDependency(int employerId) =>
            new(nameof(Employer), employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for aggregate-wide <see cref="Employer"/> counters.
        /// </summary>
        internal static CacheDependency EmployerAllDependency() =>
            new(nameof(Employer), "all");

        /// <summary>
        /// Invalidation tag for employer commission policy read model.
        /// </summary>
        internal static CacheDependency EmployerCommissionPolicyDependency(int employerId) =>
            new("EmployerCommissionPolicy", employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for <see cref="ListJobPostingsByEmployerIdQuery"/> result (summary rows per employer).
        /// </summary>
        internal static CacheKey EmployerJobPostingsSummaryKey(int employerId) =>
            new(QueryNamespace, "JobPostingListByEmployer", employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for the employer-scoped job posting list read model (any posting change under that employer).
        /// </summary>
        internal static CacheDependency EmployerJobPostingsSummaryDependency(int employerId) =>
            new("JobPostingListByEmployer", employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for <see cref="JobPostingDetailModel"/> by posting id.
        /// </summary>
        internal static CacheKey JobPostingDetailKey(int jobPostingId) =>
            new(QueryNamespace, nameof(JobPosting) + DetailSuffix, jobPostingId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for semantic matched postings by worker and limit.
        /// </summary>
        internal static CacheKey JobPostingSemanticMatchedListKey(int workerId, int limit) =>
            new(QueryNamespace, "JobPostingSemanticMatchedList", $"{workerId}:{limit}");

        /// <summary>
        /// Invalidation tag for a <see cref="JobPosting"/> aggregate instance.
        /// </summary>
        internal static CacheDependency JobPostingDependency(int jobPostingId) =>
            new(nameof(JobPosting), jobPostingId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for aggregate-wide <see cref="JobPosting"/> counters.
        /// </summary>
        internal static CacheDependency JobPostingAllDependency() =>
            new(nameof(JobPosting), "all");

        /// <summary>
        /// Cache key for <see cref="WorkerDetailModel"/> by worker id.
        /// </summary>
        internal static CacheKey WorkerDetailKey(int workerId) =>
            new(QueryNamespace, nameof(Worker) + DetailSuffix, workerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for worker-personalized notification preview for a posting.
        /// </summary>
        internal static CacheKey WorkerNotificationPreviewKey(int workerId, int jobPostingId) =>
            new(QueryNamespace, "WorkerNotificationPreview", $"{workerId}:{jobPostingId}");

        /// <summary>
        /// Invalidation tag for a <see cref="Worker"/> aggregate instance.
        /// </summary>
        internal static CacheDependency WorkerDependency(int workerId) =>
            new(nameof(Worker), workerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for aggregate-wide <see cref="Worker"/> counters.
        /// </summary>
        internal static CacheDependency WorkerAllDependency() =>
            new(nameof(Worker), "all");

        /// <summary>
        /// Invalidation tag for aggregate-wide <see cref="SystemUser"/> counters.
        /// </summary>
        internal static CacheDependency SystemUserAllDependency() =>
            new(nameof(SystemUser), "all");

        /// <summary>
        /// Invalidation tag for aggregate-wide <see cref="JobApplication"/> counters.
        /// </summary>
        internal static CacheDependency JobApplicationAllDependency() =>
            new(nameof(JobApplication), "all");

        /// <summary>
        /// Invalidation tag for a <see cref="ShiftAssignment"/> aggregate instance.
        /// </summary>
        internal static CacheDependency ShiftAssignmentDependency(int assignmentId) =>
            new(nameof(ShiftAssignment), assignmentId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for aggregate-wide <see cref="ShiftAssignment"/> read models.
        /// </summary>
        internal static CacheDependency ShiftAssignmentAllDependency() =>
            new(nameof(ShiftAssignment), "all");

        /// <summary>
        /// Invalidation tag for aggregate-wide <see cref="OverdueJobAlarm"/> read models.
        /// </summary>
        internal static CacheDependency OverdueAlarmAllDependency() =>
            new(nameof(OverdueJobAlarm), "all");

        /// <summary>
        /// Cache key for commission receivable detail by employer and period.
        /// </summary>
        internal static CacheKey CommissionReceivableDetailKey(int employerId, DateOnly periodStart, DateOnly periodEnd) =>
            new(
                QueryNamespace,
                "CommissionReceivableDetail",
                $"{employerId}:{periodStart:yyyyMMdd}:{periodEnd:yyyyMMdd}");

        /// <summary>
        /// Invalidation tag for employer-scoped commission receivable models.
        /// </summary>
        internal static CacheDependency CommissionReceivableDependency(int employerId) =>
            new(nameof(CommissionReceivable), employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for aggregate-wide commission receivable models.
        /// </summary>
        internal static CacheDependency CommissionReceivableAllDependency() =>
            new(nameof(CommissionReceivable), "all");

        #endregion Methods
    }
}
