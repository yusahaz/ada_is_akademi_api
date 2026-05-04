namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Job posting snapshot for read APIs.
    /// </summary>
    public sealed record JobPostingDetailModel(
        int Id,
        string Title,
        string Description,
        JobPostingStatus Status,
        int EmployerId,
        int EmployerLocationId,
        int JobCategoryId,
        DateOnly ShiftDate,
        TimeOnly ShiftStartTime,
        TimeOnly ShiftEndTime,
        decimal WageAmount,
        string WageCurrency,
        int HeadCount,
        int PendingApplications,
        int AcceptedApplications,
        IReadOnlyList<JobPostingSkillItemModel> Skills) :
        ModelBase;
}
