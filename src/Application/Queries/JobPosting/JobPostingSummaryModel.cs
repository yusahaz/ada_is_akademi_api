namespace Azoxia.AdaIsAkademi.Application
{
    using System.Collections.Generic;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Compact job posting row for browse / list read models.
    /// </summary>
    public sealed record JobPostingSummaryModel(
        int Id,
        string Title,
        DateOnly ShiftDate,
        TimeOnly ShiftStartTime,
        TimeOnly ShiftEndTime,
        decimal WageAmount,
        string WageCurrency,
        int EmployerId,
        string EmployerName,
        string? EmployerLogoObjectKey,
        string LocationText,
        int HeadCount,
        int ApplicationCount,
        JobPostingStatus Status,
        IReadOnlyList<string> Tags,
        IReadOnlyList<string> RequiredTags,
        string Description,
        double LocationLatitude,
        double LocationLongitude,
        double? DistanceMetres) :
        ModelBase;
}
