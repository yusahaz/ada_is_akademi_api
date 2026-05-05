namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;

    public class AddWorkerAvailabilityCommand : CommandBase<int>
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly TimeFrom { get; set; }
        public TimeOnly TimeTo { get; set; }
    }

    internal class AddWorkerAvailabilityCommandValidator : IRequestValidator<AddWorkerAvailabilityCommand>
    {
        public ValidationResult Validate(AddWorkerAvailabilityCommand request)
            => request.TimeTo > request.TimeFrom
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.TimeTo))]);
    }

    internal class AddWorkerAvailabilityCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionCommandHandlerBase<AddWorkerAvailabilityCommand>(serviceProvider)
    {
        protected override async Task<int> HandleAsync(AddWorkerAvailabilityCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            WorkerAvailability item = worker.AddAvailability(command.DayOfWeek, command.TimeFrom, command.TimeTo);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateWorkerAsync(workerId, cancellationToken);
            return item.Id;
        }
    }
}
