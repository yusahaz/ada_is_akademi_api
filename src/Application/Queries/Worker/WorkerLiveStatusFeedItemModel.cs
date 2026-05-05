namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Single live feed row for worker assignment or matching updates.
    /// </summary>
    public sealed record WorkerLiveStatusFeedItemModel(
        string ItemType,
        int ReferenceId,
        string Title,
        string Body,
        string Severity,
        DateTimeOffset OccurredAtUtc) :
        ModelBase;
}
