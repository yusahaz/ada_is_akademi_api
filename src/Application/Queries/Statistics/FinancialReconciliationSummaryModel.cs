namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Aggregated reconciliation counters and currency-based monetary totals.
    /// </summary>
    public sealed record FinancialReconciliationSummaryModel(
        int ReceivableCount,
        int PayoutPendingCount,
        int PayoutProcessingCount,
        int PayoutFailedCount,
        int PayoutPaidCount,
        IReadOnlyList<FinancialReconciliationSummaryCurrencyModel> CurrencySummaries) :
        ModelBase;
}
