namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Extensions;

    internal class ListWorkersQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListWorkersQuery, PagedQueryResultModel<WorkerListItemModel>>(serviceProvider)
    {
        protected override async Task<PagedQueryResultModel<WorkerListItemModel>> HandleAsync(ListWorkersQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.WorkerListKey(query);
            PagedQueryResultModel<WorkerListItemModel>? cached = await CacheService.GetAsync<PagedQueryResultModel<WorkerListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null) return cached;

            var filter = UnitOfWork.GetRepository<Worker>()
                .Filter()
                .Include(x => x.SystemUser)
                .AsNoTracking();

            if (query.AccountStatus.HasValue)
            {
                filter = filter.Filter(x => x.SystemUser.AccountStatus == query.AccountStatus.Value);
            }
            if (!query.SearchEmail.IsNullOrWhiteSpace())
            {
                string s = query.SearchEmail.Trim().ToLowerInvariant();
                filter = filter.Filter(x => x.SystemUser.Email.ToLower().Contains(s));
            }

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            IReadOnlyList<WorkerListItemModel> rows = (await filter
                    .OrderBy(x => x.Id)
                    .Skip(query.Offset)
                    .Take(query.Limit)
                    .ToListAsync(
                        x => new WorkerListItemModel(
                            x.SystemUser.AccountStatus,
                            x.SystemUser.Email,
                            x.SystemUser.FirstName,
                            x.SystemUser.LastName,
                            x.SystemUserId,
                            x.Id),
                        cancellationToken))
                .ToList();

            PagedQueryResultModel<WorkerListItemModel> result = new(rows, totalCount, query.Limit, query.Offset);
            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.WorkerAllDependency(), AdaIsCacheKeys.SystemUserAllDependency()),
                cancellationToken);
            return result;
        }
    }
}
