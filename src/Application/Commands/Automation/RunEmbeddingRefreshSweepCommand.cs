namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Persistence;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Refreshes worker and posting embeddings for semantic matching pipeline.
    /// </summary>
    public class RunEmbeddingRefreshSweepCommand :
        CommandBase<int>;

    internal class RunEmbeddingRefreshSweepCommandValidator : IRequestValidator<RunEmbeddingRefreshSweepCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RunEmbeddingRefreshSweepCommand request)
            => new();

        #endregion Methods
    }

    internal class RunEmbeddingRefreshSweepCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RunEmbeddingRefreshSweepCommand, int>(serviceProvider)
    {
        #region Fields

        private const int EmbeddingDimension = 64;

        #endregion Fields

        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(RunEmbeddingRefreshSweepCommand command, CancellationToken cancellationToken)
        {
            IEmbeddingVectorizer vectorizer = ServiceProvider.GetService<IEmbeddingVectorizer>()
                ?? new HashEmbeddingVectorizer();
            IRepository<Worker> workerRepository = UnitOfWork.GetRepository<Worker>();
            IRepository<JobPosting> jobPostingRepository = UnitOfWork.GetRepository<JobPosting>();

            List<Worker> workers = (await workerRepository
                    .Filter(x => !x.IsDeleted)
                    .Include(x => x.Skills)
                    .ToListAsync(cancellationToken))
                .ToList();
            foreach (Worker worker in workers)
            {
                string text = BuildWorkerEmbeddingText(worker);
                worker.UpdateSkillEmbedding(vectorizer.Vectorize(text, EmbeddingDimension));
            }

            List<JobPosting> postings = (await jobPostingRepository
                    .Filter(x => !x.IsDeleted)
                    .Include(x => x.Skills)
                    .ToListAsync(cancellationToken))
                .ToList();
            foreach (JobPosting posting in postings)
            {
                string text = BuildJobPostingEmbeddingText(posting);
                posting.UpdateEmbedding(vectorizer.Vectorize(text, EmbeddingDimension));
            }

            int totalUpdated = workers.Count + postings.Count;
            if (totalUpdated == 0)
            {
                return 0;
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            await AdaIsReadModelCacheInvalidation.InvalidateSkillAndEmbeddingListCachesAsync(
                CacheService,
                cancellationToken);

            return totalUpdated;
        }

        private string BuildJobPostingEmbeddingText(JobPosting posting)
        {
            string tags = string.Join(' ', posting.Skills.Select(x => x.Tag.Value));
            return $"{posting.Title} {posting.Description} {tags}";
        }

        private string BuildWorkerEmbeddingText(Worker worker)
        {
            string tags = string.Join(' ', worker.Skills.Select(x => x.Tag.Value));
            return $"{worker.Nationality} {worker.University} {worker.Gender} {tags}";
        }

        #endregion Utils
    }
}
