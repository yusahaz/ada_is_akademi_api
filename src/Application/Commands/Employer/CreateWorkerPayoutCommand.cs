namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Azoxia.Core.ValueTypes;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Creates idempotent worker payout row from a checked-out assignment.
    /// </summary>
    public class CreateWorkerPayoutCommand :
        CommandBase<WorkerPayoutSnapshotModel>
    {
        #region Properties

        /// <summary>
        /// Target shift assignment id.
        /// </summary>
        public int AssignmentId { get; set; }

        #endregion Properties
    }

    internal class CreateWorkerPayoutCommandValidator : IRequestValidator<CreateWorkerPayoutCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(CreateWorkerPayoutCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.AssignmentId <= 0)
            {
                failures.Add(ApplicationValidationCodes.CreateWorkerPayoutAssignmentId.ForField(nameof(CreateWorkerPayoutCommand.AssignmentId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class CreateWorkerPayoutCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<CreateWorkerPayoutCommand, WorkerPayoutSnapshotModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<WorkerPayoutSnapshotModel> HandleAsync(CreateWorkerPayoutCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            WorkerPayout? existing = await UnitOfWork
                .GetRepository<WorkerPayout>()
                .Filter(x => x.AssignmentId == command.AssignmentId)
                .FirstOrDefaultAsync(cancellationToken);
            if (existing is not null)
            {
                DateTimeOffset existingUpdatedAt = existing.PaidAt
                    ?? existing.FailedAt
                    ?? existing.ProcessingMarkedAt
                    ?? existing.CreatedAt;
                return new WorkerPayoutSnapshotModel(
                    existing.Id,
                    existing.Status,
                    existing.Status == WorkerPayoutStatus.Processing,
                    existingUpdatedAt);
            }

            ShiftAssignment? assignment = await UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x => x.Id == command.AssignmentId)
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(cancellationToken);
            assignment = assignment.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (assignment.JobPosting.EmployerId == employerId)
                .ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);
            (assignment.Status == ShiftAssignmentStatus.CheckedOut)
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentInvalidStatusTransition);

            Employer? employer = await UnitOfWork.GetRepository<Employer>().GetByIdAsync(employerId, cancellationToken);
            employer = employer.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            Money grossAmount = assignment.JobPosting.Wage;
            Money commissionAmount = new(grossAmount.Amount * employer.CommissionRate, grossAmount.Currency);

            WorkerPayout payout = new(
                assignment.Id,
                employerId,
                assignment.WorkerId,
                grossAmount,
                commissionAmount);

            UnitOfWork.Add(payout);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            UnitOfWork.Add(new CommissionAuditLog(
                employerId,
                CommissionAuditEventType.WorkerPayoutCreated,
                commissionAmount,
                assignmentId: assignment.Id,
                workerPayoutId: payout.Id,
                note: "created_from_checked_out_assignment"));
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutDependency(payout.Id), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutEmployerDependency(employerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutWorkerDependency(assignment.WorkerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutAllDependency(), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.CommissionAuditLogAllDependency(), cancellationToken);

            return new WorkerPayoutSnapshotModel(
                payout.Id,
                payout.Status,
                false,
                payout.CreatedAt);
        }

        #endregion Utils
    }
}
