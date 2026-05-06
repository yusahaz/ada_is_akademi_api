namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer and currency scoped reconciliation aggregate row.
    /// </summary>
    public sealed record FinancialReconciliationListItemModel(
        int EmployerId,
        string Currency,
        int ReceivableCount,
        decimal ReceivableAmount,
        int PaidPayoutCount,
        decimal PaidPayoutNetAmount,
        int OpenPayoutCount,
        decimal OpenPayoutNetAmount) :
        ModelBase;
}
