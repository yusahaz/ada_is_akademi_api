namespace Azoxia.AdaIsAkademi.Application
{
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
        int HeadCount) :
        ModelBase;
}
