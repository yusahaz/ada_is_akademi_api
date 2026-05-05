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
    /// Marks processing worker payout as failed.
    /// </summary>
    public class FailWorkerPayoutCommand :
        CommandBase
    {
        #region Properties

        public string? Reason { get; set; }
        public int WorkerPayoutId { get; set; }

        #endregion Properties
    }

    internal class FailWorkerPayoutCommandValidator : IRequestValidator<FailWorkerPayoutCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(FailWorkerPayoutCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.WorkerPayoutId <= 0)
            {
                failures.Add(ApplicationValidationCodes.FailWorkerPayoutWorkerPayoutId.ForField(nameof(FailWorkerPayoutCommand.WorkerPayoutId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class FailWorkerPayoutCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<FailWorkerPayoutCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(FailWorkerPayoutCommand command, CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            WorkerPayout? payout = await UnitOfWork
                .GetRepository<WorkerPayout>()
                .Filter(x => x.Id == command.WorkerPayoutId)
                .FirstOrDefaultAsync(cancellationToken);
            payout = payout.ThrowIfNull(AzoxiaErrorCodes.NotFound);
            (payout.EmployerId == employerId).ThrowIfFalse(ApplicationValidationCodes.ActorResourceAccessDenied);

            payout.Fail(command.Reason);

            UnitOfWork.Add(new CommissionAuditLog(
                employerId,
                CommissionAuditEventType.WorkerPayoutFailed,
                payout.CommissionAmount,
                assignmentId: payout.AssignmentId,
                workerPayoutId: payout.Id,
                note: command.Reason));

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutDependency(payout.Id), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutEmployerDependency(employerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.WorkerPayoutWorkerDependency(payout.WorkerId), cancellationToken);
            await CacheService.InvalidateByDependencyAsync(AdaIsCacheKeys.CommissionAuditLogAllDependency(), cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
