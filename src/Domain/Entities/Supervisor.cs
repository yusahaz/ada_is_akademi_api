namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Represents a supervisor assignment under an employer and optional location.
    /// </summary>
    public class Supervisor :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected Supervisor() { }

        /// <summary>
        /// Creates a supervisor link for an employer and optional location.
        /// </summary>
        /// <param name="employerId">Owning employer key.</param>
        /// <param name="systemUserId">Supervisor user key.</param>
        /// <param name="locationId">Optional location key.</param>
        protected internal Supervisor(
            int employerId,
            int systemUserId,
            int? locationId = null)
        {
            EmployerId = employerId;
            SystemUserId = systemUserId;
            LocationId = locationId;
            IsActive = true;
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Marks the supervisor assignment as active.
        /// </summary>
        protected internal void Activate()
            => IsActive = true;

        /// <summary>
        /// Binds the supervisor to a specific employer location.
        /// </summary>
        protected internal void AssignToLocation(int locationId)
            => LocationId = locationId;

        /// <summary>
        /// Marks the supervisor assignment as inactive.
        /// </summary>
        protected internal void Deactivate()
            => IsActive = false;

        /// <summary>
        /// Clears the optional location binding.
        /// </summary>
        protected internal void UnassignFromLocation()
            => LocationId = null;

        #endregion Utils

        #region Properties

        /// <summary>
        /// Owning employer identifier.
        /// </summary>
        public int EmployerId { get; private set; }

        /// <summary>
        /// Active flag for current supervisor assignment.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Optional assigned location identifier.
        /// </summary>
        public int? LocationId { get; private set; }

        /// <summary>
        /// Linked system user identifier for the supervisor.
        /// </summary>
        public int SystemUserId { get; private set; }


        /// <summary>
        /// Employer aggregate that owns this supervisor assignment.
        /// </summary>
        public virtual Employer Employer { get; private set; }

        /// <summary>
        /// Location this supervisor is assigned to, when available.
        /// </summary>
        public virtual EmployerLocation Location { get; private set; }

        /// <summary>
        /// System user account mapped as supervisor.
        /// </summary>
        public virtual SystemUser SystemUser { get; private set; }

        #endregion Properties
    }
}
