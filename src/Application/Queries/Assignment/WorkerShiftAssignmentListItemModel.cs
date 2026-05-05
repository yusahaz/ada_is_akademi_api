namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Worker-centric shift assignment row with posting schedule snapshot.
    /// </summary>
    public sealed record WorkerShiftAssignmentListItemModel(
        int AssignmentId,
        int JobPostingId,
        int JobApplicationId,
        ShiftAssignmentStatus Status,
        bool IsAnomalyFlagged,
        string? AnomalyCode,
        DateTimeOffset AssignedAt,
        DateTimeOffset? CheckedInAt,
        DateTimeOffset? CheckedOutAt,
        DateOnly ShiftDate,
        TimeOnly ShiftStartTime,
        TimeOnly ShiftEndTime) :
        ModelBase;
}
