namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;

    public class RemoveWorkerEducationCommand : CommandBase
    {
        public int EducationId { get; set; }
    }

    internal class RemoveWorkerEducationCommandValidator : IRequestValidator<RemoveWorkerEducationCommand>
    {
        public ValidationResult Validate(RemoveWorkerEducationCommand request)
            => request.EducationId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.EducationId))]);
    }

    internal class RemoveWorkerEducationCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionUnitCommandHandlerBase<RemoveWorkerEducationCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(RemoveWorkerEducationCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            worker.RemoveEducation(command.EducationId);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
return Unit.Value;
        }
    }
}
