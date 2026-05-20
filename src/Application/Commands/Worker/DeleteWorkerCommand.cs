namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;

    public class DeleteWorkerCommand : CommandBase
    {
        public int WorkerId { get; set; }
    }

    internal class DeleteWorkerCommandValidator : IRequestValidator<DeleteWorkerCommand>
    {
        public ValidationResult Validate(DeleteWorkerCommand request)
            => request.WorkerId > 0
                ? new ValidationResult([])
                : new ValidationResult([AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(request.WorkerId))]);
    }

    internal class DeleteWorkerCommandHandler(IServiceProvider serviceProvider)
        : CommandHandlerBase<DeleteWorkerCommand>(serviceProvider)
    {
        protected override async Task<Unit> HandleAsync(DeleteWorkerCommand command, CancellationToken cancellationToken)
        {
            Worker? worker = await UnitOfWork.GetRepository<Worker>().GetByIdAsync(command.WorkerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            SystemUser? user = await UnitOfWork.GetRepository<SystemUser>().GetByIdAsync(worker.SystemUserId, cancellationToken);
            user = user.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            worker.DeleteWorker();
            user.DeleteSystemUser();

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await AdaIsReadModelCacheInvalidation.InvalidateDeletedWorkerReadModelsAsync(
                CacheService,
                worker.Id,
                cancellationToken);

            return Unit.Value;
        }
    }
}
