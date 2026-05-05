namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Application.Validation;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// Validator rules for job application operations where actor identity can come from <c>IExecutionContext</c> claims.
    /// </summary>
    public sealed class JobPostingApplicationValidatorsTests
    {
        #region Methods

        [Fact]
        public void List_postings_by_employer_validator_accepts_payload_without_actor_fields()
        {
            var validator = new ListJobPostingsByEmployerIdQueryValidator();
            ValidationResult result = validator.Validate(new ListJobPostingsByEmployerIdQuery());

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void List_applications_requires_positive_job_posting_id_only()
        {
            var validator = new ListJobApplicationsByJobPostingIdQueryValidator();
            ValidationResult result = validator.Validate(
                new ListJobApplicationsByJobPostingIdQuery { JobPostingId = 0 });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.Field == nameof(ListJobApplicationsByJobPostingIdQuery.JobPostingId));
        }

        [Fact]
        public void Accept_application_requires_positive_application_and_job_posting_id()
        {
            var validator = new AcceptJobPostingApplicationCommandValidator();
            ValidationResult result = validator.Validate(
                new AcceptJobPostingApplicationCommand
                {
                    ApplicationId = 0,
                    JobPostingId = 0,
                });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain(e => e.Field == nameof(AcceptJobPostingApplicationCommand.ApplicationId));
            result.Errors.Should().Contain(e => e.Field == nameof(AcceptJobPostingApplicationCommand.JobPostingId));
        }

        [Fact]
        public void Reject_application_requires_positive_application_and_job_posting_id()
        {
            var validator = new RejectJobPostingApplicationCommandValidator();
            ValidationResult result = validator.Validate(
                new RejectJobPostingApplicationCommand
                {
                    ApplicationId = 0,
                    JobPostingId = 0,
                });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain(e => e.Field == nameof(RejectJobPostingApplicationCommand.ApplicationId));
            result.Errors.Should().Contain(e => e.Field == nameof(RejectJobPostingApplicationCommand.JobPostingId));
        }

        [Fact]
        public void Semantic_match_query_requires_limit_in_range()
        {
            var validator = new ListSemanticMatchedJobPostingsQueryValidator();
            ValidationResult result = validator.Validate(
                new ListSemanticMatchedJobPostingsQuery
                {
                    Limit = 100,
                });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().Contain(e => e.Field == nameof(ListSemanticMatchedJobPostingsQuery.Limit));
        }

        #endregion Methods
    }
}
