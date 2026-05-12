namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;

    public class RemoveWorkerReferenceCommand : CommandBase
    {
        public int ReferenceId { get; set; }
    }

    internal class RemoveWorkerReferenceCommandValidator : IRequestValidator<RemoveWorkerReferenceCommand>
    {
        public ValidationResult Validate(RemoveWorkerReferenceCommand request)
            => request.ReferenceId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.ReferenceId))]);
    }

    internal class RemoveWorkerReferenceCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionUnitCommandHandlerBase<RemoveWorkerReferenceCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(RemoveWorkerReferenceCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            worker.RemoveReference(command.ReferenceId);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
return Unit.Value;
        }
    }
}
