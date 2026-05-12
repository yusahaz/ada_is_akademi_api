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
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Marks worker payout as processing by employer.
    /// </summary>
    public class MarkWorkerPayoutAsProcessingCommand :
        CommandBase<WorkerPayoutSnapshotModel>
    {
        #region Properties

        public int WorkerPayoutId { get; set; }

        #endregion Properties
    }

    internal class MarkWorkerPayoutAsProcessingCommandValidator : IRequestValidator<MarkWorkerPayoutAsProcessingCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(MarkWorkerPayoutAsProcessingCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.WorkerPayoutId <= 0)
            {
                failures.Add(ApplicationValidationCodes.MarkWorkerPayoutAsProcessingWorkerPayoutId.ForField(nameof(MarkWorkerPayoutAsProcessingCommand.WorkerPayoutId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class MarkWorkerPayoutAsProcessingCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<MarkWorkerPayoutAsProcessingCommand, WorkerPayoutSnapshotModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<WorkerPayoutSnapshotModel> HandleAsync(MarkWorkerPayoutAsProcessingCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            WorkerPayout? payout = await UnitOfWork
                .GetRepository<WorkerPayout>()
                .Filter(x => x.Id == command.WorkerPayoutId)
                .Include(x => x.ShiftAssignment)
                .FirstOrDefaultAsync(cancellationToken);
            payout = payout.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (payout.EmployerId == employerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            bool assignmentIsDisputed = payout.ShiftAssignment.Status != ShiftAssignmentStatus.CheckedOut;
            payout.MarkAsProcessing(assignmentIsDisputed);

            UnitOfWork.Add(new CommissionAuditLog(
                employerId,
                CommissionAuditEventType.WorkerPayoutMarkedAsPaid,
                payout.CommissionAmount,
                assignmentId: payout.AssignmentId,
                workerPayoutId: payout.Id));

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return new WorkerPayoutSnapshotModel(
                payout.Id,
                payout.Status,
                payout.Status == WorkerPayoutStatus.Processing,
                payout.ProcessingMarkedAt ?? DateTimeOffset.UtcNow);
        }

        #endregion Utils
    }
}
