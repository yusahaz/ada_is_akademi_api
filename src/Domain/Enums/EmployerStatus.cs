namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Lifecycle state of an employer organization record.
    /// </summary>
    public enum EmployerStatus
    {
        /// <summary>
        /// Employer is approved for publishing and hiring flows.
        /// </summary>
        Active = 20,

        /// <summary>
        /// Employer is permanently blocked from platform use.
        /// </summary>
        Banned = 90,

        /// <summary>
        /// Newly registered; awaiting activation or verification.
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Temporarily disabled by policy or administrators.
        /// </summary>
        Suspended = 30,
    }
}
