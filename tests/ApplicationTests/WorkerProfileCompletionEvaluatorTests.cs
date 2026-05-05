namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.ValueTypes;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// Regression tests for deterministic worker profile completion weights.
    /// </summary>
    public sealed class WorkerProfileCompletionEvaluatorTests
    {
        #region Methods

        [Fact]
        public void CompletionPercentOf_aggregate_with_no_signals_returns_zero()
        {
            var evaluator = new WorkerProfileCompletionEvaluator();
            SystemUser user = new("wc-empty@test.local", "Password1!", SystemUserType.Worker);
            Worker worker = new(user.Id);

            evaluator.CompletionPercentOf(worker).Should().Be(0);
        }

        [Fact]
        public void CompletionPercentOf_with_only_skill_returns_weight_skills()
        {
            var evaluator = new WorkerProfileCompletionEvaluator();
            SystemUser user = new("wc-skills@test.local", "Password1!", SystemUserType.Worker);
            Worker worker = new(user.Id);
            worker.AddSkill("cooking");

            evaluator.CompletionPercentOf(worker).Should().Be(18);
        }

        [Fact]
        public void CompletionPercentOf_hundred_when_all_signals_present()
        {
            var evaluator = new WorkerProfileCompletionEvaluator();
            SystemUser user = new("wc-full@test.local", "Password1!", SystemUserType.Worker);
            Worker worker = new(user.Id);

            worker.AddSkill("a");
            worker.AddAvailability(System.DayOfWeek.Monday, new System.TimeOnly(9, 0), new System.TimeOnly(17, 0));
            worker.AddEducation("S", "D", EducationType.AssociateDegree, 2018, 2022, false);
            worker.UpdateProfile("TR", null);
            worker.UpdateExpectedSalaryRange(new Money(1m, "TRY"), new Money(10m, "TRY"));
            worker.ReplaceInterestedJobCategories([1]);
            worker.UpdateBio("Hello");
            worker.SetProfilePhotoObjectKey("workers/1/profile-photo/x");
            worker.ReplaceSocialLinks(
            [
                new WorkerSocialLinkInput(SocialMediaPlatform.Website, "https://example.com/worker"),
            ]);

            evaluator.CompletionPercentOf(worker).Should().Be(100);
        }

        #endregion Methods
    }
}
