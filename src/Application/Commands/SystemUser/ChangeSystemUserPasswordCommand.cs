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
    /// Rotates a system user's password and revokes refresh tokens.
    /// </summary>
    public class ChangeSystemUserPasswordCommand : CommandBase
    {
        #region Properties
        /// <summary>
        /// New password text supplied by the caller.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Identifier of the user whose password changes.
        /// </summary>
        public int SystemUserId { get; set; }
        #endregion Properties
    }

    internal class ChangeSystemUserPasswordCommandValidator : IRequestValidator<ChangeSystemUserPasswordCommand>
    {
        #region Methods

        /// <summary>
        /// Validates user identifier and password text on the command.
        /// </summary>
        /// <param name="request">Command instance to validate.</param>
        /// <returns>Aggregated validation failures, if any.</returns>
        public ValidationResult Validate(ChangeSystemUserPasswordCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserId <= 0)
            {
                failures.Add(ApplicationValidationCodes.ChangeSystemUserPasswordSystemUserId.ForField(nameof(request.SystemUserId)));
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                failures.Add(ApplicationValidationCodes.ChangeSystemUserPasswordPasswordRequired.ForField(nameof(request.Password)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ChangeSystemUserPasswordCommandHandler(IServiceProvider serviceProvider) : CommandHandlerBase<ChangeSystemUserPasswordCommand>(serviceProvider)
    {
        #region Utils

        /// <summary>
        /// Loads the user, changes the password, and persists changes.
        /// </summary>
        /// <param name="command">Command payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completion marker.</returns>
        protected override async Task<Unit> HandleAsync(ChangeSystemUserPasswordCommand command, CancellationToken cancellationToken)
        {
            SystemUser? entity = await UnitOfWork.GetRepository<SystemUser>().GetByIdAsync(command.SystemUserId, cancellationToken);
            entity = entity.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            entity.ChangePassword(command.Password);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
