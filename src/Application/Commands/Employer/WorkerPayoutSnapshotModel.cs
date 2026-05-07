namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Minimal payout snapshot returned after payout state transitions.
    /// </summary>
    public sealed record WorkerPayoutSnapshotModel(
        int WorkerPayoutId,
        WorkerPayoutStatus Status,
        bool IsLocked,
        DateTimeOffset UpdatedAt) :
        ModelBase;
}
