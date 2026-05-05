namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Identity;
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lists semantically matched open job postings for a worker embedding.
    /// </summary>
    public class ListSemanticMatchedJobPostingsQuery :
        QueryBase<IReadOnlyList<SemanticMatchedJobPostingModel>>
    {
        #region Properties

        /// <summary>
        /// Max number of rows to return.
        /// </summary>
        public int Limit { get; set; } = 10;

        /// <summary>
        /// Legacy compatibility field; ignored and worker scope is resolved from JWT claims.
        /// </summary>
        public int WorkerId { get; set; }

        #endregion Properties
    }

    internal class ListSemanticMatchedJobPostingsQueryValidator : IRequestValidator<ListSemanticMatchedJobPostingsQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListSemanticMatchedJobPostingsQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.Limit <= 0 || request.Limit > 50)
            {
                failures.Add(ApplicationValidationCodes.ListSemanticMatchedJobPostingsLimitRange.ForField(nameof(ListSemanticMatchedJobPostingsQuery.Limit)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListSemanticMatchedJobPostingsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListSemanticMatchedJobPostingsQuery, IReadOnlyList<SemanticMatchedJobPostingModel>>(serviceProvider)
    {
        #region Fields

        private const int EmbeddingFreshnessDays = 30;

        #endregion Fields

        #region Utils

        /// <inheritdoc />
        protected override async Task<IReadOnlyList<SemanticMatchedJobPostingModel>> HandleAsync(
            ListSemanticMatchedJobPostingsQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int workerId = executionContext.RequireAdaIsWorkerActorId();

            CacheKey cacheKey = AdaIsCacheKeys.JobPostingSemanticMatchedListKey(workerId, query.Limit);
            IReadOnlyList<SemanticMatchedJobPostingModel>? cached =
                await CacheService.GetAsync<IReadOnlyList<SemanticMatchedJobPostingModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            Worker? worker = await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => x.Id == workerId)
                .AsNoTracking()
                .Include(x => x.InterestedJobCategories)
                .FirstOrDefaultAsync(cancellationToken);

            if (worker is null)
            {
                return [];
            }

            HashSet<int>? interestedCategoryFilter = worker.InterestedJobCategories.Count > 0
                ? worker.InterestedJobCategories.Select(x => x.JobCategoryId).ToHashSet()
                : null;

            if (!HasFreshEmbedding(worker))
            {
                IReadOnlyList<WorkerAvailability> availabilities = await LoadWorkerAvailabilitiesAsync(workerId, cancellationToken);
                IReadOnlyList<SemanticMatchedJobPostingModel> fallbackRows = await BuildFallbackRowsAsync(
                    query.Limit,
                    availabilities,
                    interestedCategoryFilter,
                    cancellationToken);
                await CacheService.SetAsync(
                    cacheKey,
                    fallbackRows,
                    AdaIsCacheKeys.DetailReadModelOptions(
                        AdaIsCacheKeys.WorkerDependency(workerId),
                        AdaIsCacheKeys.JobPostingAllDependency()),
                    cancellationToken);
                return fallbackRows;
            }

            IReadOnlyList<WorkerAvailability> workerAvailabilities = await LoadWorkerAvailabilitiesAsync(workerId, cancellationToken);
            IEnumerable<JobPosting> postings = await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Status == JobPostingStatus.Open && !x.IsDeleted && x.DescriptionEmbedding != null)
                .AsNoTracking()
                .OrderBy(x => x.ShiftDate)
                .ThenBy(x => x.ShiftStartTime)
                .ToListAsync(cancellationToken);

            if (interestedCategoryFilter is not null)
            {
                postings = postings.Where(x => interestedCategoryFilter.Contains(x.JobCategoryId));
            }

            IReadOnlyList<SemanticMatchedJobPostingModel> rows = postings
                .Where(x => IsWorkerAvailableForPosting(workerAvailabilities, x))
                .Where(x => x.DescriptionEmbedding is not null && x.DescriptionEmbedding.Length == worker.SkillEmbedding.Length)
                .Select(x => new SemanticMatchedJobPostingModel(
                    x.Id,
                    x.Title,
                    x.ShiftDate,
                    x.ShiftStartTime,
                    x.ShiftEndTime,
                    ComputeCosineSimilarity(worker.SkillEmbedding!, x.DescriptionEmbedding!)))
                .OrderByDescending(x => x.SimilarityScore)
                .Take(query.Limit)
                .ToList();

            await CacheService.SetAsync(
                cacheKey,
                rows,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.WorkerDependency(workerId),
                    AdaIsCacheKeys.JobPostingAllDependency()),
                cancellationToken);

            return rows;
        }

        private async Task<IReadOnlyList<WorkerAvailability>> LoadWorkerAvailabilitiesAsync(int workerId, CancellationToken cancellationToken)
        {
            return (await UnitOfWork
                    .GetRepository<WorkerAvailability>()
                    .Filter(x => x.WorkerId == workerId)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken))
                .ToList();
        }

        private async Task<IReadOnlyList<SemanticMatchedJobPostingModel>> BuildFallbackRowsAsync(
            int limit,
            IReadOnlyList<WorkerAvailability> availabilities,
            HashSet<int>? interestedCategoryIds,
            CancellationToken cancellationToken)
        {
            bool restrictCategories = interestedCategoryIds is { Count: > 0 };

            return (await UnitOfWork
                    .GetRepository<JobPosting>()
                    .Filter(x =>
                        x.Status == JobPostingStatus.Open &&
                        !x.IsDeleted &&
                        (!restrictCategories || interestedCategoryIds!.Contains(x.JobCategoryId)))
                    .AsNoTracking()
                    .OrderBy(x => x.ShiftDate)
                    .ThenBy(x => x.ShiftStartTime)
                    .ToListAsync(
                        x => new SemanticMatchedJobPostingModel(
                            x.Id,
                            x.Title,
                            x.ShiftDate,
                            x.ShiftStartTime,
                            x.ShiftEndTime,
                            0d),
                        cancellationToken))
                .Where(x => IsWorkerAvailableForPosting(availabilities, x.ShiftDate, x.ShiftStartTime, x.ShiftEndTime))
                .Take(limit)
                .ToList();
        }

        private bool HasFreshEmbedding(Worker worker)
            => worker.SkillEmbedding is not null
               && worker.SkillEmbedding.Length > 0
               && worker.EmbeddingUpdatedAt.HasValue
               && worker.EmbeddingUpdatedAt.Value >= DateTimeOffset.UtcNow.AddDays(-EmbeddingFreshnessDays);

        private double ComputeCosineSimilarity(float[] left, float[] right)
        {
            double dot = 0;
            double leftNorm = 0;
            double rightNorm = 0;

            for (int i = 0; i < left.Length; i++)
            {
                dot += left[i] * right[i];
                leftNorm += left[i] * left[i];
                rightNorm += right[i] * right[i];
            }

            if (leftNorm == 0 || rightNorm == 0)
            {
                return 0;
            }

            return dot / (Math.Sqrt(leftNorm) * Math.Sqrt(rightNorm));
        }

        private bool IsWorkerAvailableForPosting(IReadOnlyList<WorkerAvailability> availabilities, JobPosting posting)
            => IsWorkerAvailableForPosting(availabilities, posting.ShiftDate, posting.ShiftStartTime, posting.ShiftEndTime);

        private bool IsWorkerAvailableForPosting(
            IReadOnlyList<WorkerAvailability> availabilities,
            DateOnly shiftDate,
            TimeOnly shiftStartTime,
            TimeOnly shiftEndTime)
        {
            if (availabilities.Count == 0)
            {
                return true;
            }

            DayOfWeek dayOfWeek = shiftDate.DayOfWeek;
            return availabilities.Any(x =>
                x.DayOfWeek == dayOfWeek
                && x.TimeFrom <= shiftStartTime
                && x.TimeTo >= shiftEndTime);
        }

        #endregion Utils
    }
}
