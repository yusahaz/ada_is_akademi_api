namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Worker payout list item for employer billing views.
    /// </summary>
    public sealed record WorkerPayoutListItemModel(
        int WorkerPayoutId,
        int AssignmentId,
        int WorkerId,
        string WorkerName,
        decimal Amount,
        string Currency,
        WorkerPayoutStatus Status,
        bool IsLocked,
        string? LockReason,
        string? LockedBy,
        DateTimeOffset? LockedUntil,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt) :
        ModelBase;
}
