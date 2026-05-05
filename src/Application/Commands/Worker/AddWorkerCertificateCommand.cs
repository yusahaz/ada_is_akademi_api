namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    public class AddWorkerCertificateCommand : CommandBase<int>
    {
        public string? DocumentUrl { get; set; }
        public DateOnly? ExpiresAt { get; set; }
        public DateOnly IssuedAt { get; set; }
        public string IssuingOrganization { get; set; }
        public string Name { get; set; }
    }

    internal class AddWorkerCertificateCommandValidator : IRequestValidator<AddWorkerCertificateCommand>
    {
        public ValidationResult Validate(AddWorkerCertificateCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.Name.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.Name)));
            if (request.IssuingOrganization.IsNullOrWhiteSpace()) failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.IssuingOrganization)));
            return new ValidationResult(failures);
        }
    }

    internal class AddWorkerCertificateCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionCommandHandlerBase<AddWorkerCertificateCommand>(serviceProvider)
    {
        protected override async Task<int> HandleAsync(AddWorkerCertificateCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            WorkerCertificate item = worker.AddCertificate(command.Name, command.IssuingOrganization, command.IssuedAt, command.ExpiresAt, command.DocumentUrl);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateWorkerAsync(workerId, cancellationToken);
            return item.Id;
        }
    }
}
