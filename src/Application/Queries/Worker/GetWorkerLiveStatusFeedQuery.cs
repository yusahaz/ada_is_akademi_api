namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Queries;

    /// <summary>
    /// Returns live status feed rows for worker matching and assignment updates.
    /// </summary>
    public class GetWorkerLiveStatusFeedQuery :
        QueryBase<WorkerLiveStatusFeedModel>
    {
        #region Properties

        /// <summary>
        /// Max number of live feed rows.
        /// </summary>
        public int Limit { get; set; } = 10;

        #endregion Properties
    }
}
