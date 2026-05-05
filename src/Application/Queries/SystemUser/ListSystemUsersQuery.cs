namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Lists system users with optional filtering and paging.
    /// </summary>
    public class ListSystemUsersQuery :
        QueryBase<PagedQueryResultModel<SystemUserListItemModel>>
    {
        public AccountStatus? AccountStatus { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; }
        public string? SearchEmail { get; set; }
        public SystemUserType? Type { get; set; }
    }

    internal class ListSystemUsersQueryValidator : IRequestValidator<ListSystemUsersQuery>
    {
        public ValidationResult Validate(ListSystemUsersQuery request)
        {
            List<ValidationFailure> failures = [];
            if (request.Limit is < 1 or > 200) failures.Add(ApplicationValidationCodes.ListSystemUsersLimit.ForField(nameof(ListSystemUsersQuery.Limit)));
            if (request.Offset < 0) failures.Add(ApplicationValidationCodes.ListSystemUsersOffset.ForField(nameof(ListSystemUsersQuery.Offset)));
            return new ValidationResult(failures);
        }
    }

    internal class ListSystemUsersQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListSystemUsersQuery, PagedQueryResultModel<SystemUserListItemModel>>(serviceProvider)
    {
        protected override async Task<PagedQueryResultModel<SystemUserListItemModel>> HandleAsync(ListSystemUsersQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.SystemUserListKey(query);
            PagedQueryResultModel<SystemUserListItemModel>? cached = await CacheService.GetAsync<PagedQueryResultModel<SystemUserListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null) return cached;

            var filter = UnitOfWork.GetRepository<SystemUser>().Filter().AsNoTracking();
            if (query.AccountStatus.HasValue) filter = filter.Filter(x => x.AccountStatus == query.AccountStatus.Value);
            if (query.Type.HasValue) filter = filter.Filter(x => x.Type == query.Type.Value);
            if (!query.SearchEmail.IsNullOrWhiteSpace())
            {
                string s = query.SearchEmail.Trim().ToLowerInvariant();
                filter = filter.Filter(x => x.Email.ToLower().Contains(s));
            }

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            IReadOnlyList<SystemUserListItemModel> rows = (await filter
                    .OrderBy(x => x.Email)
                    .Skip(query.Offset)
                    .Take(query.Limit)
                    .ToListAsync(
                        x => new SystemUserListItemModel(x.AccountStatus, x.Email, x.Id, x.Type),
                        cancellationToken))
                .ToList();

            PagedQueryResultModel<SystemUserListItemModel> result = new(rows, totalCount, query.Limit, query.Offset);
            await CacheService.SetAsync(cacheKey, result, AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.SystemUserAllDependency()), cancellationToken);
            return result;
        }
    }
}
