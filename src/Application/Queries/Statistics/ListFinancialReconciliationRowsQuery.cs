namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Lists employer/currency reconciliation rows with optional employer and date filters.
    /// </summary>
    public class ListFinancialReconciliationRowsQuery :
        QueryBase<PagedQueryResultModel<FinancialReconciliationListItemModel>>
    {
        #region Properties

        /// <summary>
        /// Optional employer identifier filter.
        /// </summary>
        public int? EmployerId { get; set; }

        /// <summary>
        /// Inclusive UTC lower bound for created-at filtering.
        /// </summary>
        public DateTimeOffset? From { get; set; }

        /// <summary>
        /// Maximum row count to return.
        /// </summary>
        public int Limit { get; set; } = 50;

        /// <summary>
        /// Zero-based row offset.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Inclusive UTC upper bound for created-at filtering.
        /// </summary>
        public DateTimeOffset? To { get; set; }

        #endregion Properties
    }

    internal class ListFinancialReconciliationRowsQueryValidator :
        IRequestValidator<ListFinancialReconciliationRowsQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(ListFinancialReconciliationRowsQuery request)
        {
            List<ValidationFailure> failures = [];

            if (request.EmployerId.HasValue && request.EmployerId <= 0)
            {
                failures.Add(ApplicationValidationCodes.ListFinancialReconciliationRowsEmployerId.ForField(nameof(ListFinancialReconciliationRowsQuery.EmployerId)));
            }

            if (request.Limit is < 1 or > 200)
            {
                failures.Add(ApplicationValidationCodes.ListFinancialReconciliationRowsLimit.ForField(nameof(ListFinancialReconciliationRowsQuery.Limit)));
            }

            if (request.Offset < 0)
            {
                failures.Add(ApplicationValidationCodes.ListFinancialReconciliationRowsOffset.ForField(nameof(ListFinancialReconciliationRowsQuery.Offset)));
            }

            if (request.From.HasValue &&
                request.To.HasValue &&
                request.From > request.To)
            {
                failures.Add(ApplicationValidationCodes.ListFinancialReconciliationRowsDateRange.ForField(nameof(ListFinancialReconciliationRowsQuery.From)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class ListFinancialReconciliationRowsQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<ListFinancialReconciliationRowsQuery, PagedQueryResultModel<FinancialReconciliationListItemModel>>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<PagedQueryResultModel<FinancialReconciliationListItemModel>> HandleAsync(
            ListFinancialReconciliationRowsQuery query,
            CancellationToken cancellationToken)
        {
            CacheKey cacheKey = AdaIsCacheKeys.FinancialReconciliationRowsKey(query.EmployerId, query.From, query.To, query.Limit, query.Offset);
            PagedQueryResultModel<FinancialReconciliationListItemModel>? cached =
                await CacheService.GetAsync<PagedQueryResultModel<FinancialReconciliationListItemModel>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            IReadOnlyList<CommissionReceivable> receivables = (await UnitOfWork
                    .GetRepository<CommissionReceivable>()
                    .Filter()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken))
                .Where(x => IsInRange(x.CreatedAt, query.From, query.To) &&
                            (!query.EmployerId.HasValue || x.EmployerId == query.EmployerId.Value))
                .ToList();

            IReadOnlyList<WorkerPayout> payouts = (await UnitOfWork
                    .GetRepository<WorkerPayout>()
                    .Filter()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken))
                .Where(x => IsInRange(x.CreatedAt, query.From, query.To) &&
                            (!query.EmployerId.HasValue || x.EmployerId == query.EmployerId.Value))
                .ToList();

            IReadOnlyList<FinancialReconciliationListItemModel> rows = BuildRows(receivables, payouts);

            int totalCount = rows.Count;
            IReadOnlyList<FinancialReconciliationListItemModel> paged = rows
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToList();

            PagedQueryResultModel<FinancialReconciliationListItemModel> result = new(
                paged,
                totalCount,
                query.Limit,
                query.Offset);

            await CacheService.SetAsync(
                cacheKey,
                result,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.CommissionReceivableAllDependency(),
                    AdaIsCacheKeys.WorkerPayoutAllDependency()),
                cancellationToken);

            return result;
        }

        private IReadOnlyList<FinancialReconciliationListItemModel> BuildRows(
            IReadOnlyList<CommissionReceivable> receivables,
            IReadOnlyList<WorkerPayout> payouts)
        {
            HashSet<(int EmployerId, string Currency)> keys = receivables
                .Select(x => (x.EmployerId, x.Amount.Currency))
                .Concat(payouts.Select(x => (x.EmployerId, x.NetAmount.Currency)))
                .ToHashSet();

            return keys
                .OrderBy(x => x.EmployerId)
                .ThenBy(x => x.Currency, StringComparer.OrdinalIgnoreCase)
                .Select(key =>
                {
                    IReadOnlyList<CommissionReceivable> receivableRows = receivables
                        .Where(x => x.EmployerId == key.EmployerId &&
                                    string.Equals(x.Amount.Currency, key.Currency, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    IReadOnlyList<WorkerPayout> payoutRows = payouts
                        .Where(x => x.EmployerId == key.EmployerId &&
                                    string.Equals(x.NetAmount.Currency, key.Currency, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    return new FinancialReconciliationListItemModel(
                        key.EmployerId,
                        key.Currency,
                        receivableRows.Count,
                        receivableRows.Sum(x => x.Amount.Amount),
                        payoutRows.Count(x => x.Status == WorkerPayoutStatus.Paid),
                        payoutRows.Where(x => x.Status == WorkerPayoutStatus.Paid).Sum(x => x.NetAmount.Amount),
                        payoutRows.Count(x => x.Status != WorkerPayoutStatus.Paid),
                        payoutRows.Where(x => x.Status != WorkerPayoutStatus.Paid).Sum(x => x.NetAmount.Amount));
                })
                .ToList();
        }

        private bool IsInRange(DateTimeOffset value, DateTimeOffset? from, DateTimeOffset? to)
        {
            bool afterFrom = !from.HasValue || value >= from.Value;
            bool beforeTo = !to.HasValue || value <= to.Value;
            return afterFrom && beforeTo;
        }

        #endregion Utils
    }
}
