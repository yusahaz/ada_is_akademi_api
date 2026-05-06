namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Queries;
    using Azoxia.Core.Application.Validation;

    /// <summary>
    /// Returns commission receivable and payout status totals for reconciliation reporting.
    /// </summary>
    public class GetFinancialReconciliationSummaryQuery :
        QueryBase<FinancialReconciliationSummaryModel>;

    internal class GetFinancialReconciliationSummaryQueryValidator :
        IRequestValidator<GetFinancialReconciliationSummaryQuery>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(GetFinancialReconciliationSummaryQuery request)
            => new();

        #endregion Methods
    }

    internal class GetFinancialReconciliationSummaryQueryHandler(IServiceProvider serviceProvider) :
        QueryHandlerBase<GetFinancialReconciliationSummaryQuery, FinancialReconciliationSummaryModel>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<FinancialReconciliationSummaryModel> HandleAsync(
            GetFinancialReconciliationSummaryQuery query,
            CancellationToken cancellationToken)
        {
            _ = query;
            CacheKey cacheKey = AdaIsCacheKeys.DashboardFinancialReconciliationSummaryKey();
            FinancialReconciliationSummaryModel? cached =
                await CacheService.GetAsync<FinancialReconciliationSummaryModel>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return cached;
            }

            IReadOnlyList<CommissionReceivable> receivables = (await UnitOfWork
                    .GetRepository<CommissionReceivable>()
                    .Filter()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken))
                .ToList();
            IReadOnlyList<WorkerPayout> payouts = (await UnitOfWork
                    .GetRepository<WorkerPayout>()
                    .Filter()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken))
                .ToList();

            HashSet<string> currencies = receivables
                .Select(x => x.Amount.Currency)
                .Concat(payouts.Select(x => x.NetAmount.Currency))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            IReadOnlyList<FinancialReconciliationSummaryCurrencyModel> currencySummaries = currencies
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Select(currency =>
                {
                    decimal receivableAmount = receivables
                        .Where(x => string.Equals(x.Amount.Currency, currency, StringComparison.OrdinalIgnoreCase))
                        .Sum(x => x.Amount.Amount);

                    decimal paidPayoutNetAmount = payouts
                        .Where(x => x.Status == WorkerPayoutStatus.Paid &&
                                    string.Equals(x.NetAmount.Currency, currency, StringComparison.OrdinalIgnoreCase))
                        .Sum(x => x.NetAmount.Amount);

                    decimal openPayoutNetAmount = payouts
                        .Where(x => x.Status != WorkerPayoutStatus.Paid &&
                                    string.Equals(x.NetAmount.Currency, currency, StringComparison.OrdinalIgnoreCase))
                        .Sum(x => x.NetAmount.Amount);

                    return new FinancialReconciliationSummaryCurrencyModel(
                        currency,
                        receivableAmount,
                        paidPayoutNetAmount,
                        openPayoutNetAmount);
                })
                .ToList();

            FinancialReconciliationSummaryModel model = new(
                receivables.Count,
                payouts.Count(x => x.Status == WorkerPayoutStatus.Pending),
                payouts.Count(x => x.Status == WorkerPayoutStatus.Processing),
                payouts.Count(x => x.Status == WorkerPayoutStatus.Failed),
                payouts.Count(x => x.Status == WorkerPayoutStatus.Paid),
                currencySummaries);

            await CacheService.SetAsync(
                cacheKey,
                model,
                AdaIsCacheKeys.DetailReadModelOptions(
                    AdaIsCacheKeys.CommissionReceivableAllDependency(),
                    AdaIsCacheKeys.WorkerPayoutAllDependency()),
                cancellationToken);

            return model;
        }

        #endregion Utils
    }
}
