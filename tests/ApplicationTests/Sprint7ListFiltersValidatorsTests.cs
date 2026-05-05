namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Application.Validation;
    using FluentAssertions;

    /// <summary>
    /// Sprint 7 validator tests for new filtered list queries.
    /// </summary>
    public sealed class Sprint7ListFiltersValidatorsTests
    {
        [Fact]
        public void List_employers_validator_should_fail_for_invalid_range_and_paging()
        {
            ValidationResult result = new ListEmployersQueryValidator().Validate(
                new ListEmployersQuery { Limit = 0, Offset = -1, CommissionRateMin = 0.8m, CommissionRateMax = 0.2m });
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(3);
        }

        [Fact]
        public void List_workers_validator_should_fail_for_invalid_paging()
        {
            ValidationResult result = new ListWorkersQueryValidator().Validate(
                new ListWorkersQuery { Limit = 0, Offset = -1 });
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void List_system_users_validator_should_fail_for_invalid_paging()
        {
            ValidationResult result = new ListSystemUsersQueryValidator().Validate(
                new ListSystemUsersQuery { Limit = 0, Offset = -1 });
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void List_system_user_groups_validator_should_fail_for_invalid_paging()
        {
            ValidationResult result = new ListSystemUserGroupsQueryValidator().Validate(
                new ListSystemUserGroupsQuery { Limit = 0, Offset = -1 });
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }
    }
}
