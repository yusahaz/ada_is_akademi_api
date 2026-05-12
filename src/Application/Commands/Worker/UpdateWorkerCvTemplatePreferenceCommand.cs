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
    /// Persists authenticated worker CV options payload.
    /// </summary>
    public sealed class UpdateWorkerCvTemplatePreferenceCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Serialized CV options payload (template/layout/palette/version JSON).
        /// </summary>
        public string? CvOptions { get; set; }

        #endregion Properties
    }

    internal sealed class UpdateWorkerCvTemplatePreferenceCommandValidator :
        IRequestValidator<UpdateWorkerCvTemplatePreferenceCommand>
    {
        private const int CvOptionsPayloadMaxLength = 1024;

        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(UpdateWorkerCvTemplatePreferenceCommand request)
        {
            List<ValidationFailure> failures = [];
            if (!request.CvOptions.IsNullOrWhiteSpace() &&
                request.CvOptions.Trim().Length > CvOptionsPayloadMaxLength)
            {
                failures.Add(
                    AzoxiaErrorCodes.RequestValidationFailed.ForField(
                        nameof(UpdateWorkerCvTemplatePreferenceCommand.CvOptions)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class UpdateWorkerCvTemplatePreferenceCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<UpdateWorkerCvTemplatePreferenceCommand>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            UpdateWorkerCvTemplatePreferenceCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .GetByIdAsync(workerId, cancellationToken);
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            worker.UpdateCvOptions(command.CvOptions);
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        #endregion Methods
    }
}
