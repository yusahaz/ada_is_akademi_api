namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Row model for commission receivable list results.
    /// </summary>
    public sealed record CommissionReceivableListItemModel(
        decimal Amount,
        string Currency,
        DateTimeOffset CreatedAt,
        int Id,
        DateOnly PeriodEnd,
        DateOnly PeriodStart) :
        ModelBase;
}
