namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Worker weekly availability row.
    /// </summary>
    public sealed record WorkerAvailabilityDetailModel(
        int Id,
        DayOfWeek DayOfWeek,
        TimeOnly TimeFrom,
        TimeOnly TimeTo) :
        ModelBase;
}
