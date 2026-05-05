namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Delivery status for a notification dispatch row.
    /// </summary>
    public enum NotificationDispatchStatus
    {
        /// <summary>
        /// Row created and waiting to be processed.
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Notification successfully delivered through selected channel.
        /// </summary>
        Sent = 20,

        /// <summary>
        /// Delivery attempt failed.
        /// </summary>
        Failed = 30,
    }
}
