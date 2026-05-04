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
    /// Revokes an active refresh token for a known user and device pairing.
    /// </summary>
    public class LogoutSystemUserCommand :
        CommandBase
    {
        #region Properties

        /// <summary>
        /// Device identifier used when the refresh token was issued.
        /// </summary>
        public string DeviceIdentifier { get; set; }

        /// <summary>
        /// Refresh token value that should be revoked.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// User identifier owning the refresh token.
        /// </summary>
        public int SystemUserId { get; set; }

        #endregion Properties
    }

    internal class LogoutSystemUserCommandValidator : IRequestValidator<LogoutSystemUserCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(LogoutSystemUserCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserId <= 0)
            {
                failures.Add(ApplicationValidationCodes.LogoutSystemUserSystemUserId.ForField(nameof(LogoutSystemUserCommand.SystemUserId)));
            }

            if (request.DeviceIdentifier.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.LogoutSystemUserDeviceIdentifierRequired.ForField(nameof(LogoutSystemUserCommand.DeviceIdentifier)));
            }

            if (request.RefreshToken.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.LogoutSystemUserRefreshTokenRequired.ForField(nameof(LogoutSystemUserCommand.RefreshToken)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class LogoutSystemUserCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<LogoutSystemUserCommand>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<Unit> HandleAsync(LogoutSystemUserCommand command, CancellationToken cancellationToken)
        {
            SystemUser? user = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x => x.Id == command.SystemUserId)
                .Include(x => x.Devices)
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(cancellationToken);
            user = user.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            SystemUserDevice? device = user.Devices
                .FirstOrDefault(x => x.DeviceIdentifier == command.DeviceIdentifier);
            device = device.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            SystemUserRefreshToken? token = user.RefreshTokens
                .FirstOrDefault(x =>
                    x.DeviceId == device.Id &&
                    x.TokenHash == command.RefreshToken &&
                    x.IsActive);
            token = token.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            token.Revoke();
            device.RecordActivity();

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        #endregion Utils
    }
}
