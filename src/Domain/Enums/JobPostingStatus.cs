namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Lifecycle state of a job posting aggregate.
    /// </summary>
    public enum JobPostingStatus
    {
        /// <summary>
        /// Posting was withdrawn before completion.
        /// </summary>
        Cancelled = 40,

        /// <summary>
        /// Shift work finished and posting is closed.
        /// </summary>
        Completed = 30,

        /// <summary>
        /// Employer is still editing; not visible to applicants.
        /// </summary>
        Draft = 10,

        /// <summary>
        /// Capacity is fully staffed while the shift is still active.
        /// </summary>
        Filled = 25,

        /// <summary>
        /// Published and accepting applications within capacity.
        /// </summary>
        Open = 20,
    }
}
