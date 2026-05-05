namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Formal education entry linked to a worker profile.
    /// </summary>
    public class WorkerEducation :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected WorkerEducation() { }

        /// <summary>
        /// Creates an education history row.
        /// </summary>
        protected internal WorkerEducation(
            int workerId,
            string school,
            string department,
            EducationType educationType,
            int startYear,
            int? endYear,
            bool isOngoing)
        {
            WorkerId = workerId;
            School = school;
            Department = department;
            EducationType = educationType;
            StartYear = startYear;
            IsOngoing = isOngoing;
            EndYear = endYear;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Academic department or program name.
        /// </summary>
        public string Department { get; private set; }

        /// <summary>
        /// Broad category of the credential (degree level, etc.).
        /// </summary>
        public EducationType EducationType { get; private set; }

        /// <summary>
        /// Graduation or expected final year, when not ongoing.
        /// </summary>
        public int? EndYear { get; private set; }

        /// <summary>
        /// True while the program has not yet concluded.
        /// </summary>
        public bool IsOngoing { get; private set; }

        /// <summary>
        /// Institution name.
        /// </summary>
        public string School { get; private set; }

        /// <summary>
        /// Calendar year when studies began.
        /// </summary>
        public int StartYear { get; private set; }

        /// <summary>
        /// Foreign key to the owning worker.
        /// </summary>
        public int WorkerId { get; private set; }


        /// <summary>
        /// Owning worker aggregate.
        /// </summary>
        public virtual Worker Worker { get; private set; }

        #endregion Properties
    }
}
