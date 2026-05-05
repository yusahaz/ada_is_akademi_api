namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Queries;

    /// <summary>
    /// Returns personalized notification preview with channel fallback for a worker.
    /// </summary>
    public class GetWorkerPersonalizedNotificationPreviewQuery :
        QueryBase<WorkerNotificationPreviewModel>
    {
        #region Properties

        /// <summary>
        /// Target posting used for notification context.
        /// </summary>
        public int JobPostingId { get; set; }

        #endregion Properties
    }

}
