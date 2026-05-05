namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Lists commission receivable rows by employer.
    /// </summary>
    public class ListCommissionReceivablesByEmployerQuery :
        QueryBase<PagedQueryResultModel<CommissionReceivableListItemModel>>
    {
        #region Properties

        /// <summary>
        /// Employer identifier.
        /// </summary>
        public int EmployerId { get; set; }

        /// <summary>
        /// Maximum row count to return.
        /// </summary>
        public int Limit { get; set; } = 20;

        /// <summary>
        /// Zero-based row offset.
        /// </summary>
        public int Offset { get; set; }

        #endregion Properties
    }

    internal class ListCommissionReceivablesByEmployerQueryValidator : IRequestValidator<ListCommissionReceivablesByEmployerQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListCommissionReceivablesByEmployerQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.ListCommissionReceivablesByEmployerEmployerId.ForField(nameof(ListCommissionReceivablesByEmployerQuery.EmployerId)));
            }

            if (request.Limit is < 1 or > 100)
            {
                failures.Add(ApplicationValidationCodes.ListCommissionReceivablesByEmployerLimit.ForField(nameof(ListCommissionReceivablesByEmployerQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(ApplicationValidationCodes.ListCommissionReceivablesByEmployerOffset.ForField(nameof(ListCommissionReceivablesByEmployerQuery.Offset)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListCommissionReceivablesByEmployerQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListCommissionReceivablesByEmployerQuery, PagedQueryResultModel<CommissionReceivableListItemModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<CommissionReceivableListItemModel>> HandleAsync(
            ListCommissionReceivablesByEmployerQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.CommissionReceivableListKey(query.EmployerId, query.Limit, query.Offset);
            PagedQueryResultModel<CommissionReceivableListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<CommissionReceivableListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            var filter = UnitOfWork
                    .GetRepository<CommissionReceivable>()
                    .Filter(x => x.EmployerId == query.EmployerId)
                .AsNoTracking();

            int totalCount = checked((int)await filter.CountAsync(cancellationToken));

            IReadOnlyList<CommissionReceivableListItemModel> rows = (await filter
                    .OrderByDescending(x => x.PeriodStart)
                    .ThenByDescending(x => x.PeriodEnd)
                .Skip(query.Offset)
                    .Take(query.Limit)
                    .ToListAsync(
                        x => new CommissionReceivableListItemModel(
                            x.Amount.Amount,
                            x.Amount.Currency,
                            x.CreatedAt,
                            x.Id,
                            x.PeriodEnd,
                            x.PeriodStart),
                        cancellationToken))
                .ToList();

            PagedQueryResultModel<CommissionReceivableListItemModel> result = new(
                rows,
                totalCount,
                query.Limit,
                query.Offset);

            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.CommissionReceivableDependency(query.EmployerId),
                    AdaIsCacheKeys.CommissionReceivableAllDependency()),
                cancellationToken);

            return result;
        }

        #endregion Utils
    }
}
