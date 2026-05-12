namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;

    public class RemoveWorkerCertificateCommand : CommandBase
    {
        public int CertificateId { get; set; }
    }

    internal class RemoveWorkerCertificateCommandValidator : IRequestValidator<RemoveWorkerCertificateCommand>
    {
        public ValidationResult Validate(RemoveWorkerCertificateCommand request)
            => request.CertificateId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.CertificateId))]);
    }

    internal class RemoveWorkerCertificateCommandHandler(IServiceProvider serviceProvider)
        : WorkerCollectionUnitCommandHandlerBase<RemoveWorkerCertificateCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(RemoveWorkerCertificateCommand command, CancellationToken cancellationToken)
        {
            (int workerId, Worker worker) = await GetActorWorkerAsync(cancellationToken);
            worker.RemoveCertificate(command.CertificateId);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
return Unit.Value;
        }
    }
}
