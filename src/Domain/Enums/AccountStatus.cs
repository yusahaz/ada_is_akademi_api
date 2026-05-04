namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// High-level lifecycle state for a system user credential record.
    /// </summary>
    public enum AccountStatus
    {
        /// <summary>
        /// Account can authenticate subject to other policies.
        /// </summary>
        Active = 10,

        /// <summary>
        /// Account is blocked from all sign-in flows.
        /// </summary>
        Banned = 30,

        /// <summary>
        /// Created but not yet verified or fully provisioned.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Temporarily disabled by administrators.
        /// </summary>
        Suspended = 20,
    }
}
