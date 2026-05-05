namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;

    public class RemoveWorkerLanguageCommand : CommandBase
    {
        public int LanguageId { get; set; }
    }

    internal class RemoveWorkerLanguageCommandValidator : IRequestValidator<RemoveWorkerLanguageCommand>
    {
        public ValidationResult Validate(RemoveWorkerLanguageCommand request)
            => request.LanguageId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.LanguageId))]);
    }

    internal class RemoveWorkerLanguageCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionUnitCommandHandlerBase<RemoveWorkerLanguageCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(RemoveWorkerLanguageCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            worker.RemoveLanguage(command.LanguageId);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateWorkerAsync(workerId, cancellationToken);
            return Unit.Value;
        }
    }
}
