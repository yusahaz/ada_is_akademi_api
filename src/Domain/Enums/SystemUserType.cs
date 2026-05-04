namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Role or audience classification for a system user account.
    /// </summary>
    public enum SystemUserType
    {
        /// <summary>
        /// Internal operator with elevated configuration access.
        /// </summary>
        Admin = 10,

        /// <summary>
        /// Organization user managing job postings and staff.
        /// </summary>
        Employer = 20,

        /// <summary>
        /// Individual applicant maintaining a worker profile.
        /// </summary>
        Worker = 30,
    }
}
