namespace Azoxia.AdaIsAkademi.Domain.Tests;

using Azoxia.AdaIsAkademi.Domain;
using Azoxia.Core.Exceptions;
using Azoxia.Core.ValueTypes;
using FluentAssertions;

public class EmployerDomainTests
{
    [Fact]
    public void SetAsActive_ShouldThrow_WhenEmployerIsBanned()
    {
        // Arrange
        Employer employer = new("ACME", "Test", " 1234567890 ");
        employer.SetAsBanned();

        // Act
        Action act = () => employer.SetAsActive();

        // Assert
        AzoxiaException exception = act.Should().Throw<AzoxiaException>().Which;
        exception.Error.Should().Be(DomainErrorCodes.EmployerInvalidStatusTransition);
    }

    [Fact]
    public void Constructor_ShouldNormalizeTaxNumber()
    {
        // Arrange
        Employer employer = new("ACME", null, " 1234567890 ");

        // Assert
        ((string)employer.TaxNumber).Should().Be("1234567890");
    }

    [Fact]
    public void AddJobPosting_ShouldThrow_WhenEmployerIsNotActive()
    {
        Employer employer = new("ACME", null, "1234567890");
        employer.AddLocation("Main");

        Action act = () => employer.AddJobPosting(
            employerLocationId: 0,
            jobCategoryId: 1,
            title: "Kasiyer",
            description: "Vardiya",
            shiftDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            shiftStartTime: new TimeOnly(9, 0),
            shiftEndTime: new TimeOnly(18, 0),
            wage: new Money(100m, "TRY"),
            headCount: 2);

        AzoxiaException exception = act.Should().Throw<AzoxiaException>().Which;
        exception.Error.Should().Be(DomainErrorCodes.EmployerCannotCreateJobPosting);
    }

    [Fact]
    public void AddJobPosting_ShouldThrow_WhenLocationDoesNotBelongToEmployer()
    {
        Employer employer = new("ACME", null, "1234567890");
        employer.SetAsActive();

        Action act = () => employer.AddJobPosting(
            employerLocationId: 999,
            jobCategoryId: 1,
            title: "Kasiyer",
            description: "Vardiya",
            shiftDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            shiftStartTime: new TimeOnly(9, 0),
            shiftEndTime: new TimeOnly(18, 0),
            wage: new Money(100m, "TRY"),
            headCount: 2);

        AzoxiaException exception = act.Should().Throw<AzoxiaException>().Which;
        exception.Error.Should().Be(DomainErrorCodes.EmployerLocationNotFound);
    }

    [Fact]
    public void AddJobPosting_ShouldThrow_WhenShiftEndIsNotAfterStart()
    {
        Employer employer = new("ACME", null, "1234567890");
        employer.SetAsActive();
        EmployerLocation location = employer.AddLocation("Main");

        Action act = () => employer.AddJobPosting(
            employerLocationId: location.Id,
            jobCategoryId: 1,
            title: "Kasiyer",
            description: "Vardiya",
            shiftDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            shiftStartTime: new TimeOnly(18, 0),
            shiftEndTime: new TimeOnly(9, 0),
            wage: new Money(100m, "TRY"),
            headCount: 2);

        AzoxiaException exception = act.Should().Throw<AzoxiaException>().Which;
        exception.Error.Should().Be(DomainErrorCodes.JobPostingInvalidShiftTimes);
    }

    [Fact]
    public void AddJobPosting_ShouldCreateDraft_WhenEmployerActiveAndLocationValid()
    {
        Employer employer = new("ACME", null, "1234567890");
        employer.SetAsActive();
        EmployerLocation location = employer.AddLocation("Main");

        JobPosting posting = employer.AddJobPosting(
            employerLocationId: location.Id,
            jobCategoryId: 1,
            title: "Kasiyer",
            description: "Vardiya",
            shiftDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            shiftStartTime: new TimeOnly(9, 0),
            shiftEndTime: new TimeOnly(18, 0),
            wage: new Money(100m, "TRY"),
            headCount: 2);

        posting.Status.Should().Be(JobPostingStatus.Draft);
        posting.EmployerId.Should().Be(employer.Id);
        posting.EmployerLocationId.Should().Be(location.Id);
        employer.JobPostings.Should().ContainSingle().Which.Should().BeSameAs(posting);
    }
}
