namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Live status feed payload for worker dashboard polling.
    /// </summary>
    public sealed record WorkerLiveStatusFeedModel(
        IReadOnlyList<WorkerLiveStatusFeedItemModel> Items,
        DateTimeOffset GeneratedAtUtc) :
        ModelBase;
}
