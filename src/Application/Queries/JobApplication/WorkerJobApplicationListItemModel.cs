namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Worker-centric job application row with posting shift snapshot.
    /// </summary>
    public sealed record WorkerJobApplicationListItemModel(
        int ApplicationId,
        int JobPostingId,
        JobApplicationStatus Status,
        DateTimeOffset AppliedAt,
        string? Note,
        string JobTitle,
        string EmployerName,
        string? EmployerLogoObjectKey,
        string LocationText,
        DateOnly ShiftDate,
        TimeOnly ShiftStartTime,
        TimeOnly ShiftEndTime,
        int? AssignmentId) :
        ModelBase;
}
