namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Lists system users with optional filtering and paging.
    /// </summary>
    public class ListSystemUsersQuery :
        QueryBase<IReadOnlyList<SystemUserListItemModel>>
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
        QueryHandlerBase<ListSystemUsersQuery, IReadOnlyList<SystemUserListItemModel>>(serviceProvider)
    {
        protected override async Task<IReadOnlyList<SystemUserListItemModel>> HandleAsync(ListSystemUsersQuery query, CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.SystemUserListKey(query);
            IReadOnlyList<SystemUserListItemModel>? cached = await CacheService.GetAsync<IReadOnlyList<SystemUserListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null) return cached;

            var filter = UnitOfWork.GetRepository<SystemUser>().Filter().AsNoTracking();
            if (query.AccountStatus.HasValue) filter = filter.Filter(x => x.AccountStatus == query.AccountStatus.Value);
            if (query.Type.HasValue) filter = filter.Filter(x => x.Type == query.Type.Value);
            if (!string.IsNullOrWhiteSpace(query.SearchEmail))
            {
                string s = query.SearchEmail.Trim().ToLowerInvariant();
                filter = filter.Filter(x => x.Email.ToLower().Contains(s));
            }

            IReadOnlyList<SystemUserListItemModel> rows = (await filter
                    .OrderBy(x => x.Email)
                    .Skip(query.Offset)
                    .Take(query.Limit)
                    .ToListAsync(
                        x => new SystemUserListItemModel(x.AccountStatus, x.Email, x.Id, x.Type),
                        cancellationToken))
                .ToList();

            await CacheService.SetAsync(cacheKey, rows, AdaIsCacheKeys.DetailReadModelOptions(AdaIsCacheKeys.SystemUserAllDependency()), cancellationToken);
            return rows;
        }
    }
}
