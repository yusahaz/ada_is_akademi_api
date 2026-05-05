namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Queries;

    /// <summary>
    /// Loads full worker profile detail by worker id.
    /// </summary>
    public class GetWorkerDetailQuery :
        QueryBase<WorkerFullDetailModel>
    {
        #region Properties

        /// <summary>
        /// Worker primary key.
        /// </summary>
        public int WorkerId { get; set; }

        #endregion Properties
    }
}
