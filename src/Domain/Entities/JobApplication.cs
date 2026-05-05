namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Represents a worker's application for a specific job posting.
    /// </summary>
    public class JobApplication :
        EntityBase
    {
        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected JobApplication() { }

        /// <summary>
        /// Creates a pending application for the given posting and worker.
        /// </summary>
        /// <param name="jobPostingId">Target posting key.</param>
        /// <param name="workerId">Applicant worker key.</param>
        /// <param name="note">Optional applicant note.</param>
        protected internal JobApplication(
            int jobPostingId,
            int workerId,
            string? note = null)
        {
            JobPostingId = jobPostingId;
            WorkerId = workerId;
            Note = note;
            Status = JobApplicationStatus.Pending;
            AppliedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Transitions a pending application to accepted when rules allow.
        /// </summary>
        protected internal void Accept()
        {
            (Status == JobApplicationStatus.Pending)
                .ThrowIfFalse(DomainErrorCodes.JobApplicationInvalidStatusTransition);
            Status = JobApplicationStatus.Accepted;
        }

        /// <summary>
        /// Marks a pending application as expired when rules allow.
        /// </summary>
        protected internal void Expire()
        {
            (Status == JobApplicationStatus.Pending)
                .ThrowIfFalse(DomainErrorCodes.JobApplicationInvalidStatusTransition);
            Status = JobApplicationStatus.Expired;
        }

        /// <summary>
        /// Rejects a pending application and optionally records a reason.
        /// </summary>
        protected internal void Reject(string? reason = null)
        {
            (Status == JobApplicationStatus.Pending)
                .ThrowIfFalse(DomainErrorCodes.JobApplicationInvalidStatusTransition);
            Status = JobApplicationStatus.Rejected;
            RejectionReason = reason;
        }

        /// <summary>
        /// Withdraws a pending application when rules allow.
        /// </summary>
        protected internal void Withdraw()
        {
            (Status == JobApplicationStatus.Pending)
                .ThrowIfFalse(DomainErrorCodes.JobApplicationInvalidStatusTransition);
            Status = JobApplicationStatus.Withdrawn;
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Timestamp when the worker submitted the application.
        /// </summary>
        public DateTimeOffset AppliedAt { get; private set; }

        /// <summary>
        /// Owning job posting identifier.
        /// </summary>
        public int JobPostingId { get; private set; }

        /// <summary>
        /// Optional note supplied by the worker.
        /// </summary>
        public string? Note { get; private set; }

        /// <summary>
        /// Optional reason captured when the application is rejected.
        /// </summary>
        public string? RejectionReason { get; private set; }

        /// <summary>
        /// Current lifecycle status of the application.
        /// </summary>
        public JobApplicationStatus Status { get; private set; }

        /// <summary>
        /// Applicant worker identifier.
        /// </summary>
        public int WorkerId { get; private set; }

        /// <summary>
        /// Job posting this application belongs to.
        /// </summary>
        public virtual JobPosting JobPosting { get; private set; }

        /// <summary>
        /// Worker who created the application.
        /// </summary>
        public virtual Worker Worker { get; private set; }

        #endregion Properties
    }
}
