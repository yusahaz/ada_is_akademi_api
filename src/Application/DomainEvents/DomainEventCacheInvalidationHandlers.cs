namespace Azoxia.AdaIsAkademi.Application.DomainEvents
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.AdaIsAkademi.Domain.Events;

    using Azoxia.Core.Application.Caching;

    /// <summary>
    /// Invalidates job posting read-model cache entries aligned with employer/worker listing queries.
    /// </summary>
    internal sealed class JobPostingReadModelCacheInvalidationDomainEventHandler(ICacheService cacheService) :
        IDomainEventHandler<JobPostingPublishedEvent>,
        IDomainEventHandler<JobPostingFilledEvent>,
        IDomainEventHandler<JobPostingCancelledEvent>,
        IDomainEventHandler<JobPostingCompletedEvent>,
        IDomainEventHandler<JobApplicationSubmittedEvent>,
        IDomainEventHandler<JobApplicationAcceptedEvent>,
        IDomainEventHandler<JobApplicationRejectedEvent>
    {
        private Task InvalidateRelatedCachesAsync(
            int jobPostingId,
            int employerId,
            CancellationToken cancellationToken)
            => AdaIsReadModelCacheInvalidation.InvalidateJobPostingReadModelsAsync(
                cacheService,
                jobPostingId,
                employerId,
                cancellationToken);

        Task IDomainEventHandler<JobPostingPublishedEvent>.HandleAsync(
            JobPostingPublishedEvent domainEvent,
            CancellationToken cancellationToken)
            => InvalidateRelatedCachesAsync(domainEvent.JobPostingId, domainEvent.EmployerId, cancellationToken);

        Task IDomainEventHandler<JobPostingFilledEvent>.HandleAsync(
            JobPostingFilledEvent domainEvent,
            CancellationToken cancellationToken)
            => InvalidateRelatedCachesAsync(domainEvent.JobPostingId, domainEvent.EmployerId, cancellationToken);

        Task IDomainEventHandler<JobPostingCancelledEvent>.HandleAsync(
            JobPostingCancelledEvent domainEvent,
            CancellationToken cancellationToken)
            => InvalidateRelatedCachesAsync(domainEvent.JobPostingId, domainEvent.EmployerId, cancellationToken);

        Task IDomainEventHandler<JobPostingCompletedEvent>.HandleAsync(
            JobPostingCompletedEvent domainEvent,
            CancellationToken cancellationToken)
            => InvalidateRelatedCachesAsync(domainEvent.JobPostingId, domainEvent.EmployerId, cancellationToken);

        Task IDomainEventHandler<JobApplicationSubmittedEvent>.HandleAsync(
            JobApplicationSubmittedEvent domainEvent,
            CancellationToken cancellationToken)
            => InvalidateRelatedCachesAsync(domainEvent.JobPostingId, domainEvent.EmployerId, cancellationToken);

        Task IDomainEventHandler<JobApplicationAcceptedEvent>.HandleAsync(
            JobApplicationAcceptedEvent domainEvent,
            CancellationToken cancellationToken)
            => InvalidateRelatedCachesAsync(domainEvent.JobPostingId, domainEvent.EmployerId, cancellationToken);

        Task IDomainEventHandler<JobApplicationRejectedEvent>.HandleAsync(
            JobApplicationRejectedEvent domainEvent,
            CancellationToken cancellationToken)
            => InvalidateRelatedCachesAsync(domainEvent.JobPostingId, domainEvent.EmployerId, cancellationToken);
    }

    /// <summary>
    /// Invalidates worker-scoped cached read models when profile aggregates change.
    /// </summary>
    internal sealed class WorkerReadModelCacheInvalidationDomainEventHandler(ICacheService cacheService) :
        IDomainEventHandler<WorkerProfileUpdatedEvent>,
        IDomainEventHandler<WorkerRegisteredEvent>
    {
        private Task InvalidateWorkerCachesAsync(int workerId, CancellationToken cancellationToken)
            => AdaIsReadModelCacheInvalidation.InvalidateWorkerReadModelsAsync(
                cacheService,
                workerId,
                cancellationToken);

        Task IDomainEventHandler<WorkerProfileUpdatedEvent>.HandleAsync(
            WorkerProfileUpdatedEvent domainEvent,
            CancellationToken cancellationToken)
            => InvalidateWorkerCachesAsync(domainEvent.WorkerId, cancellationToken);

        Task IDomainEventHandler<WorkerRegisteredEvent>.HandleAsync(
            WorkerRegisteredEvent domainEvent,
            CancellationToken cancellationToken)
            => InvalidateWorkerCachesAsync(domainEvent.WorkerId, cancellationToken);
    }

    /// <summary>
    /// Keeps CV-related worker read paths coherent after upload session transitions.
    /// </summary>
    internal sealed class CvSessionWorkerCacheInvalidationDomainEventHandler(ICacheService cacheService) :
        IDomainEventHandler<CvUploadedEvent>,
        IDomainEventHandler<CvExtractionCompletedEvent>,
        IDomainEventHandler<CvExtractionFailedEvent>,
        IDomainEventHandler<CvImportConfirmedEvent>,
        IDomainEventHandler<CvImportDiscardedEvent>
    {
        private Task InvalidateWorkerAsync(int workerId, CancellationToken cancellationToken)
            => AdaIsReadModelCacheInvalidation.InvalidateWorkerReadModelsAsync(
                cacheService,
                workerId,
                cancellationToken);

        Task IDomainEventHandler<CvUploadedEvent>.HandleAsync(CvUploadedEvent e, CancellationToken ct)
            => InvalidateWorkerAsync(e.WorkerId, ct);

        Task IDomainEventHandler<CvExtractionCompletedEvent>.HandleAsync(CvExtractionCompletedEvent e, CancellationToken ct)
            => InvalidateWorkerAsync(e.WorkerId, ct);

        Task IDomainEventHandler<CvExtractionFailedEvent>.HandleAsync(CvExtractionFailedEvent e, CancellationToken ct)
            => InvalidateWorkerAsync(e.WorkerId, ct);

        Task IDomainEventHandler<CvImportConfirmedEvent>.HandleAsync(CvImportConfirmedEvent e, CancellationToken ct)
            => InvalidateWorkerAsync(e.WorkerId, ct);

        Task IDomainEventHandler<CvImportDiscardedEvent>.HandleAsync(CvImportDiscardedEvent e, CancellationToken ct)
            => InvalidateWorkerAsync(e.WorkerId, ct);
    }

    /// <summary>
    /// Aligns payout-related cached projections with payout lifecycle transitions.
    /// </summary>
    internal sealed class WorkerPayoutReadModelCacheInvalidationDomainEventHandler(ICacheService cacheService) :
        IDomainEventHandler<WorkerPayoutPendingEvent>,
        IDomainEventHandler<WorkerPayoutMarkedAsPaidEvent>,
        IDomainEventHandler<WorkerPayoutConfirmedEvent>,
        IDomainEventHandler<WorkerPayoutFailedEvent>
    {
        private Task InvalidatePayoutProjectionCachesAsync(
            int workerPayoutId,
            int employerId,
            int workerId,
            CancellationToken cancellationToken)
            => AdaIsReadModelCacheInvalidation.InvalidateWorkerPayoutReadModelsAsync(
                cacheService,
                workerPayoutId,
                employerId,
                workerId,
                cancellationToken);

        Task IDomainEventHandler<WorkerPayoutPendingEvent>.HandleAsync(WorkerPayoutPendingEvent e, CancellationToken ct)
            => InvalidatePayoutProjectionCachesAsync(e.WorkerPayoutId, e.EmployerId, e.WorkerId, ct);

        Task IDomainEventHandler<WorkerPayoutMarkedAsPaidEvent>.HandleAsync(WorkerPayoutMarkedAsPaidEvent e, CancellationToken ct)
            => InvalidatePayoutProjectionCachesAsync(e.WorkerPayoutId, e.EmployerId, e.WorkerId, ct);

        Task IDomainEventHandler<WorkerPayoutConfirmedEvent>.HandleAsync(WorkerPayoutConfirmedEvent e, CancellationToken ct)
            => InvalidatePayoutProjectionCachesAsync(e.WorkerPayoutId, e.EmployerId, e.WorkerId, ct);

        Task IDomainEventHandler<WorkerPayoutFailedEvent>.HandleAsync(WorkerPayoutFailedEvent e, CancellationToken ct)
            => InvalidatePayoutProjectionCachesAsync(e.WorkerPayoutId, e.EmployerId, e.WorkerId, ct);
    }
}
