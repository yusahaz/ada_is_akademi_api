namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using System.Globalization;

    /// <summary>
    /// Logical <see cref="CacheKey"/> values and <see cref="CacheDependency"/> tags for AdaIs read models.
    /// Query entries use namespace <c>query</c> and depend on domain aggregate names so command handlers can invalidate via <see cref="ICacheService.InvalidateByDependencyAsync"/>.
    /// </summary>
    internal static class AdaIsCacheKeys
    {
        #region Fields

        private const string QueryNamespace = "query";

        private const string DetailSuffix = "Detail";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Default TTLs for serialized detail DTOs (L1 short, L2 optional per Core defaults on <see cref="CacheEntryOptions"/>).
        /// </summary>
        /// <param name="dependencies">Aggregate roots that should evict this entry when invalidated.</param>
        internal static CacheEntryOptions DetailReadModelOptions(params CacheDependency[] dependencies) =>
            new()
            {
                Dependencies = dependencies,
            };

        /// <summary>
        /// Cache key for <see cref="EmployerDetailModel"/> by employer id.
        /// </summary>
        internal static CacheKey EmployerDetailKey(int employerId) =>
            new(QueryNamespace, nameof(Employer) + DetailSuffix, employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for an <see cref="Employer"/> aggregate instance.
        /// </summary>
        internal static CacheDependency EmployerDependency(int employerId) =>
            new(nameof(Employer), employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for <see cref="ListJobPostingsByEmployerIdQuery"/> result (summary rows per employer).
        /// </summary>
        internal static CacheKey EmployerJobPostingsSummaryKey(int employerId) =>
            new(QueryNamespace, "JobPostingListByEmployer", employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for the employer-scoped job posting list read model (any posting change under that employer).
        /// </summary>
        internal static CacheDependency EmployerJobPostingsSummaryDependency(int employerId) =>
            new("JobPostingListByEmployer", employerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for <see cref="JobPostingDetailModel"/> by posting id.
        /// </summary>
        internal static CacheKey JobPostingDetailKey(int jobPostingId) =>
            new(QueryNamespace, nameof(JobPosting) + DetailSuffix, jobPostingId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for a <see cref="JobPosting"/> aggregate instance.
        /// </summary>
        internal static CacheDependency JobPostingDependency(int jobPostingId) =>
            new(nameof(JobPosting), jobPostingId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Cache key for <see cref="WorkerDetailModel"/> by worker id.
        /// </summary>
        internal static CacheKey WorkerDetailKey(int workerId) =>
            new(QueryNamespace, nameof(Worker) + DetailSuffix, workerId.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Invalidation tag for a <see cref="Worker"/> aggregate instance.
        /// </summary>
        internal static CacheDependency WorkerDependency(int workerId) =>
            new(nameof(Worker), workerId.ToString(CultureInfo.InvariantCulture));

        #endregion Methods
    }
}
