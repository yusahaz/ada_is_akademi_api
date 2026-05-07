namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using System.Globalization;
    using System.Text.Json;

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
        /// Cache key for financial reconciliation summary counters and per-currency totals.
        /// </summary>
        internal static CacheKey DashboardFinancialReconciliationSummaryKey() =>
            new(QueryNamespace, "DashboardFinancialReconciliationSummary", "default");

        /// <summary>
        /// Cache key for filtered financial reconciliation rows.
        /// </summary>
        internal static CacheKey FinancialReconciliationRowsKey(
            int? employerId,
            DateTimeOffset? from,
            DateTimeOffset? to,
            int limit,
            int offset) =>
            new(
                QueryNamespace,
                "FinancialReconciliationRows",
                $"{employerId?.ToString(CultureInfo.InvariantCulture) ?? "all"}:{from?.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture) ?? "null"}:{to?.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture) ?? "null"}:{limit.ToString(CultureInfo.InvariantCulture)}:{offset.ToString(CultureInfo.InvariantCulture)}");

        /// <summary>
        /// Cache key for overdue alarms CSV export package.
        /// </summary>
        internal static CacheKey OverdueAlarmExportPackageKey() =>
            new(QueryNamespace, "OverdueAlarmExportPackage", "default");

        /// <summary>
        /// Cache key for system-user notification dispatch CSV export package.
        /// </summary>
        internal static CacheKey SystemUserNotificationDispatchExportPackageKey() =>
            new(QueryNamespace, "SystemUserNotificationDispatchExportPackage", "default");

        /// <summary>
        /// Cache key for <see cref="EmployerDetailModel"/> by employer id.
        /// </summary>
        internal static CacheKey EmployerDetailKey(int employerId) =>
            new(QueryNamespace, nameof(Employer) + DetailSuffix, employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for <see cref="EmployerFullDetailModel"/> by employer id.
        /// </summary>
        internal static CacheKey EmployerFullDetailKey(int employerId) =>
            new(QueryNamespace, "EmployerFullDetail", employerId.ToString(CultureInfo.InvariantCulture));

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
        /// Cache key for filtered employer listing queries.
        /// </summary>
        internal static CacheKey EmployerListKey(ListEmployersQuery query) =>
            new(QueryNamespace, "EmployerList", JsonSerializer.Serialize(query));

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
        /// Invalidation tag for a single <see cref="EmployerWorkerProfileViewStat"/> row (employer + worker pair).
        /// </summary>
        internal static CacheDependency EmployerWorkerProfileViewStatDependency(int employerId, int workerId) =>
            new(nameof(EmployerWorkerProfileViewStat), $"{employerId.ToString(CultureInfo.InvariantCulture)}:{workerId.ToString(CultureInfo.InvariantCulture)}");

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
        /// Cache key for <see cref="ListJobPostingsByEmployerIdQuery"/> result (summary rows per employer page).
        /// </summary>
        internal static CacheKey EmployerJobPostingsSummaryKey(int employerId, int limit, int offset) =>
            new(QueryNamespace, "JobPostingListByEmployer", $"{employerId}:{limit}:{offset}");

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
        /// Cache key for open job posting list by paging window.
        /// </summary>
        internal static CacheKey OpenJobPostingListKey(int limit, int offset, string? countryCode = null) =>
            new(QueryNamespace, "JobPostingOpenList", $"{limit}:{offset}:{countryCode ?? "all"}");

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
        /// Cache key for <see cref="WorkerEmployerSafeDetailModel"/> scoped by employer and worker identifiers.
        /// </summary>
        internal static CacheKey WorkerEmployerSafeDetailKey(int employerId, int workerId) =>
            new(
                QueryNamespace,
                "WorkerEmployerSafeDetail",
                $"{employerId.ToString(CultureInfo.InvariantCulture)}:{workerId.ToString(CultureInfo.InvariantCulture)}");

        /// <summary>
        /// Cache key for <see cref="WorkerEmployerSafeFullDetailModel"/> scoped by employer and worker identifiers.
        /// </summary>
        internal static CacheKey WorkerEmployerSafeFullDetailKey(int employerId, int workerId) =>
            new(
                QueryNamespace,
                "WorkerEmployerSafeFullDetail",
                $"{employerId.ToString(CultureInfo.InvariantCulture)}:{workerId.ToString(CultureInfo.InvariantCulture)}");

        /// <summary>
        /// Cache key for <see cref="WorkerSelfDetailModel"/> for the authenticated worker actor.
        /// </summary>
        internal static CacheKey WorkerSelfDetailKey(int workerId) =>
            new(QueryNamespace, "WorkerSelfDetail", workerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for <see cref="WorkerSelfFullDetailModel"/> for the authenticated worker actor.
        /// </summary>
        internal static CacheKey WorkerSelfFullDetailKey(int workerId) =>
            new(QueryNamespace, "WorkerSelfFullDetail", workerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for worker-personalized notification preview for a posting.
        /// </summary>
        internal static CacheKey WorkerNotificationPreviewKey(int workerId, int jobPostingId) =>
            new(QueryNamespace, "WorkerNotificationPreview", $"{workerId}:{jobPostingId}");

        /// <summary>
        /// Cache key for worker live status feed by limit.
        /// </summary>
        internal static CacheKey WorkerLiveStatusFeedKey(int workerId, int limit) =>
            new(QueryNamespace, "WorkerLiveStatusFeed", $"{workerId}:{limit}");

        /// <summary>
        /// Cache key for filtered worker listing queries.
        /// </summary>
        internal static CacheKey WorkerListKey(ListWorkersQuery query) =>
            new(QueryNamespace, "WorkerList", JsonSerializer.Serialize(query));

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
        /// Cache key for filtered system-user listing queries.
        /// </summary>
        internal static CacheKey SystemUserListKey(ListSystemUsersQuery query) =>
            new(QueryNamespace, "SystemUserList", JsonSerializer.Serialize(query));

        /// <summary>
        /// Cache key for authenticated system-user notification inbox pages.
        /// </summary>
        internal static CacheKey SystemUserNotificationInboxKey(int systemUserId, bool? isRead, int limit, int offset) =>
            new(QueryNamespace, "SystemUserNotificationInbox", $"{systemUserId}:{isRead?.ToString() ?? "all"}:{limit}:{offset}");

        /// <summary>
        /// Invalidation tag for aggregate-wide <see cref="SystemUserGroup"/> read models.
        /// </summary>
        internal static CacheDependency SystemUserGroupAllDependency() =>
            new(nameof(SystemUserGroup), "all");

        /// <summary>
        /// Cache key for filtered system-user-group listing queries.
        /// </summary>
        internal static CacheKey SystemUserGroupListKey(ListSystemUserGroupsQuery query) =>
            new(QueryNamespace, "SystemUserGroupList", JsonSerializer.Serialize(query));

        /// <summary>
        /// Invalidation tag for aggregate-wide <see cref="JobApplication"/> counters.
        /// </summary>
        internal static CacheDependency JobApplicationAllDependency() =>
            new(nameof(JobApplication), "all");

        /// <summary>
        /// Cache key for worker-scoped job application list by paging window.
        /// </summary>
        internal static CacheKey WorkerJobApplicationListKey(int workerId, int limit, int offset) =>
            new(QueryNamespace, "WorkerJobApplicationList", $"{workerId}:{limit}:{offset}");

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
        /// Cache key for worker-scoped shift assignment list by paging window.
        /// </summary>
        internal static CacheKey WorkerShiftAssignmentListKey(int workerId, int limit, int offset) =>
            new(QueryNamespace, "WorkerShiftAssignmentList", $"{workerId}:{limit}:{offset}");

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
        /// Cache key for commission receivable list by employer and paging window.
        /// </summary>
        internal static CacheKey CommissionReceivableListKey(int employerId, int limit, int offset) =>
            new(
                QueryNamespace,
                "CommissionReceivableList",
                $"{employerId}:{limit}:{offset}");

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

        /// <summary>
        /// Invalidation tag for a single <see cref="WorkerPayout"/> row.
        /// </summary>
        internal static CacheDependency WorkerPayoutDependency(int workerPayoutId) =>
            new(nameof(WorkerPayout), workerPayoutId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for employer-scoped worker payout read models.
        /// </summary>
        internal static CacheDependency WorkerPayoutEmployerDependency(int employerId) =>
            new(nameof(WorkerPayout), $"employer:{employerId.ToString(CultureInfo.InvariantCulture)}");

        /// <summary>
        /// Invalidation tag for worker-scoped worker payout read models.
        /// </summary>
        internal static CacheDependency WorkerPayoutWorkerDependency(int workerId) =>
            new(nameof(WorkerPayout), $"worker:{workerId.ToString(CultureInfo.InvariantCulture)}");

        /// <summary>
        /// Invalidation tag for aggregate-wide worker payout models.
        /// </summary>
        internal static CacheDependency WorkerPayoutAllDependency() =>
            new(nameof(WorkerPayout), "all");

        /// <summary>
        /// Invalidation tag for worker-scoped notification dispatch rows.
        /// </summary>
        internal static CacheDependency SystemUserNotificationDispatchWorkerDependency(int workerId) =>
            new(nameof(SystemUserNotificationDispatch), workerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for system-user-scoped notification dispatch rows.
        /// </summary>
        internal static CacheDependency SystemUserNotificationDispatchDependency(int systemUserId) =>
            new(nameof(SystemUserNotificationDispatch), $"systemUser:{systemUserId.ToString(CultureInfo.InvariantCulture)}");

        /// <summary>
        /// Invalidation tag for aggregate-wide notification dispatch rows.
        /// </summary>
        internal static CacheDependency SystemUserNotificationDispatchAllDependency() =>
            new(nameof(SystemUserNotificationDispatch), "all");

        /// <summary>
        /// Invalidation tag for aggregate-wide commission audit models.
        /// </summary>
        internal static CacheDependency CommissionAuditLogAllDependency() =>
            new(nameof(CommissionAuditLog), "all");

        /// <summary>
        /// Cache key for per-user permission resolver effective rule sets.
        /// </summary>
        internal static CacheKey PermissionResolverCacheKey(int systemUserId, int? employerId) =>
            new(
                "auth",
                "PermissionResolver",
                $"{systemUserId.ToString(CultureInfo.InvariantCulture)}:{employerId?.ToString(CultureInfo.InvariantCulture) ?? "null"}");

        /// <summary>
        /// Invalidation tag for aggregate-wide permission resolver memberships.
        /// </summary>
        internal static CacheDependency PermissionResolverMembershipAllDependency() =>
            new(nameof(SystemUserGroupMembership), "all");

        /// <summary>
        /// Invalidation tag for aggregate-wide permission resolver group permission rules.
        /// </summary>
        internal static CacheDependency PermissionResolverGroupPermissionAllDependency() =>
            new(nameof(SystemUserGroupPermission), "all");

        /// <summary>
        /// Invalidation tag for aggregate-wide permission resolver group definitions (activation/deactivation).
        /// </summary>
        internal static CacheDependency PermissionResolverGroupAllDependency() =>
            new(nameof(SystemUserGroup), "all");

        #endregion Methods
    }
}
