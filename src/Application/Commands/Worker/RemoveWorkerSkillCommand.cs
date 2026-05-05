namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;

    public class RemoveWorkerSkillCommand : CommandBase
    {
        public int SkillId { get; set; }
    }

    internal class RemoveWorkerSkillCommandValidator : IRequestValidator<RemoveWorkerSkillCommand>
    {
        public ValidationResult Validate(RemoveWorkerSkillCommand request)
            => request.SkillId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.SkillId))]);
    }

    internal class RemoveWorkerSkillCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionUnitCommandHandlerBase<RemoveWorkerSkillCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(RemoveWorkerSkillCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            worker.RemoveSkill(command.SkillId);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateWorkerAsync(workerId, cancellationToken);
            return Unit.Value;
        }
    }
}
