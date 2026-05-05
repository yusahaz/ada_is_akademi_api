namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Single row model for overdue alarm export package.
    /// </summary>
    public sealed record OverdueAlarmExportItemModel(
        DateOnly AlarmDate,
        DateOnly ShiftDate,
        int JobPostingId,
        string JobPostingStatus,
        string Title) :
        ModelBase;
}
