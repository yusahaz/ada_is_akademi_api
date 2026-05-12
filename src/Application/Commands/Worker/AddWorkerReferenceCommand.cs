namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.ValueTypes;

    public class AddWorkerReferenceCommand : CommandBase<int>
    {
        public string Company { get; set; }
        public string ContactEmail { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string? ContactPhone { get; set; }
        public string Position { get; set; }
    }

    internal class AddWorkerReferenceCommandValidator : IRequestValidator<AddWorkerReferenceCommand>
    {
        public ValidationResult Validate(AddWorkerReferenceCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.Company.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.Company)));
            if (request.Position.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.Position)));
            if (request.ContactFirstName.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.ContactFirstName)));
            if (request.ContactLastName.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.ContactLastName)));
            if (request.ContactEmail.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.ContactEmail)));
            return new ValidationResult(failures);
        }
    }

    internal class AddWorkerReferenceCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionCommandHandlerBase<AddWorkerReferenceCommand>(serviceProvider)
    {
        protected override async Task<int> HandleAsync(AddWorkerReferenceCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            Contact contact = new(command.ContactFirstName, command.ContactLastName, command.ContactEmail, command.ContactPhone);
            WorkerReference item = worker.AddReference(command.Company, command.Position, contact);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
return item.Id;
        }
    }
}
