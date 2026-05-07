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
    /// Retries failed worker payout and returns it to pending.
    /// </summary>
    public class RetryWorkerPayoutCommand :
        CommandBase<WorkerPayoutSnapshotModel>
    {
        #region Properties

        public int WorkerPayoutId { get; set; }

        #endregion Properties
    }

    internal class RetryWorkerPayoutCommandValidator : IRequestValidator<RetryWorkerPayoutCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RetryWorkerPayoutCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.WorkerPayoutId <= 0)
            {
                failures.Add(ApplicationValidationCodes.RetryWorkerPayoutWorkerPayoutId.ForField(nameof(RetryWorkerPayoutCommand.WorkerPayoutId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class RetryWorkerPayoutCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RetryWorkerPayoutCommand, WorkerPayoutSnapshotModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<WorkerPayoutSnapshotModel> HandleAsync(RetryWorkerPayoutCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            WorkerPayout? payout = await UnitOfWork
                .GetRepository<WorkerPayout>()
                .Filter(x => x.Id == command.WorkerPayoutId)
                .FirstOrDefaultAsync(cancellationToken);
            payout = payout.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (payout.EmployerId == employerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            payout.Retry();

            UnitOfWork.Add(new CommissionAuditLog(
                employerId,
                CommissionAuditEventType.WorkerPayoutRetried,
                payout.CommissionAmount,
                assignmentId: payout.AssignmentId,
                workerPayoutId: payout.Id));

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutDependency(payout.Id), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutEmployerDependency(employerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutWorkerDependency(payout.WorkerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.CommissionAuditLogAllDependency(), cancellationToken);

            return new WorkerPayoutSnapshotModel(
                payout.Id,
                payout.Status,
                false,
                DateTimeOffset.UtcNow);
        }

        #endregion Utils
    }
}
