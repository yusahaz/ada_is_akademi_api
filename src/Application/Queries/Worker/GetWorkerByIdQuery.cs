namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Queries;

    /// <summary>
    /// Loads a single worker profile read model by identifier.
    /// </summary>
    public class GetWorkerByIdQuery :
        QueryBase<WorkerDetailModel>
    {
        #region Properties

        /// <summary>
        /// Worker primary key.
        /// </summary>
        public int WorkerId { get; set; }

        #endregion Properties
    }

}
