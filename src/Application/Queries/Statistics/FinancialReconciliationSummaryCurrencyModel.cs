namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Currency-scoped totals used by financial reconciliation summary.
    /// </summary>
    public sealed record FinancialReconciliationSummaryCurrencyModel(
        string Currency,
        decimal ReceivableAmount,
        decimal PaidPayoutNetAmount,
        decimal OpenPayoutNetAmount) :
        ModelBase;
}
