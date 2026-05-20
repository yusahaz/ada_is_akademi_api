namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;

    public class RemoveWorkerAvailabilityCommand : CommandBase
    {
        public int AvailabilityId { get; set; }
    }

    internal class RemoveWorkerAvailabilityCommandValidator : IRequestValidator<RemoveWorkerAvailabilityCommand>
    {
        public ValidationResult Validate(RemoveWorkerAvailabilityCommand request)
            => request.AvailabilityId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.AvailabilityId))]);
    }

    internal class RemoveWorkerAvailabilityCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionUnitCommandHandlerBase<RemoveWorkerAvailabilityCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(RemoveWorkerAvailabilityCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            worker.RemoveAvailability(command.AvailabilityId);
            await SaveWorkerChangesAndInvalidateReadModelsAsync(workerId, cancellationToken);
            return Unit.Value;
        }
    }
}
