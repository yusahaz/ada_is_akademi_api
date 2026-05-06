namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Lists inbox notifications for the authenticated system user.
    /// </summary>
    public class ListMyNotificationsQuery :
        QueryBase<PagedQueryResultModel<SystemUserNotificationListItemModel>>
    {
        #region Properties

        public bool? IsRead { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; }

        #endregion Properties
    }

    internal class ListMyNotificationsQueryValidator : IRequestValidator<ListMyNotificationsQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListMyNotificationsQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.Limit is < 1 or > 200)
            {
                failures.Add(ApplicationValidationCodes.ListMyNotificationsLimit.ForField(nameof(ListMyNotificationsQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(ApplicationValidationCodes.ListMyNotificationsOffset.ForField(nameof(ListMyNotificationsQuery.Offset)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListMyNotificationsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListMyNotificationsQuery, PagedQueryResultModel<SystemUserNotificationListItemModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<SystemUserNotificationListItemModel>> HandleAsync(
            ListMyNotificationsQuery query,
            CancellationToken cancellationToken)
        {
            IExecutionContext executionContext = ServiceProvider.GetRequiredService<IExecutionContext>();
            if (!int.TryParse(executionContext.GetClaim("system_user_id"), out int systemUserId) || systemUserId <= 0)
            {
                ApplicationValidationCodes.ActorSystemUserIdClaimRequired.Throw();
            }

            CacheKey cacheKey = AdaIsCacheKeys.SystemUserNotificationInboxKey(systemUserId, query.IsRead, query.Limit, query.Offset);
            PagedQueryResultModel<SystemUserNotificationListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<SystemUserNotificationListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                .GetRepository<SystemUserNotificationDispatch>()
                .Filter(x => x.SystemUserId == systemUserId);

            if (query.IsRead.HasValue)
            {
                filter = filter.Filter(x => x.IsRead == query.IsRead.Value);
            }

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            IReadOnlyList<SystemUserNotificationListItemModel> rows = (await filter
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip(query.Offset)
                    .Take(query.Limit)
                    .ToListAsync(
                        x => new SystemUserNotificationListItemModel(
                            x.Id,
                            x.Title,
                            x.Body,
                            x.TemplateCode,
                            x.Channel,
                            x.Status,
                            x.IsRead,
                            x.CreatedAt,
                            x.ReadAt,
                            x.SentAt,
                            x.WorkerId,
                            x.JobPostingId),
                        cancellationToken))
                .ToList();

            PagedQueryResultModel<SystemUserNotificationListItemModel> result =
                new(rows, totalCount, query.Limit, query.Offset);

            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.SystemUserNotificationDispatchDependency(systemUserId),
                    AdaIsCacheKeys.SystemUserNotificationDispatchAllDependency()),
                cancellationToken);

            return result;
        }

        #endregion Utils
    }
}
