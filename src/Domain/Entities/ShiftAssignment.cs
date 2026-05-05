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
        #region Fields

        private const int AnomalyEarlyCheckOutMinutesThreshold = 30;
        private const int MutualQrGraceMinutesThreshold = 15;

        #endregion Fields

        #region Ctors

        protected ShiftAssignment() { }

        protected internal ShiftAssignment(
            int jobPostingId,
            int jobApplicationId,
            int workerId,
            string checkInTokenHash,
            string supervisorCheckInTokenHash)
        {
            JobPostingId = jobPostingId;
            JobApplicationId = jobApplicationId;
            WorkerId = workerId;
            CheckInTokenHash = checkInTokenHash;
            SupervisorCheckInTokenHash = supervisorCheckInTokenHash;
            Status = ShiftAssignmentStatus.Pending;
            AssignedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Utils

        protected internal void CheckIn(string checkInTokenHash)
        {
            (Status == ShiftAssignmentStatus.Pending || Status == ShiftAssignmentStatus.AwaitingMutualQr)
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentInvalidStatusTransition);
            (CheckInTokenHash == checkInTokenHash)
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentCheckInTokenInvalid);
            (!CheckedInAt.HasValue).ThrowIfFalse(DomainErrorCodes.ShiftAssignmentInvalidStatusTransition);

            DateTimeOffset now = DateTimeOffset.UtcNow;
            CheckedInAt = now;
            Status = ShiftAssignmentStatus.AwaitingMutualQr;
            TryCompleteMutualCheckIn(now);
        }

        protected internal void SupervisorCheckIn(string supervisorCheckInTokenHash)
        {
            (Status == ShiftAssignmentStatus.Pending || Status == ShiftAssignmentStatus.AwaitingMutualQr)
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentInvalidStatusTransition);
            (SupervisorCheckInTokenHash == supervisorCheckInTokenHash)
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentCheckInTokenInvalid);
            (!SupervisorCheckedInAt.HasValue).ThrowIfFalse(DomainErrorCodes.ShiftAssignmentInvalidStatusTransition);

            DateTimeOffset now = DateTimeOffset.UtcNow;
            SupervisorCheckedInAt = now;
            Status = ShiftAssignmentStatus.AwaitingMutualQr;
            TryCompleteMutualCheckIn(now);
        }

        protected internal void CheckOut()
        {
            (Status == ShiftAssignmentStatus.CheckedIn)
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentInvalidStatusTransition);

            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (CheckedInAt.HasValue &&
                now - CheckedInAt.Value < TimeSpan.FromMinutes(AnomalyEarlyCheckOutMinutesThreshold))
            {
                IsAnomalyFlagged = true;
                AnomalyCode = "EARLY_CHECKOUT";
            }

            CheckedOutAt = now;
            Status = ShiftAssignmentStatus.CheckedOut;
        }

        private void TryCompleteMutualCheckIn(DateTimeOffset now)
        {
            if (!CheckedInAt.HasValue || !SupervisorCheckedInAt.HasValue)
            {
                return;
            }

            TimeSpan difference = (CheckedInAt.Value - SupervisorCheckedInAt.Value).Duration();
            (difference <= TimeSpan.FromMinutes(MutualQrGraceMinutesThreshold))
                .ThrowIfFalse(DomainErrorCodes.ShiftAssignmentMutualQrWindowExpired);

            Status = ShiftAssignmentStatus.CheckedIn;
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Assignment creation timestamp.
        /// </summary>
        public DateTimeOffset AssignedAt { get; private set; }

        /// <summary>
        /// True when assignment lifecycle contains an anomaly marker.
        /// </summary>
        public bool IsAnomalyFlagged { get; private set; }

        /// <summary>
        /// Machine-friendly anomaly code when <see cref="IsAnomalyFlagged"/> is true.
        /// </summary>
        public string? AnomalyCode { get; private set; }

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
        /// Hash value expected during supervisor-side QR mutual confirmation.
        /// </summary>
        public string SupervisorCheckInTokenHash { get; private set; }

        /// <summary>
        /// Timestamp recorded when supervisor-side check-in succeeds.
        /// </summary>
        public DateTimeOffset? SupervisorCheckedInAt { get; private set; }

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
