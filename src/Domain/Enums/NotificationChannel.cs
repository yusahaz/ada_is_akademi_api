namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Delivery channel for worker notifications.
    /// </summary>
    public enum NotificationChannel
    {
        /// <summary>
        /// Email channel.
        /// </summary>
        Email = 20,

        /// <summary>
        /// In-app inbox channel.
        /// </summary>
        InApp = 30,

        /// <summary>
        /// Mobile push channel (FCM/APNS).
        /// </summary>
        Push = 10,
    }
}
