namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Services;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
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
        /// Legacy compatibility field; ignored by handler and kept for contract backward compatibility.
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
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            SystemUserRefreshToken? existingRefreshToken = await UnitOfWork
                .GetRepository<SystemUserRefreshToken>()
                .Filter(x => x.TokenHash == command.RefreshToken && !x.IsRevoked)
                .Include(x => x.Device)
                .FirstOrDefaultAsync(cancellationToken);
            if (existingRefreshToken is null ||
                existingRefreshToken.ExpiresAt <= DateTimeOffset.UtcNow ||
                existingRefreshToken.Device.DeviceIdentifier != command.DeviceIdentifier)
            {
                ApplicationValidationCodes.RefreshSystemUserTokenAuthenticationFailed.Throw();
            }

            SystemUser? user = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x => x.Id == existingRefreshToken.SystemUserId)
                .AsSplitQuery()
                .Include(x => x.Devices)
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
            {
                ApplicationValidationCodes.RefreshSystemUserTokenAuthenticationFailed.Throw();
            }

            // Refresh is only allowed for active and non-locked accounts.
            if (user.AccountStatus != AccountStatus.Active || user.IsLocked)
            {
                ApplicationValidationCodes.RefreshSystemUserTokenAuthenticationFailed.Throw();
            }

            SystemUserDevice? device = user.Devices
                .FirstOrDefault(x => x.DeviceIdentifier == command.DeviceIdentifier);
            if (device is null)
            {
                ApplicationValidationCodes.RefreshSystemUserTokenAuthenticationFailed.Throw();
            }

            SystemUserRefreshToken? activeToken = user.RefreshTokens
                .FirstOrDefault(x =>
                    x.DeviceId == device.Id &&
                    x.TokenHash == command.RefreshToken &&
                    x.IsActive);
            if (activeToken is null)
            {
                ApplicationValidationCodes.RefreshSystemUserTokenAuthenticationFailed.Throw();
            }

            string? claimedSystemUserId = executionContext.GetClaim("system_user_id");
            if (int.TryParse(claimedSystemUserId, out int actorSystemUserId) &&
                actorSystemUserId > 0 &&
                actorSystemUserId != user.Id)
            {
                ApplicationValidationCodes.RefreshSystemUserTokenAuthenticationFailed.Throw();
            }

            ITokenService tokenService = ServiceProvider.GetRequiredService<ITokenService>();
            activeToken.Revoke();
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
