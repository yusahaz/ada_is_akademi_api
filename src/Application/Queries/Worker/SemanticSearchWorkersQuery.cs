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
    /// Employer-scoped worker semantic search query.
    /// </summary>
    public class SemanticSearchWorkersQuery :
        QueryBase<PagedQueryResultModel<SemanticSearchedWorkerListItemModel>>
    {
        #region Properties

        public int Limit { get; set; } = 20;
        public int? LocationId { get; set; }
        public int Offset { get; set; }
        public string QueryText { get; set; } = string.Empty;

        #endregion Properties
    }

    internal class SemanticSearchWorkersQueryValidator : IRequestValidator<SemanticSearchWorkersQuery>
    {
        /// <inheritdoc />
        public ValidationResult Validate(SemanticSearchWorkersQuery request)
        {
            List<ValidationFailure> failures = [];
            if (string.IsNullOrWhiteSpace(request.QueryText))
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(SemanticSearchWorkersQuery.QueryText)));
            }

            if (request.Limit is < 1 or > 100)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(SemanticSearchWorkersQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(AzoxiaErrorCodes.RequestValidationFailed.ForField(nameof(SemanticSearchWorkersQuery.Offset)));
            }

            return new ValidationResult(failures);
        }
    }

    internal class SemanticSearchWorkersQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<SemanticSearchWorkersQuery, PagedQueryResultModel<SemanticSearchedWorkerListItemModel>>(serviceProvider)
    {
        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<SemanticSearchedWorkerListItemModel>> HandleAsync(
            SemanticSearchWorkersQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            int employerId = executionContext.RequireAdaIsEmployerActorId();

            string normalized = query.QueryText.Trim().ToLowerInvariant();
            CacheKey cacheKey = new("query", "WorkersSemanticSearch", $"{employerId}:{query.LocationId}:{normalized}:{query.Limit}:{query.Offset}");
            PagedQueryResultModel<SemanticSearchedWorkerListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<SemanticSearchedWorkerListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            List<int> candidateWorkerIds = (await UnitOfWork
                .GetRepository<JobApplication>()
                .Filter(x =>
                    x.JobPosting.EmployerId == employerId
                    && (!query.LocationId.HasValue || x.JobPosting.EmployerLocationId == query.LocationId.Value))
                .AsNoTracking()
                .ToListAsync(x => x.WorkerId, cancellationToken))
                .Distinct()
                .ToList();

            if (candidateWorkerIds.Count == 0)
            {
                return new PagedQueryResultModel<SemanticSearchedWorkerListItemModel>([], 0, query.Limit, query.Offset);
            }

            List<SemanticSearchedWorkerListItemModel> rows = (await UnitOfWork
                .GetRepository<Worker>()
                .Filter(x => candidateWorkerIds.Contains(x.Id) && !x.IsDeleted)
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.SystemUser)
                .Include(x => x.Skills)
                .Include(x => x.Languages)
                .Include(x => x.Experiences)
                .ToListAsync(
                    x => new SemanticSearchedWorkerListItemModel(
                        x.Id,
                        $"{x.SystemUser.FirstName ?? string.Empty} {x.SystemUser.LastName ?? string.Empty}".Trim(),
                        CalculateSemanticScore(normalized, x),
                        CalculateReliabilityScore(x),
                        x.Experiences
                            .Where(e => e.EndDate.HasValue)
                            .OrderByDescending(e => e.EndDate)
                            .Select(e => (DateTimeOffset?)new DateTimeOffset(e.EndDate!.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero))
                            .FirstOrDefault(),
                        x.Skills.Select(s => s.Tag.Value).Take(10).ToList(),
                        x.Languages.Select(l => l.Language).Take(10).ToList(),
                        x.Experiences
                            .OrderByDescending(e => e.StartDate)
                            .Select(e => e.CompanyName)
                            .FirstOrDefault() ?? string.Empty),
                    cancellationToken))
                .OrderByDescending(x => x.SemanticScore)
                .ThenByDescending(x => x.ReliabilityScore)
                .ToList();

            int totalCount = rows.Count;
            IReadOnlyList<SemanticSearchedWorkerListItemModel> paged = rows.Skip(query.Offset).Take(query.Limit).ToList();
            PagedQueryResultModel<SemanticSearchedWorkerListItemModel> result = new(paged, totalCount, query.Limit, query.Offset);

            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.EmployerDependency(employerId),
                    AdaIsCacheKeys.WorkerAllDependency()),
                cancellationToken);

            return result;
        }

        private decimal CalculateReliabilityScore(Worker worker)
        {
            int skillCount = worker.Skills.Count;
            int languageCount = worker.Languages.Count;
            int experienceCount = worker.Experiences.Count;
            decimal score = (skillCount * 6m) + (languageCount * 7m) + (experienceCount * 9m);
            return decimal.Min(100m, score);
        }

        private decimal CalculateSemanticScore(string queryText, Worker worker)
        {
            if (string.IsNullOrWhiteSpace(queryText))
            {
                return 0m;
            }

            int score = 0;
            string fullName = $"{worker.SystemUser.FirstName} {worker.SystemUser.LastName}".Trim().ToLowerInvariant();
            if (fullName.Contains(queryText))
            {
                score += 45;
            }

            if (worker.Skills.Any(x => x.Tag.Value.ToLowerInvariant().Contains(queryText)))
            {
                score += 40;
            }

            if (worker.Languages.Any(x => x.Language.ToLowerInvariant().Contains(queryText)))
            {
                score += 15;
            }

            return decimal.Min(100m, score);
        }
    }
}
