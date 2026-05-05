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
    /// Confirms processing payout by payout owner worker.
    /// </summary>
    public class ConfirmWorkerPayoutCommand :
        CommandBase
    {
        #region Properties

        public int WorkerPayoutId { get; set; }

        #endregion Properties
    }

    internal class ConfirmWorkerPayoutCommandValidator : IRequestValidator<ConfirmWorkerPayoutCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ConfirmWorkerPayoutCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.WorkerPayoutId <= 0)
            {
                failures.Add(ApplicationValidationCodes.ConfirmWorkerPayoutWorkerPayoutId.ForField(nameof(ConfirmWorkerPayoutCommand.WorkerPayoutId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ConfirmWorkerPayoutCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<ConfirmWorkerPayoutCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(ConfirmWorkerPayoutCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            WorkerPayout? payout = await UnitOfWork
                .GetRepository<WorkerPayout>()
                .Filter(x => x.Id == command.WorkerPayoutId)
                .FirstOrDefaultAsync(cancellationToken);
            payout = payout.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (payout.WorkerId == workerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            payout.ConfirmPaid();

            UnitOfWork.Add(new CommissionAuditLog(
                payout.EmployerId,
                CommissionAuditEventType.WorkerPayoutConfirmed,
                payout.CommissionAmount,
                assignmentId: payout.AssignmentId,
                workerPayoutId: payout.Id));

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutDependency(payout.Id), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutEmployerDependency(payout.EmployerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutWorkerDependency(workerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.CommissionAuditLogAllDependency(), cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
