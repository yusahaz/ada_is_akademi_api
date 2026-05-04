namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Classifies a client device platform for telemetry and policy.
    /// </summary>
    public enum DevicePlatform
    {
        /// <summary>
        /// Google Android devices.
        /// </summary>
        Android = 20,

        /// <summary>
        /// Apple iOS or iPadOS devices.
        /// </summary>
        iOS = 10,

        /// <summary>
        /// Browser or other web clients.
        /// </summary>
        Web = 30,
    }
}
