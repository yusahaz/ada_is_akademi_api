namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Application.Validation;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// Validator rules for identity-related system user commands.
    /// </summary>
    public sealed class SystemUserIdentityValidatorsTests
    {
        #region Methods

        [Fact]
        public void Request_email_verification_requires_future_expiration()
        {
            var validator = new RequestSystemUserEmailVerificationCommandValidator();
            ValidationResult result = validator.Validate(
                new RequestSystemUserEmailVerificationCommand
                {
                    SystemUserId = 1,
                    TokenHash = "token-hash",
                    ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-1),
                });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.Field == nameof(RequestSystemUserEmailVerificationCommand.ExpiresAt));
        }

        [Fact]
        public void Refresh_token_requires_device_identifier_and_token()
        {
            var validator = new RefreshSystemUserTokenCommandValidator();
            ValidationResult result = validator.Validate(new RefreshSystemUserTokenCommand());

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain(x => x.Field == nameof(RefreshSystemUserTokenCommand.DeviceIdentifier));
            result.Errors.Should().Contain(x => x.Field == nameof(RefreshSystemUserTokenCommand.RefreshToken));
        }

        [Fact]
        public void Logout_requires_device_identifier_and_token()
        {
            var validator = new LogoutSystemUserCommandValidator();
            ValidationResult result = validator.Validate(new LogoutSystemUserCommand());

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain(x => x.Field == nameof(LogoutSystemUserCommand.DeviceIdentifier));
            result.Errors.Should().Contain(x => x.Field == nameof(LogoutSystemUserCommand.RefreshToken));
        }

        #endregion Methods
    }
}
