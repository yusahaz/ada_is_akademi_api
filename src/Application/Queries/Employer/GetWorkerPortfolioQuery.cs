namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Identity;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Returns employer portfolio rows for previously engaged workers.
    /// </summary>
    public class GetWorkerPortfolioQuery :
        QueryBase<IReadOnlyList<WorkerPortfolioListItemModel>>
    {
        #region Properties

        public int Limit { get; set; } = 50;

        #endregion Properties
    }

    internal class GetWorkerPortfolioQueryValidator : IRequestValidator<GetWorkerPortfolioQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(GetWorkerPortfolioQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(GetWorkerPortfolioQuery.Limit)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class GetWorkerPortfolioQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetWorkerPortfolioQuery, IReadOnlyList<WorkerPortfolioListItemModel>>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<IReadOnlyList<WorkerPortfolioListItemModel>> HandleAsync(
            GetWorkerPortfolioQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            CacheKey cacheKey = new("query", "EmployerWorkerPortfolio", $"{employerId}:{query.Limit}");
            IReadOnlyList<WorkerPortfolioListItemModel>? cached = await CacheService.GetAsync<IReadOnlyList<WorkerPortfolioListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            List<int> workerIds = (await UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x => x.JobPosting.EmployerId == employerId)
                .AsNoTracking()
                .ToListAsync(x => x.WorkerId, cancellationToken))
                .Distinct()
                .ToList();

            if (workerIds.Count == 0)
            {
                return [];
            }

            IReadOnlyList<WorkerPortfolioListItemModel> rows = (await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => workerIds.Contains(x.Id) && !x.IsDeleted)
                .AsNoTracking()
                .Include(x => x.SystemUser)
                .ToListAsync(
                    x => new WorkerPortfolioListItemModel(
                        x.Id,
                        $"{x.SystemUser.FirstName ?? string.Empty} {x.SystemUser.LastName ?? string.Empty}".Trim(),
                        0m,
                        0,
                        0,
                        0,
                        null),
                    cancellationToken))
                .ToList();

            Dictionary<int, List<ShiftAssignment>> assignmentsByWorker = (await UnitOfWork
                .GetRepository<ShiftAssignment>()
                .Filter(x => x.JobPosting.EmployerId == employerId && workerIds.Contains(x.WorkerId))
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .GroupBy(x => x.WorkerId)
                .ToDictionary(x => x.Key, x => x.ToList());

            IReadOnlyList<WorkerPortfolioListItemModel> projected = rows
                .Select(row =>
                {
                    assignmentsByWorker.TryGetValue(row.WorkerId, out List<ShiftAssignment>? assignments);
                    assignments ??= [];
                    int completed = assignments.Count(x => x.Status == ShiftAssignmentStatus.CheckedOut);
                    int noShow = assignments.Count(x => x.Status == ShiftAssignmentStatus.Pending && x.CheckedInAt == null);
                    int dispute = assignments.Count(x => x.Status != ShiftAssignmentStatus.CheckedOut);
                    DateTimeOffset? lastWorkedAt = assignments
                        .Where(x => x.CheckedOutAt.HasValue)
                        .OrderByDescending(x => x.CheckedOutAt)
                        .Select(x => x.CheckedOutAt)
                        .FirstOrDefault();
                    decimal reliability = decimal.Max(0m, decimal.Min(100m, 100m - (noShow * 12m) - (dispute * 8m) + (completed * 2m)));
                    return row with
                    {
                        ReliabilityScore = reliability,
                        CompletedAssignmentCount = completed,
                        NoShowCount = noShow,
                        DisputeCount = dispute,
                        LastWorkedAt = lastWorkedAt
                    };
                })
                .OrderByDescending(x => x.ReliabilityScore)
                .ThenByDescending(x => x.CompletedAssignmentCount)
                .Take(query.Limit)
                .ToList();

            await CacheService.SetAsync(
                cacheKey,
                projected,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.EmployerDependency(employerId),
                    AdaIsCacheKeys.ShiftAssignmentAllDependency(),
                    AdaIsCacheKeys.WorkerAllDependency()),
                cancellationToken);

            return projected;
        }
    }
}
