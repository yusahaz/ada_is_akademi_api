namespace Azoxia.AdaIsAkademi.Domain.Tests;

using Azoxia.AdaIsAkademi.Domain;
using Azoxia.Core.Exceptions;
using FluentAssertions;

public class SystemUserDomainTests
{
    [Fact]
    public void Reactivate_ShouldThrow_WhenUserIsBanned()
    {
        // Arrange
        SystemUser user = new("user@test.com", "123456", SystemUserType.Worker);
        user.Ban();

        // Act
        Action act = () => user.Reactivate();

        // Assert
        AzoxiaException exception = act.Should().Throw<AzoxiaException>().Which;
        exception.Error.Should().Be(DomainErrorCodes.SystemUserInvalidStatusTransition);
    }

    [Fact]
    public void VerifyEmail_ShouldThrow_WhenTokenIsInvalid()
    {
        // Arrange
        SystemUser user = new("user@test.com", "123456", SystemUserType.Worker);
        user.RequestEmailVerification("valid-token", DateTimeOffset.UtcNow.AddMinutes(5));

        // Act
        Action act = () => user.VerifyEmail("invalid-token");

        // Assert
        AzoxiaException exception = act.Should().Throw<AzoxiaException>().Which;
        exception.Error.Should().Be(DomainErrorCodes.SystemUserEmailVerificationInvalid);
    }

    [Fact]
    public void VerifyEmail_ShouldActivate_WhenTokenIsValid()
    {
        // Arrange
        SystemUser user = new("user@test.com", "123456", SystemUserType.Worker);
        string token = "valid-token";
        user.RequestEmailVerification(token, DateTimeOffset.UtcNow.AddMinutes(5));

        // Act
        user.VerifyEmail(token);

        // Assert
        user.EmailVerifiedAt.Should().NotBeNull();
        user.AccountStatus.Should().Be(AccountStatus.Active);
        user.EmailVerificationToken.Should().BeNull();
        user.EmailVerificationExpiresAt.Should().BeNull();
    }
}
