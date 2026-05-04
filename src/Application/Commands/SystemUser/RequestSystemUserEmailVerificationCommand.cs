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
    /// Stores an email verification token and its expiration for a user.
    /// </summary>
    public class RequestSystemUserEmailVerificationCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Absolute expiration instant for the verification token.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// Identifier of the user requesting verification.
        /// </summary>
        public int SystemUserId { get; set; }

        /// <summary>
        /// Hash of the verification token to persist.
        /// </summary>
        public string TokenHash { get; set; }
        #endregion Properties
    }

    internal class RequestSystemUserEmailVerificationCommandValidator : IRequestValidator<RequestSystemUserEmailVerificationCommand>
    {
        #region Methods

        /// <summary>
        /// Validates user identifier and token hash on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(RequestSystemUserEmailVerificationCommand request)
        {
            List<ValidationFailure> failures = [];
            DateTimeOffset now = DateTimeOffset.UtcNow;

            if (request.SystemUserId <= 0)
            {
                failures.Add(ApplicationValidationCodes.RequestSystemUserEmailVerificationSystemUserId.ForField(nameof(request.SystemUserId)));
            }

            if (string.IsNullOrWhiteSpace(request.TokenHash))
            {
                failures.Add(ApplicationValidationCodes.RequestSystemUserEmailVerificationTokenHash.ForField(nameof(request.TokenHash)));
            }

            if (request.ExpiresAt <= now)
            {
                failures.Add(ApplicationValidationCodes.RequestSystemUserEmailVerificationExpiresAtFuture.ForField(nameof(request.ExpiresAt)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class RequestSystemUserEmailVerificationCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<RequestSystemUserEmailVerificationCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the user, records verification material, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(RequestSystemUserEmailVerificationCommand command, CancellationToken cancellationToken)
        {
            SystemUser? entity = await UnitOfWork.GetRepository<SystemUser>().GetByIdAsync(command.SystemUserId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.RequestEmailVerification(command.TokenHash, command.ExpiresAt);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
