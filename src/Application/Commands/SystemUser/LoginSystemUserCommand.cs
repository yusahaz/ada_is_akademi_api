namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Services;
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
                or SystemUserType.Supervisor
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
            string normalizedEmail = SystemUserEmailNormalizer.Normalize(command.Email);

            Logger.LogInformation(
                "Login request received. Email={Email}, EmailLength={EmailLength}, TrimmedEmailLength={TrimmedEmailLength}, SystemUserType={SystemUserType}, Platform={Platform}, DeviceIdentifier={DeviceIdentifier}, HasDeviceToken={HasDeviceToken}.",
                command.Email,
                command.Email.Length,
                normalizedEmail.Length,
                (int)command.SystemUserType,
                (int)command.Platform,
                command.DeviceIdentifier,
                !command.DeviceToken.IsNullOrWhiteSpace());

            SystemUser? user = await UnitOfWork
                .GetRepository<SystemUser>()
                .Filter(x =>
                    x.Email == normalizedEmail
                    && (command.SystemUserType == SystemUserType.Admin || x.Type == command.SystemUserType))
                .AsSplitQuery()
                .Include(x => x.Devices)
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is not null)
            {
                Logger.LogInformation(
                    "Login user lookup succeeded. Id={Id}, Email={Email}, Type={Type}, AccountStatus={AccountStatus}, IsLocked={IsLocked}, IsDeleted={IsDeleted}, CreatedAt={CreatedAt}, CreatedBy={CreatedBy}, UpdatedAt={UpdatedAt}, UpdatedBy={UpdatedBy}, DeletedAt={DeletedAt}, DeletedBy={DeletedBy}, EmailVerificationExpiresAt={EmailVerificationExpiresAt}, HasEmailVerificationToken={HasEmailVerificationToken}, EmailVerifiedAt={EmailVerifiedAt}, FailedLoginAttempts={FailedLoginAttempts}, FirstName={FirstName}, LastFailedLoginAt={LastFailedLoginAt}, LastName={LastName}, LastPasswordChangeAt={LastPasswordChangeAt}, LastSuccessfulLoginAt={LastSuccessfulLoginAt}, HasPasswordHash={HasPasswordHash}, HasPasswordSalt={HasPasswordSalt}, Phone={Phone}.",
                    user.Id,
                    user.Email,
                    (int)user.Type,
                    (int)user.AccountStatus,
                    user.IsLocked,
                    user.IsDeleted,
                    user.CreatedAt,
                    user.CreatedBy,
                    user.UpdatedAt,
                    user.UpdatedBy,
                    user.DeletedAt,
                    user.DeletedBy,
                    user.EmailVerificationExpiresAt,
                    !user.EmailVerificationToken.IsNullOrWhiteSpace(),
                    user.EmailVerifiedAt,
                    user.FailedLoginAttempts,
                    user.FirstName,
                    user.LastFailedLoginAt,
                    user.LastName,
                    user.LastPasswordChangeAt,
                    user.LastSuccessfulLoginAt,
                    !user.PasswordHash.IsNullOrWhiteSpace(),
                    !user.PasswordSalt.IsNullOrWhiteSpace(),
                    user.Phone);
            }

            if (user is null)
            {
                Logger.LogWarning(
                    "Login user lookup failed. Email={Email}, EmailLength={EmailLength}, TrimmedEmailLength={TrimmedEmailLength}, SystemUserType={SystemUserType}.",
                    command.Email,
                    command.Email.Length,
                    normalizedEmail.Length,
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

            Claim[] claims = await SystemUserClaimsBuilder.BuildAsync(user, UnitOfWork, cancellationToken);
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

        #endregion Utils
    }
}
