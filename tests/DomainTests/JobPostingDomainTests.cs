namespace Azoxia.AdaIsAkademi.Domain.Tests;

using Azoxia.AdaIsAkademi.Domain;
using Azoxia.Core.Exceptions;
using Azoxia.Core.ValueTypes;
using FluentAssertions;

public class JobPostingDomainTests
{
    [Fact]
    public void Cancel_ShouldThrow_WhenStatusDoesNotAllowTransition()
    {
        // Arrange
        JobPosting posting = CreateDraftPosting();
        posting.Publish();
        posting.Complete();

        // Act
        Action act = () => posting.Cancel();

        // Assert
        AzoxiaException exception = act.Should().Throw<AzoxiaException>().Which;
        exception.Error.Should().Be(DomainErrorCodes.JobPostingInvalidStatusTransition);
    }

    [Fact]
    public void AddApplication_SecondCallForSameWorker_ReturnsSameInstance()
    {
        JobPosting posting = CreateOpenPostingForTomorrow();
        JobApplication first = posting.AddApplication(workerId: 42, hasConflictingShift: false);
        JobApplication second = posting.AddApplication(workerId: 42, hasConflictingShift: false);

        ReferenceEquals(first, second).Should().BeTrue();
        posting.Applications.Should().HaveCount(1);
    }

    [Fact]
    public void AddSkill_ShouldNormalizeAndPreventDuplicates_CaseInsensitive()
    {
        // Arrange
        JobPosting posting = CreateDraftPosting();
        posting.Publish();

        // Act
        JobPostingSkill first = posting.AddSkill("  CSharp  ", isRequired: true);
        JobPostingSkill second = posting.AddSkill("csharp", isRequired: false);

        // Assert
        first.Id.Should().Be(second.Id);
        posting.Skills.Should().HaveCount(1);
        ((string)first.Tag).Should().Be("CSHARP");
    }

    private static JobPosting CreateDraftPosting()
        => new(
            employerId: 1,
            employerLocationId: 1,
            jobCategoryId: 1,
            title: "Kasiyer",
            description: "Vardiya",
            shiftDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            shiftStartTime: new TimeOnly(9, 0),
            shiftEndTime: new TimeOnly(18, 0),
            wage: new Money(100m, "TRY"),
            headCount: 2);

    private static JobPosting CreateOpenPostingForTomorrow()
    {
        JobPosting posting = CreateDraftPosting();
        posting.Publish();
        return posting;
    }
}
