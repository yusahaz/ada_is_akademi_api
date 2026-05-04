namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Personalized notification preview for a worker and job posting.
    /// </summary>
    public sealed record WorkerNotificationPreviewModel(
        int JobPostingId,
        string Channel,
        WorkerNotificationPreviewMessageModel Message,
        bool FallbackApplied) :
        ModelBase;
}
