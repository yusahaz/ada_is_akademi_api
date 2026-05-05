namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Payload returned after attempting to record an employer-initiated worker profile view (UTC calendar-day dedupe).
    /// </summary>
    public sealed record RecordEmployerWorkerProfileViewResultModel(
        bool ViewCounted,
        int TotalEmployerSourcedProfileViews) :
        ModelBase;
}
