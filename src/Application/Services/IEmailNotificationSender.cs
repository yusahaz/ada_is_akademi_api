namespace Azoxia.AdaIsAkademi.Application
{
    /// <summary>
    /// Sends notification e-mails through configured provider.
    /// </summary>
    public interface IEmailNotificationSender
    {
        /// <summary>
        /// Sends one e-mail message.
        /// </summary>
        Task<EmailNotificationSendResult> SendAsync(
            string toEmail,
            string subject,
            string body,
            CancellationToken cancellationToken);
    }
}
