namespace Azoxia.AdaIsAkademi.Infrastructure
{
    using Azoxia.AdaIsAkademi.Application;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Placeholder push sender adapter; can be replaced by FCM implementation.
    /// </summary>
    internal class FakePushNotificationSender(ILogger<FakePushNotificationSender> logger) : IPushNotificationSender
    {
        #region Methods

        /// <inheritdoc />
        public Task<PushNotificationSendResult> SendAsync(
            IReadOnlyList<string> deviceTokens,
            string title,
            string body,
            CancellationToken cancellationToken)
        {
            if (deviceTokens.Count == 0)
            {
                return Task.FromResult(new PushNotificationSendResult(false, "NO_TOKEN", "No active device tokens."));
            }

            logger.LogInformation(
                "Fake push sent. TokenCount={TokenCount}, Title={Title}",
                deviceTokens.Count,
                title);

            return Task.FromResult(new PushNotificationSendResult(true));
        }

        #endregion Methods
    }
}
