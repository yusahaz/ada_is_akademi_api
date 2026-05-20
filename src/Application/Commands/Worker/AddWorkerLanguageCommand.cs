namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    public class AddWorkerLanguageCommand : CommandBase<int>
    {
        public string Language { get; set; }
        public LanguageLevel Level { get; set; }
    }

    internal class AddWorkerLanguageCommandValidator : IRequestValidator<AddWorkerLanguageCommand>
    {
        public ValidationResult Validate(AddWorkerLanguageCommand request)
            => request.Language.IsNullOrWhiteSpace()
                ? new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.Language))])
                : new ValidationResult([]);
    }

    internal class AddWorkerLanguageCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionCommandHandlerBase<AddWorkerLanguageCommand>(serviceProvider)
    {
        protected override async Task<int> HandleAsync(AddWorkerLanguageCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            WorkerLanguage item = worker.AddLanguage(command.Language, command.Level);
            await SaveWorkerChangesAndInvalidateReadModelsAsync(workerId, cancellationToken);
            return item.Id;
        }
    }
}
