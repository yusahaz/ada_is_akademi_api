namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Application.Services;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;

    internal class ListWorkersQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListWorkersQuery, PagedQueryResultModel<WorkerListItemModel>>(serviceProvider)
    {
        private static readonly TimeSpan ProfilePhotoViewTtl = TimeSpan.FromMinutes(10);

        protected override async Task<PagedQueryResultModel<WorkerListItemModel>> HandleAsync(ListWorkersQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.WorkerListKey(query);
            PagedQueryResultModel<WorkerListItemModel>? cached = await CacheService.GetAsync<PagedQueryResultModel<WorkerListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                IReadOnlyList<WorkerListItemModel> enrichedCached = await EnrichProfilePhotoViewUrlsAsync(cached.Items, cancellationToken);
                return new PagedQueryResultModel<WorkerListItemModel>(enrichedCached, cached.TotalCount, query.Limit, query.Offset);
            }

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
            if (!query.SearchName.IsNullOrWhiteSpace())
            {
                string s = query.SearchName.Trim().ToLowerInvariant();
                filter = filter.Filter(x =>
                    ((x.SystemUser.FirstName ?? string.Empty) + " " + (x.SystemUser.LastName ?? string.Empty)).ToLower().Contains(s) ||
                    (x.SystemUser.FirstName ?? string.Empty).ToLower().Contains(s) ||
                    (x.SystemUser.LastName ?? string.Empty).ToLower().Contains(s));
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
                            x.ProfilePhotoObjectKey,
                            null,
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
            IReadOnlyList<WorkerListItemModel> enriched = await EnrichProfilePhotoViewUrlsAsync(rows, cancellationToken);
            return new PagedQueryResultModel<WorkerListItemModel>(enriched, totalCount, query.Limit, query.Offset);
        }

        private async Task<IReadOnlyList<WorkerListItemModel>> EnrichProfilePhotoViewUrlsAsync(
            IReadOnlyList<WorkerListItemModel> rows,
            CancellationToken cancellationToken)
        {
            if (rows.Count == 0)
            {
                return rows;
            }

            IObjectStoragePresigner presigner = ServiceProvider.GetRequiredService<IObjectStoragePresigner>();
            List<WorkerListItemModel> result = new(capacity: rows.Count);

            foreach (WorkerListItemModel row in rows)
            {
                if (row.ProfilePhotoObjectKey.IsNullOrWhiteSpace())
                {
                    result.Add(row with { ProfilePhotoViewUrl = null });
                    continue;
                }

                try
                {
                    string url = await presigner.CreatePresignedGetAsync(row.ProfilePhotoObjectKey!, ProfilePhotoViewTtl, cancellationToken);
                    result.Add(row with { ProfilePhotoViewUrl = url });
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Worker list profile-photo presign failed for worker {WorkerId}.", row.WorkerId);
                    result.Add(row with { ProfilePhotoViewUrl = null });
                }
            }

            return result;
        }
    }
}
