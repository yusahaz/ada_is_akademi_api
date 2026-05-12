namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Worker self: kısa “hakkında” metni (boş/geçiş = temizlenir).
    /// </summary>
    public sealed class UpdateWorkerBioCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Opsiyonel biyografi metni (trimlenecek).
        /// </summary>
        public string? Bio { get; set; }

        #endregion Properties
    }

    internal sealed class UpdateWorkerBioCommandValidator :
        IRequestValidator<UpdateWorkerBioCommand>
    {
        private const int MaxBioChars = 3000;

        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(UpdateWorkerBioCommand request)
        {
            List<ValidationFailure> failures = [];

            if (!request.Bio.IsNullOrWhiteSpace() &&
                request.Bio.Trim().Length > MaxBioChars)
            {
                failures.Add(
                    ApplicationValidationCodes.UpdateWorkerBioMaxLength.ForField(nameof(UpdateWorkerBioCommand.Bio)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class UpdateWorkerBioCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<UpdateWorkerBioCommand>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            UpdateWorkerBioCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            worker.UpdateBio(command.Bio);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Methods
    }
}
