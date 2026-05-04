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
    /// Reactivates a previously non-banned system user account.
    /// </summary>
    public class ReactivateSystemUserCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the user to reactivate.
        /// </summary>
        public int SystemUserId { get; set; }
        #endregion Properties
    }

    internal class ReactivateSystemUserCommandValidator : IRequestValidator<ReactivateSystemUserCommand>
    {
        #region Methods

        /// <summary>
        /// Ensures the user identifier is positive.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(ReactivateSystemUserCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserId <= 0)
            {
                failures.Add(ApplicationValidationCodes.ReactivateSystemUserSystemUserId.ForField(nameof(request.SystemUserId)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ReactivateSystemUserCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<ReactivateSystemUserCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the user, reactivates the account, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(ReactivateSystemUserCommand command, CancellationToken cancellationToken)
        {
            SystemUser? entity = await UnitOfWork.GetRepository<SystemUser>().GetByIdAsync(command.SystemUserId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.Reactivate();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await CacheService.InvalidateByDependencyAsync(
                AdaIsCacheKeys.SystemUserAllDependency(),
                cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
