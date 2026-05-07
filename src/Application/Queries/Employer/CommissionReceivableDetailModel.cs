namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Commission receivable detail read model.
    /// </summary>
    public sealed record CommissionReceivableDetailModel(
        decimal Amount,
        string Currency,
        DateTimeOffset CreatedAt,
        int EmployerId,
        int Id,
    string Period,
        DateOnly PeriodEnd,
    DateOnly PeriodStart,
    string Status,
    DateOnly DueDate,
    string? InvoicePdfUrl) :
        ModelBase;
}
