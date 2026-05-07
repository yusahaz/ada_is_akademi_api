namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Employer-scoped shift history row.
    /// </summary>
    public sealed record ShiftAssignmentHistoryListItemModel(
        int AssignmentId,
        int WorkerId,
        ShiftAssignmentStatus Status,
        bool WasNoShow,
        DateTimeOffset? CompletedAt,
        string? AnomalySummary,
        string? DisputeSummary) :
        ModelBase;
}
