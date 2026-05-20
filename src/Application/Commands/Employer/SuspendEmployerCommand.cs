namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using System;

    /// <summary>
    /// Suspends an employer that is not banned.
    /// </summary>
    public class SuspendEmployerCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the employer to suspend.
        /// </summary>
        public int EmployerId { get; set; }
        #endregion Properties
    }

    internal class SuspendEmployerCommandValidator : IRequestValidator<SuspendEmployerCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the employer identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(SuspendEmployerCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.SuspendEmployerEmployerId.ForField(nameof(request.EmployerId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class SuspendEmployerCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<SuspendEmployerCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the employer, suspends it, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(SuspendEmployerCommand command, CancellationToken cancellationToken)
        {
            Employer? entity = await UnitOfWork.GetRepository<Employer>().GetByIdAsync(command.EmployerId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.SetAsSuspended();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateEmployerReadModelsAsync(
                CacheService,
                command.EmployerId,
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
