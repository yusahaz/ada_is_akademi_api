namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Persistence;

    /// <summary>
    /// Loads job category display names for a worker’s interested categories without EF <c>ThenInclude</c> on the application project.
    /// </summary>
    internal sealed class WorkerInterestedJobCategoryItemsReader(IUnitOfWork unitOfWork)
    {
        #region Methods

        /// <summary>
        /// Builds ordered item models for the worker’s <see cref="Worker.InterestedJobCategories"/> collection.
        /// </summary>
        /// <param name="worker">Worker entity (interested categories should be included on the query).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Ordered models with category names resolved from persistence.</returns>
        public async Task<IReadOnlyList<WorkerInterestedJobCategoryItemModel>> ListForWorkerAsync(
            Worker worker,
            CancellationToken cancellationToken)
        {
            worker = worker.ThrowIfNull(AzoxiaErrorCodes.ArgumentNull);
            IReadOnlyList<WorkerInterestedJobCategory> junction = worker.InterestedJobCategories;

            if (junction.Count == 0)
            {
                return [];
            }

            int[] ids = junction
                .Select(x => x.JobCategoryId)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            List<JobCategory> loaded = (await unitOfWork
                .GetRepository<JobCategory>()
                .Filter(x => ids.Contains(x.Id))
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .ToList();

            Dictionary<int, string> namesById = loaded.ToDictionary(x => x.Id, x => x.Name);

            return junction
                .OrderBy(x => x.JobCategoryId)
                .Select(x =>
                {
                    string name = namesById.TryGetValue(x.JobCategoryId, out string? n)
                        ? n
                        : string.Empty;
                    return new WorkerInterestedJobCategoryItemModel(x.JobCategoryId, name);
                })
                .ToList();
        }

        #endregion Methods
    }
}
