namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Services;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Security.Claims;

    /// <summary>
    /// Authenticates a system user and returns access/refresh tokens.
    /// </summary>
    public class LoginSystemUserCommand :
        CommandBase<SystemUserTokenModel>
    {
        #region Properties

        /// <summary>
        /// Unique device identifier used for refresh token scoping.
        /// </summary>
        public string DeviceIdentifier { get; set; }

        /// <summary>
        /// Optional platform push token.
        /// </summary>
        public string? DeviceToken { get; set; }

        /// <summary>
        /// User email used for sign-in.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password text used for sign-in.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Declares which audience panel is attempting login (Admin/Employer/Worker).
        /// </summary>
        public SystemUserType SystemUserType { get; set; }

        /// <summary>
        /// Client platform for device registration.
        /// </summary>
        public DevicePlatform Platform { get; set; }

        #endregion Properties
    }

    internal class LoginSystemUserCommandValidator : IRequestValidator<LoginSystemUserCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(LoginSystemUserCommand request)
        {
            List<ValidationFailure> failures = [];

            if (request.Email.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.LoginSystemUserEmailRequired.ForField(nameof(request.Email)));
            }

            if (request.Password.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.LoginSystemUserPasswordRequired.ForField(nameof(request.Password)));
            }

            if (request.DeviceIdentifier.IsNullOrWhiteSpace())
            {
                failures.Add(ApplicationValidationCodes.LoginSystemUserDeviceIdentifierRequired.ForField(nameof(request.DeviceIdentifier)));
            }

            bool isKnownType = request.SystemUserType is SystemUserType.Admin
                or SystemUserType.Employer
                or SystemUserType.Worker;
            if (!isKnownType)
            {
                failures.Add(ApplicationValidationCodes.LoginSystemUserTypeRequired.ForField(nameof(request.SystemUserType)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class LoginSystemUserCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<LoginSystemUserCommand, SystemUserTokenModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<SystemUserTokenModel> HandleAsync(LoginSystemUserCommand command, CancellationToken cancellationToken)
        {
            ITokenService tokenService = ServiceProvider.GetRequiredService<ITokenService>();

            Logger.LogInformation(
                "Login request received. Email={Email}, SystemUserType={SystemUserType}, Platform={Platform}, DeviceIdentifier={DeviceIdentifier}, HasDeviceToken={HasDeviceToken}.",
                command.Email,
                (int)command.SystemUserType,
                (int)command.Platform,
                command.DeviceIdentifier,
                !command.DeviceToken.IsNullOrWhiteSpace());

            SystemUser? user = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x =>
                    x.Email == command.Email
                    && (command.SystemUserType == SystemUserType.Admin || x.Type == command.SystemUserType))
                .AsSplitQuery()
                .Include(x => x.Devices)
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
            {
                Logger.LogWarning(
                    "Login user lookup failed. Email={Email}, SystemUserType={SystemUserType}.",
                    command.Email,
                    (int)command.SystemUserType);

                ApplicationValidationCodes.LoginSystemUserAuthenticationFailed.Throw();
            }

            // Login is only allowed for active and non-locked accounts.
            if (user.AccountStatus != AccountStatus.Active || user.IsLocked)
            {
                ApplicationValidationCodes.LoginSystemUserAuthenticationFailed.Throw();
            }

            bool isPasswordValid = await user.CheckPassword(command.Password);
            if (!isPasswordValid)
            {
                user.RecordFailedLoginAttempt();
                await UnitOfWork.SaveChangesAsync(cancellationToken);
                ApplicationValidationCodes.LoginSystemUserAuthenticationFailed.Throw();
            }

            user.RecordSuccessfulLogin();
            SystemUserDevice device = user.AddOrUpdateDevice(command.DeviceIdentifier, command.Platform, command.DeviceToken);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            Claim[] claims = await BuildClaimsAsync(user, cancellationToken);
            (string accessToken, DateTime accessExpiresAt) = tokenService.GenerateAccessToken(claims);
            (string refreshToken, DateTime refreshExpiresAt) = tokenService.GenerateRefreshToken();

            user.IssueRefreshToken(refreshToken, device.Id, refreshExpiresAt);
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
