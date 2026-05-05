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
                .FirstOrDefaultAsync(cancellationToken);

            if (worker?.SkillEmbedding is null || worker.SkillEmbedding.Length == 0)
            {
                return [];
            }

            IEnumerable<JobPosting> postings = await UnitOfWork
                .GetRepository<JobPosting>()
                .Filter(x => x.Status == JobPostingStatus.Open && !x.IsDeleted && x.DescriptionEmbedding != null)
                .AsNoTracking()
                .OrderBy(x => x.ShiftDate)
                .ThenBy(x => x.ShiftStartTime)
                .ToListAsync(cancellationToken);

            IReadOnlyList<SemanticMatchedJobPostingModel> rows = postings
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

        #endregion Utils
    }
}
