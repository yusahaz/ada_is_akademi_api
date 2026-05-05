namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Single employment history segment for a worker.
    /// </summary>
    public class WorkerExperience :
        EntityBase
    {
        #region Ctors

        protected WorkerExperience() { }

        protected internal WorkerExperience(
            int workerId,
            string companyName,
            string position,
            DateOnly startDate,
            DateOnly? endDate,
            string? description = null)
        {
            WorkerId = workerId;
            CompanyName = companyName;
            Position = position;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
        }

        #endregion Ctors

        #region Properties
        /// <summary>
        /// Employer or organization name.
        /// </summary>
        public string CompanyName { get; private set; }

        /// <summary>
        /// Optional free-text responsibilities or achievements.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Last calendar day of employment, if no longer current.
        /// </summary>
        public DateOnly? EndDate { get; private set; }

        /// <summary>
        /// Role title during this employment.
        /// </summary>
        public string Position { get; private set; }

        /// <summary>
        /// First calendar day of employment.
        /// </summary>
        public DateOnly StartDate { get; private set; }

        /// <summary>
        /// Foreign key to the owning worker.
        /// </summary>
        public int WorkerId { get; private set; }

        /// <summary>
        /// True when no end date is set (treated as current role).
        /// </summary>
        public bool IsCurrent => EndDate is null;

        /// <summary>
        /// Owning worker aggregate.
        /// </summary>
        public virtual Worker Worker { get; private set; }
        #endregion Properties
    }
}
