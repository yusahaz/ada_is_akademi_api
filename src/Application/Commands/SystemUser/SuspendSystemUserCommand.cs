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
    /// Suspends a system user account.
    /// </summary>
    public class SuspendSystemUserCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the user to suspend.
        /// </summary>
        public int SystemUserId { get; set; }
        #endregion Properties
    }

    internal class SuspendSystemUserCommandValidator : IRequestValidator<SuspendSystemUserCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the user identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(SuspendSystemUserCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserId <= 0)
            {
                failures.Add(ApplicationValidationCodes.SuspendSystemUserSystemUserId.ForField(nameof(request.SystemUserId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class SuspendSystemUserCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<SuspendSystemUserCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the user, suspends the account, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(SuspendSystemUserCommand command, CancellationToken cancellationToken)
        {
            SystemUser? entity = await UnitOfWork.GetRepository<SystemUser>().GetByIdAsync(command.SystemUserId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.Suspend();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateSystemUserReadModelsAsync(
                CacheService,
                command.SystemUserId,
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
