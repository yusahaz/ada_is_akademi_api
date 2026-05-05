namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Lifecycle state of a worker application to a job posting.
    /// </summary>
    public enum JobApplicationStatus
    {
        /// <summary>
        /// Employer accepted the applicant for the shift.
        /// </summary>
        Accepted = 20,

        /// <summary>
        /// Submitted and awaiting employer decision.
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Employer declined the application.
        /// </summary>
        Rejected = 30,

        /// <summary>
        /// Applicant retracted before acceptance.
        /// </summary>
        Withdrawn = 40,

        /// <summary>
        /// Application automatically expired without an employer decision.
        /// </summary>
        Expired = 50,
    }
}
