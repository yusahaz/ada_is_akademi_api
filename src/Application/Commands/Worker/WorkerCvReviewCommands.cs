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
    /// Worker self: confirms extracted CV payload for one session.
    /// </summary>
    public sealed class ConfirmWorkerCvReviewCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Target upload session identifier.
        /// </summary>
        public int CvUploadSessionId { get; set; }

        #endregion Properties
    }

    internal sealed class ConfirmWorkerCvReviewCommandValidator :
        IRequestValidator<ConfirmWorkerCvReviewCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ConfirmWorkerCvReviewCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.CvUploadSessionId <= 0)
            {
                failures.Add(ApplicationValidationCodes.WorkerCvUploadSessionIdRequired.ForField(nameof(ConfirmWorkerCvReviewCommand.CvUploadSessionId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class ConfirmWorkerCvReviewCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<ConfirmWorkerCvReviewCommand>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            ConfirmWorkerCvReviewCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CvUploadSession? session = await UnitOfWork
                .GetRepository<CvUploadSession>()
                .Filter(x => x.Id == command.CvUploadSessionId && x.WorkerId == workerId)
                .FirstOrDefaultAsync(cancellationToken);
            session = session.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            session.Confirm();
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        #endregion Methods
    }

    /// <summary>
    /// Worker self: discards extracted CV payload for one session.
    /// </summary>
    public sealed class DiscardWorkerCvReviewCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Target upload session identifier.
        /// </summary>
        public int CvUploadSessionId { get; set; }

        #endregion Properties
    }

    internal sealed class DiscardWorkerCvReviewCommandValidator :
        IRequestValidator<DiscardWorkerCvReviewCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(DiscardWorkerCvReviewCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.CvUploadSessionId <= 0)
            {
                failures.Add(ApplicationValidationCodes.WorkerCvUploadSessionIdRequired.ForField(nameof(DiscardWorkerCvReviewCommand.CvUploadSessionId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal sealed class DiscardWorkerCvReviewCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<DiscardWorkerCvReviewCommand>(serviceProvider)
    {
        #region Methods

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(
            DiscardWorkerCvReviewCommand command,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CvUploadSession? session = await UnitOfWork
                .GetRepository<CvUploadSession>()
                .Filter(x => x.Id == command.CvUploadSessionId && x.WorkerId == workerId)
                .FirstOrDefaultAsync(cancellationToken);
            session = session.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            session.Discard();
            await UnitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        #endregion Methods
    }
}
