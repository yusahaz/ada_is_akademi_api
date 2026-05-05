namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Lists workers with optional filters and paging.
    /// </summary>
    public class ListWorkersQuery :
        QueryBase<IReadOnlyList<WorkerListItemModel>>
    {
        public AccountStatus? AccountStatus { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; }
        public string? SearchEmail { get; set; }
    }

    internal class ListWorkersQueryValidator : IRequestValidator<ListWorkersQuery>
    {
        public ValidationResult Validate(ListWorkersQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200) failures.Add(ApplicationValidationCodes.ListWorkersLimit.ForField(nameof(ListWorkersQuery.Limit)));
            if (request.Offset < 0) failures.Add(ApplicationValidationCodes.ListWorkersOffset.ForField(nameof(ListWorkersQuery.Offset)));
            return new ValidationResult(failures);
        }
    }

    internal class ListWorkersQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListWorkersQuery, IReadOnlyList<WorkerListItemModel>>(serviceProvider)
    {
        protected override async Task<IReadOnlyList<WorkerListItemModel>> HandleAsync(ListWorkersQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.WorkerListKey(query);
            IReadOnlyList<WorkerListItemModel>? cached = await CacheService.GetAsync<IReadOnlyList<WorkerListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null) return cached;

            var filter = UnitOfWork.GetRepository<Worker>()
                .Filter()
                .Include(x => x.SystemUser)
                .AsNoTracking();

            if (query.AccountStatus.HasValue)
            {
                filter = filter.Filter(x => x.SystemUser.AccountStatus == query.AccountStatus.Value);
            }
            if (!string.IsNullOrWhiteSpace(query.SearchEmail))
            {
                string s = query.SearchEmail.Trim().ToLowerInvariant();
                filter = filter.Filter(x => x.SystemUser.Email.ToLower().Contains(s));
            }

            IReadOnlyList<WorkerListItemModel> rows = (await filter
                    .OrderBy(x => x.Id)
                    .Skip(query.Offset)
                    .Take(query.Limit)
                    .ToListAsync(
                        x => new WorkerListItemModel(x.SystemUser.AccountStatus, x.SystemUser.Email, x.SystemUserId, x.Id),
                        cancellationToken))
                .ToList();

            await CacheService.SetAsync(
                cacheKey,
                rows,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.WorkerAllDependency(), AdaIsCacheKeys.SystemUserAllDependency()),
                cancellationToken);
            return rows;
        }
    }
}
