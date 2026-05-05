namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Worker experience row.
    /// </summary>
    public sealed record WorkerExperienceDetailModel(
        int Id,
        string CompanyName,
        string Position,
        DateOnly StartDate,
        DateOnly? EndDate,
        bool IsCurrent,
        string? Description) :
        ModelBase;
}
