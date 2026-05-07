namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Synthetic dispute row built from assignment anomaly and payout states.
    /// </summary>
    public sealed record EmployerDisputeListItemModel(
        int DisputeId,
        int AssignmentId,
        int WorkerId,
        string ReasonCode,
        string ReasonText,
        string Status,
        DateTimeOffset OpenedAt,
        DateTimeOffset? ResolvedAt,
        bool IsAnomalyRelated,
        string? AnomalyCode) :
        ModelBase;
}
