namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Link between a worker and a job category the worker selects for recommendation matching.
    /// </summary>
    public class WorkerInterestedJobCategory :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Initializes a blank row for EF Core materialization.
        /// </summary>
        protected WorkerInterestedJobCategory() { }

        /// <summary>
        /// Creates an association scoped to <paramref name="workerId"/>.
        /// </summary>
        /// <param name="workerId">Owning worker key.</param>
        /// <param name="jobCategoryId">Chosen <see cref="JobCategory"/> key.</param>
        protected internal WorkerInterestedJobCategory(
            int workerId,
            int jobCategoryId)
        {
            WorkerId = workerId;
            JobCategoryId = jobCategoryId;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Foreign key to <see cref="JobCategory"/>.
        /// </summary>
        public int JobCategoryId { get; private set; }

        /// <summary>
        /// Referenced classification node.
        /// </summary>
        public virtual JobCategory JobCategory { get; private set; }

        /// <summary>
        /// Foreign key to <see cref="Worker"/>.
        /// </summary>
        public int WorkerId { get; private set; }

        /// <summary>
        /// Owning worker aggregate root.
        /// </summary>
        public virtual Worker Worker { get; private set; }

        #endregion Properties
    }
}
