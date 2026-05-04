namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Single application row in a posting-centric list read model.
    /// </summary>
    public sealed record JobApplicationListItemModel(
        int ApplicationId,
        int WorkerId,
        JobApplicationStatus Status,
        DateTimeOffset AppliedAt,
        string? Note) :
        ModelBase;
}
