namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Represents a worker-to-posting assignment instance for shift attendance flow.
    /// </summary>
    public class ShiftAssignment :
        EntityBase
    {
        #region Ctors

        private ShiftAssignment() { }

        protected internal ShiftAssignment(
            int jobPostingId,
            int jobApplicationId,
            int workerId,
            string checkInTokenHash)
        {
            JobPostingId = jobPostingId;
            JobApplicationId = jobApplicationId;
            WorkerId = workerId;
            CheckInTokenHash = checkInTokenHash;
            Status = ShiftAssignmentStatus.Pending;
            AssignedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Utils

        protected internal void CheckIn(string checkInTokenHash)
        {
            (Status == ShiftAssignmentStatus.Pending)
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentInvalidStatusTransition);
            (CheckInTokenHash == checkInTokenHash)
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentCheckInTokenInvalid);

            CheckedInAt = DateTimeOffset.UtcNow;
            Status = ShiftAssignmentStatus.CheckedIn;
        }

        protected internal void CheckOut()
        {
            (Status == ShiftAssignmentStatus.CheckedIn)
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentInvalidStatusTransition);

            CheckedOutAt = DateTimeOffset.UtcNow;
            Status = ShiftAssignmentStatus.CheckedOut;
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Assignment creation timestamp.
        /// </summary>
        public DateTimeOffset AssignedAt { get; private set; }

        /// <summary>
        /// Timestamp recorded when worker check-in succeeds.
        /// </summary>
        public DateTimeOffset? CheckedInAt { get; private set; }

        /// <summary>
        /// Timestamp recorded when worker check-out succeeds.
        /// </summary>
        public DateTimeOffset? CheckedOutAt { get; private set; }

        /// <summary>
        /// Hash value expected during QR check-in verification.
        /// </summary>
        public string CheckInTokenHash { get; private set; }

        /// <summary>
        /// Source job application identifier.
        /// </summary>
        public int JobApplicationId { get; private set; }

        /// <summary>
        /// Assigned job posting identifier.
        /// </summary>
        public int JobPostingId { get; private set; }

        /// <summary>
        /// Current assignment lifecycle status.
        /// </summary>
        public ShiftAssignmentStatus Status { get; private set; }

        /// <summary>
        /// Worker identifier for this assignment.
        /// </summary>
        public int WorkerId { get; private set; }

        /// <summary>
        /// Source job application entity.
        /// </summary>
        public virtual JobApplication JobApplication { get; private set; }

        /// <summary>
        /// Assigned job posting entity.
        /// </summary>
        public virtual JobPosting JobPosting { get; private set; }

        /// <summary>
        /// Assigned worker entity.
        /// </summary>
        public virtual Worker Worker { get; private set; }

        #endregion Properties
    }
}
