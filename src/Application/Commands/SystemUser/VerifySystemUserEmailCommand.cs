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
    /// Confirms a user's email using a previously issued verification token hash.
    /// </summary>
    public class VerifySystemUserEmailCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// Identifier of the user completing verification.
        /// </summary>
        public int SystemUserId { get; set; }

        /// <summary>
        /// Verification token hash presented by the caller.
        /// </summary>
        public string TokenHash { get; set; }
        #endregion Properties
    }

    internal class VerifySystemUserEmailCommandValidator : IRequestValidator<VerifySystemUserEmailCommand>
    {
        #region Methods

        /// <summary>
        /// Validates user identifier and token hash on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(VerifySystemUserEmailCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserId <= 0)
            {
                failures.Add(ApplicationValidationCodes.VerifySystemUserEmailSystemUserId.ForField(nameof(request.SystemUserId)));
            }

            if (string.IsNullOrWhiteSpace(request.TokenHash))
            {
                failures.Add(ApplicationValidationCodes.VerifySystemUserEmailTokenHash.ForField(nameof(request.TokenHash)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class VerifySystemUserEmailCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<VerifySystemUserEmailCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the user, verifies the email, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(VerifySystemUserEmailCommand command, CancellationToken cancellationToken)
        {
            SystemUser? entity = await UnitOfWork.GetRepository<SystemUser>().GetByIdAsync(command.SystemUserId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.VerifyEmail(command.TokenHash);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
