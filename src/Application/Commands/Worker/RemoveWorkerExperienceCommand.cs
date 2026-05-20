namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;

    public class RemoveWorkerExperienceCommand : CommandBase
    {
        public int ExperienceId { get; set; }
    }

    internal class RemoveWorkerExperienceCommandValidator : IRequestValidator<RemoveWorkerExperienceCommand>
    {
        public ValidationResult Validate(RemoveWorkerExperienceCommand request)
            => request.ExperienceId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.ExperienceId))]);
    }

    internal class RemoveWorkerExperienceCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionUnitCommandHandlerBase<RemoveWorkerExperienceCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(RemoveWorkerExperienceCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            worker.RemoveExperience(command.ExperienceId);
            await SaveWorkerChangesAndInvalidateReadModelsAsync(workerId, cancellationToken);
            return Unit.Value;
        }
    }
}
