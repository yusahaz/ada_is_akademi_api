namespace Azoxia.AdaIsAkademi.Application
{
    /// <summary>
    /// Sends push notifications to device tokens via provider adapter.
    /// </summary>
    public interface IPushNotificationSender
    {
        /// <summary>
        /// Sends one push payload to a set of device tokens.
        /// </summary>
        Task<PushNotificationSendResult> SendAsync(
            IReadOnlyList<string> deviceTokens,
            string title,
            string body,
            CancellationToken cancellationToken);
    }
}
