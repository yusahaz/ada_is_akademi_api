namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Services;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Security.Claims;

    /// <summary>
    /// Rotates access/refresh tokens for a known device-bound refresh token.
    /// </summary>
    public class RefreshSystemUserTokenCommand :
        CommandBase<SystemUserTokenModel>
    {
        #region Properties

        /// <summary>
        /// Device identifier used when the refresh token was issued.
        /// </summary>
        public string DeviceIdentifier { get; set; }

        /// <summary>
        /// Refresh token value received from the client.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// User identifier owning the refresh token.
        /// </summary>
        public int SystemUserId { get; set; }

        #endregion Properties
    }

    internal class RefreshSystemUserTokenCommandValidator : IRequestValidator<RefreshSystemUserTokenCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RefreshSystemUserTokenCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.SystemUserId <= 0)
            {
                failures.Add(ApplicationValidationCodes.RefreshSystemUserTokenSystemUserId.ForField(nameof(request.SystemUserId)));
            }

            if (string.IsNullOrWhiteSpace(request.DeviceIdentifier))
            {
                failures.Add(ApplicationValidationCodes.RefreshSystemUserTokenDeviceIdentifierRequired.ForField(nameof(request.DeviceIdentifier)));
            }

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                failures.Add(ApplicationValidationCodes.RefreshSystemUserTokenRefreshTokenRequired.ForField(nameof(request.RefreshToken)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class RefreshSystemUserTokenCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RefreshSystemUserTokenCommand, SystemUserTokenModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<SystemUserTokenModel> HandleAsync(RefreshSystemUserTokenCommand command, CancellationToken cancellationToken)
        {
            SystemUser? user = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x => x.Id == command.SystemUserId)
                .Include(x => x.Devices)
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(cancellationToken);
            user = user.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            // Refresh is only allowed for active and non-locked accounts.
            (user.AccountStatus == AccountStatus.Active).ThrowIfFalse(AzoxiaErrorCodes.NotFound);
            (!user.IsLocked).ThrowIfFalse(AzoxiaErrorCodes.NotFound);

            SystemUserDevice? device = user.Devices
                .FirstOrDefault(x => x.DeviceIdentifier == command.DeviceIdentifier);
            device = device.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            SystemUserRefreshToken? existingRefreshToken = user.RefreshTokens
                .FirstOrDefault(x =>
                    x.DeviceId == device.Id &&
                    x.TokenHash == command.RefreshToken &&
                    x.IsActive);
            existingRefreshToken = existingRefreshToken.ThrowIfNull(AzoxiaErrorCodes.NotFound);

            ITokenService tokenService = ServiceProvider.GetRequiredService<ITokenService>();
            existingRefreshToken.Revoke();
            (string refreshToken, DateTime refreshExpiresAt) = tokenService.GenerateRefreshToken();
            user.IssueRefreshToken(refreshToken, device.Id, refreshExpiresAt);
            device.RecordActivity();

            Claim[] claims = await BuildClaimsAsync(user, cancellationToken);
            (string accessToken, DateTime accessExpiresAt) = tokenService.GenerateAccessToken(claims);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return new SystemUserTokenModel(
                user.Id,
                (int)user.Type,
                accessToken,
                accessExpiresAt,
                refreshToken,
                refreshExpiresAt);
        }

        private async Task<Claim[]> BuildClaimsAsync(SystemUser user, CancellationToken cancellationToken)
        {
            List<Claim> claims =
            [
                new("system_user_id", user.Id.ToString()),
                new("system_user_type", ((int)user.Type).ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
            ];

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => x.SystemUserId == user.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (worker is not null)
            {
                claims.Add(new Claim("worker_id", worker.Id.ToString()));
            }

            ShiftSupervisor? supervisor = await UnitOfWork
                .GetRepository<ShiftSupervisor>()
                .Filter(x => x.SystemUserId == user.Id && x.IsActive)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            if (supervisor is not null)
            {
                claims.Add(new Claim("employer_id", supervisor.EmployerId.ToString()));
            }

            return [.. claims];
        }

        #endregion Utils
    }
}
