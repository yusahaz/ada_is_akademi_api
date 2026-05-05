namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// Ensures cache key and dependency segments stay aligned for invalidation.
    /// </summary>
    public sealed class AdaIsCacheKeysTests
    {
        #region Methods

        /// <summary>
        /// Employer detail storage key must include the detail suffix; dependency index uses aggregate name and id.
        /// </summary>
        [Fact]
        public void Employer_detail_key_and_dependency_use_invariant_id()
        {
            var key = AdaIsCacheKeys.EmployerDetailKey(42);
            var dep = AdaIsCacheKeys.EmployerDependency(42);

            key.ToStorageKey().Should().Contain("EmployerDetail");
            dep.ToIndexSegment().Should().Be("Employer:42");
        }

        /// <summary>
        /// Job posting detail key must remain consistent with dependency invalidation tag.
        /// </summary>
        [Fact]
        public void JobPosting_detail_key_and_dependency_use_invariant_id()
        {
            var key = AdaIsCacheKeys.JobPostingDetailKey(7);
            var dep = AdaIsCacheKeys.JobPostingDependency(7);

            key.ToStorageKey().Should().Contain("JobPostingDetail");
            dep.ToIndexSegment().Should().Be("JobPosting:7");
        }

        /// <summary>
        /// Employer-scoped job posting list key aligns with summary invalidation tag.
        /// </summary>
        [Fact]
        public void Employer_job_postings_summary_key_and_dependency_use_invariant_id()
        {
            var key = AdaIsCacheKeys.EmployerJobPostingsSummaryKey(5, 20, 0);
            var dep = AdaIsCacheKeys.EmployerJobPostingsSummaryDependency(5);

            key.ToStorageKey().Should().Contain("JobPostingListByEmployer");
            dep.ToIndexSegment().Should().Be("JobPostingListByEmployer:5");
        }

        /// <summary>
        /// Employer-safe worker detail key pairs employer + worker ids; profile-view counters use a dedicated dependency tag.
        /// </summary>
        [Fact]
        public void Worker_employer_safe_detail_key_and_profile_view_dependency_use_invariant_ids()
        {
            var key = AdaIsCacheKeys.WorkerEmployerSafeDetailKey(5, 99);
            var workerDep = AdaIsCacheKeys.WorkerDependency(99);
            var viewDep = AdaIsCacheKeys.EmployerWorkerProfileViewStatDependency(5, 99);

            key.ToStorageKey().Should().Contain("WorkerEmployerSafeDetail");
            key.ToStorageKey().Should().Contain("5:99");
            workerDep.ToIndexSegment().Should().Be("Worker:99");
            viewDep.ToIndexSegment().Should().Be("EmployerWorkerProfileViewStat:5:99");
        }

        #endregion Methods
    }
}
