namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;

    /// <summary>
    /// Represents a supervisor assignment under an employer and optional location.
    /// </summary>
    public class ShiftSupervisor :
        EntityBase
    {
        #region Ctors

        protected ShiftSupervisor() { }

        protected internal ShiftSupervisor(
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

        protected internal void Activate()
            => IsActive = true;

        protected internal void AssignToLocation(int locationId)
            => LocationId = locationId;

        protected internal void Deactivate()
            => IsActive = false;

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
