namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Lists system user groups with optional filters and paging.
    /// </summary>
    public class ListSystemUserGroupsQuery :
        QueryBase<PagedQueryResultModel<SystemUserGroupListItemModel>>
    {
        public bool? IsActive { get; set; }
        public bool? IsSystem { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; }
        public string? SearchName { get; set; }
    }

    internal class ListSystemUserGroupsQueryValidator : IRequestValidator<ListSystemUserGroupsQuery>
    {
        public ValidationResult Validate(ListSystemUserGroupsQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200) failures.Add(ApplicationValidationCodes.ListSystemUserGroupsLimit.ForField(nameof(ListSystemUserGroupsQuery.Limit)));
            if (request.Offset < 0) failures.Add(ApplicationValidationCodes.ListSystemUserGroupsOffset.ForField(nameof(ListSystemUserGroupsQuery.Offset)));
            return new ValidationResult(failures);
        }
    }

    internal class ListSystemUserGroupsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListSystemUserGroupsQuery, PagedQueryResultModel<SystemUserGroupListItemModel>>(serviceProvider)
    {
        protected override async Task<PagedQueryResultModel<SystemUserGroupListItemModel>> HandleAsync(ListSystemUserGroupsQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.SystemUserGroupListKey(query);
            PagedQueryResultModel<SystemUserGroupListItemModel>? cached = await CacheService.GetAsync<PagedQueryResultModel<SystemUserGroupListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null) return cached;

            var filter = UnitOfWork.GetRepository<SystemUserGroup>().Filter().AsNoTracking();
            if (query.IsActive.HasValue) filter = filter.Filter(x => x.IsActive == query.IsActive.Value);
            if (query.IsSystem.HasValue) filter = filter.Filter(x => x.IsSystem == query.IsSystem.Value);
            if (!query.SearchName.IsNullOrWhiteSpace())
            {
                string s = query.SearchName.Trim().ToLowerInvariant();
                filter = filter.Filter(x => x.Name.ToLower().Contains(s));
            }

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            IReadOnlyList<SystemUserGroupListItemModel> rows = (await filter
                    .OrderBy(x => x.Level)
                    .ThenBy(x => x.Name)
                    .Skip(query.Offset)
                    .Take(query.Limit)
                    .ToListAsync(
                        x => new SystemUserGroupListItemModel(x.Id, x.IsActive, x.IsSystem, x.Level, x.Name),
                        cancellationToken))
                .ToList();

            PagedQueryResultModel<SystemUserGroupListItemModel> result = new(rows, totalCount, query.Limit, query.Offset);
            await CacheService.SetAsync(cacheKey, result, AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.SystemUserGroupAllDependency()), cancellationToken);
            return result;
        }
    }
}
