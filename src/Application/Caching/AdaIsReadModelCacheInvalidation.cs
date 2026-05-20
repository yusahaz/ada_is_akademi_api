namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Caching;

    /// <summary>
    /// Explicit read-model cache eviction for command handlers (in addition to domain-event handlers).
    /// Prefer these helpers over ad-hoc <see cref="ICacheService.InvalidateByDependencyAsync"/> calls so direct cache keys are always cleared.
    /// </summary>
    internal static class AdaIsReadModelCacheInvalidation
    {
        #region Methods

        /// <summary>
        /// Evicts worker-scoped query caches; optionally removes self detail keys and system-user list scopes.
        /// </summary>
        public static async Task InvalidateWorkerReadModelsAsync(
            ICacheService cacheService,
            int workerId,
            CancellationToken cancellationToken,
            bool invalidateSystemUserScopes = false,
            bool removeSelfDetailKeys = true)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerDependency(workerId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerAllDependency(),
                cancellationToken);

            if (removeSelfDetailKeys)
            {
                await cacheService.RemoveAsync(AdaIsCacheKeys.WorkerSelfDetailKey(workerId), cancellationToken);
                await cacheService.RemoveAsync(AdaIsCacheKeys.WorkerSelfFullDetailKey(workerId), cancellationToken);
            }

            if (invalidateSystemUserScopes)
            {
                await InvalidateSystemUserAggregateListAsync(cacheService, cancellationToken);
            }
        }

        /// <summary>
        /// Evicts job posting and related listing caches for a posting mutation.
        /// </summary>
        public static async Task InvalidateJobPostingReadModelsAsync(
            ICacheService cacheService,
            int jobPostingId,
            int employerId,
            CancellationToken cancellationToken,
            bool includeApplicationScopes = true)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingDependency(jobPostingId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerJobPostingsSummaryDependency(employerId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingAllDependency(),
                cancellationToken);
            await cacheService.RemoveAsync(AdaIsCacheKeys.JobPostingDetailKey(jobPostingId), cancellationToken);

            if (includeApplicationScopes)
            {
                await cacheService.InvalidateByDependencyAsync(
                    AdaIsCacheKeys.JobApplicationAllDependency(),
                    cancellationToken);
            }
        }

        /// <summary>
        /// Evicts employer-scoped read models after employer profile or configuration mutations.
        /// </summary>
        public static async Task InvalidateEmployerReadModelsAsync(
            ICacheService cacheService,
            int employerId,
            CancellationToken cancellationToken,
            bool invalidateSystemUserScopes = false)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerDependency(employerId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerAllDependency(),
                cancellationToken);
            await cacheService.RemoveAsync(AdaIsCacheKeys.EmployerDetailKey(employerId), cancellationToken);
            await cacheService.RemoveAsync(AdaIsCacheKeys.EmployerFullDetailKey(employerId), cancellationToken);

            if (invalidateSystemUserScopes)
            {
                await InvalidateSystemUserAggregateListAsync(cacheService, cancellationToken);
            }
        }

        /// <summary>
        /// Evicts commission policy and receivable projections for an employer.
        /// </summary>
        public static async Task InvalidateEmployerCommissionReadModelsAsync(
            ICacheService cacheService,
            int employerId,
            CancellationToken cancellationToken)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerCommissionPolicyDependency(employerId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.CommissionReceivableDependency(employerId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.CommissionReceivableAllDependency(),
                cancellationToken);
            await cacheService.RemoveAsync(AdaIsCacheKeys.EmployerCommissionPolicyKey(employerId), cancellationToken);
            await cacheService.RemoveAsync(AdaIsCacheKeys.EmployerCommissionEstimateKey(employerId), cancellationToken);
        }

        /// <summary>
        /// Evicts system-user list and notification inbox scopes.
        /// </summary>
        public static async Task InvalidateSystemUserReadModelsAsync(
            ICacheService cacheService,
            int systemUserId,
            CancellationToken cancellationToken,
            bool invalidateAggregateList = true)
        {
            await InvalidateSystemUserNotificationScopesAsync(
                cacheService,
                systemUserId,
                workerId: null,
                cancellationToken);

            if (invalidateAggregateList)
            {
                await InvalidateSystemUserAggregateListAsync(cacheService, cancellationToken);
            }
        }

        /// <summary>
        /// Evicts notification dispatch and inbox caches for a user and optional linked worker.
        /// </summary>
        public static async Task InvalidateSystemUserNotificationScopesAsync(
            ICacheService cacheService,
            int? systemUserId,
            int? workerId,
            CancellationToken cancellationToken,
            bool invalidateDispatchAll = true)
        {
            if (systemUserId.HasValue && systemUserId.Value > 0)
            {
                await cacheService.InvalidateByDependencyAsync(
                    AdaIsCacheKeys.SystemUserNotificationDispatchDependency(systemUserId.Value),
                    cancellationToken);
            }

            if (workerId.HasValue && workerId.Value > 0)
            {
                await cacheService.InvalidateByDependencyAsync(
                    AdaIsCacheKeys.SystemUserNotificationDispatchWorkerDependency(workerId.Value),
                    cancellationToken);
            }

            if (invalidateDispatchAll)
            {
                await cacheService.InvalidateByDependencyAsync(
                    AdaIsCacheKeys.SystemUserNotificationDispatchAllDependency(),
                    cancellationToken);
            }
        }

        /// <summary>
        /// Evicts shift assignment listings and related worker live feeds.
        /// </summary>
        public static async Task InvalidateShiftAssignmentReadModelsAsync(
            ICacheService cacheService,
            int assignmentId,
            int workerId,
            CancellationToken cancellationToken,
            int? employerId = null)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.ShiftAssignmentDependency(assignmentId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.ShiftAssignmentAllDependency(),
                cancellationToken);
            await InvalidateWorkerReadModelsAsync(
                cacheService,
                workerId,
                cancellationToken,
                removeSelfDetailKeys: false);

            if (employerId.HasValue && employerId.Value > 0)
            {
                await cacheService.InvalidateByDependencyAsync(
                    AdaIsCacheKeys.EmployerDependency(employerId.Value),
                    cancellationToken);
            }
        }

        /// <summary>
        /// Evicts payout and commission audit projections after payout lifecycle changes.
        /// </summary>
        public static async Task InvalidateWorkerPayoutReadModelsAsync(
            ICacheService cacheService,
            int workerPayoutId,
            int employerId,
            int workerId,
            CancellationToken cancellationToken)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerPayoutDependency(workerPayoutId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerPayoutEmployerDependency(employerId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerPayoutWorkerDependency(workerId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerPayoutAllDependency(),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.CommissionAuditLogAllDependency(),
                cancellationToken);
        }

        /// <summary>
        /// Evicts commission receivable read models for an employer period mutation.
        /// </summary>
        public static async Task InvalidateCommissionReceivableReadModelsAsync(
            ICacheService cacheService,
            int employerId,
            CancellationToken cancellationToken)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.CommissionReceivableDependency(employerId),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.CommissionReceivableAllDependency(),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.CommissionAuditLogAllDependency(),
                cancellationToken);
        }

        /// <summary>
        /// Evicts employer profile view statistics for employer-safe worker detail caches.
        /// </summary>
        public static async Task InvalidateEmployerWorkerProfileViewStatAsync(
            ICacheService cacheService,
            int employerId,
            int workerId,
            CancellationToken cancellationToken)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.EmployerWorkerProfileViewStatDependency(employerId, workerId),
                cancellationToken);
            await cacheService.RemoveAsync(
                AdaIsCacheKeys.WorkerEmployerSafeDetailKey(employerId, workerId),
                cancellationToken);
            await cacheService.RemoveAsync(
                AdaIsCacheKeys.WorkerEmployerSafeFullDetailKey(employerId, workerId),
                cancellationToken);
        }

        /// <summary>
        /// Evicts overdue alarm exports and summaries after sweep mutations.
        /// </summary>
        public static async Task InvalidateOverdueAlarmReadModelsAsync(
            ICacheService cacheService,
            CancellationToken cancellationToken)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.OverdueAlarmAllDependency(),
                cancellationToken);
            await cacheService.RemoveAsync(AdaIsCacheKeys.OverdueAlarmExportPackageKey(), cancellationToken);
            await cacheService.RemoveAsync(AdaIsCacheKeys.DashboardOverdueJobSummaryKey(), cancellationToken);
        }

        /// <summary>
        /// Evicts global skill dictionary and embedding-sensitive listing caches.
        /// </summary>
        public static async Task InvalidateSkillAndEmbeddingListCachesAsync(
            ICacheService cacheService,
            CancellationToken cancellationToken)
        {
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.WorkerAllDependency(),
                cancellationToken);
            await cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.JobPostingAllDependency(),
                cancellationToken);
        }

        /// <summary>
        /// Evicts worker and linked system-user scopes after worker deletion.
        /// </summary>
        public static async Task InvalidateDeletedWorkerReadModelsAsync(
            ICacheService cacheService,
            int workerId,
            CancellationToken cancellationToken)
        {
            await InvalidateWorkerReadModelsAsync(
                cacheService,
                workerId,
                cancellationToken,
                invalidateSystemUserScopes: true);
        }

        /// <summary>
        /// Evicts employer and linked system-user scopes after employer deletion.
        /// </summary>
        public static async Task InvalidateDeletedEmployerReadModelsAsync(
            ICacheService cacheService,
            int employerId,
            CancellationToken cancellationToken)
        {
            await InvalidateEmployerReadModelsAsync(
                cacheService,
                employerId,
                cancellationToken,
                invalidateSystemUserScopes: true);
        }

        private static Task InvalidateSystemUserAggregateListAsync(
            ICacheService cacheService,
            CancellationToken cancellationToken)
            => cacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.SystemUserAllDependency(),
                cancellationToken);

        #endregion Methods
    }
}
