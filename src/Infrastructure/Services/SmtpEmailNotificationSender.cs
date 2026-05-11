namespace Azoxia.AdaIsAkademi.Infrastructure.Services
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.AdaIsAkademi.Infrastructure.Configuration;
    using Azoxia.Core.Extensions;
    using Microsoft.Extensions.Logging;
    using System.Net;
    using System.Net.Mail;

    /// <summary>
    /// SMTP-backed e-mail notification sender.
    /// </summary>
    internal sealed class SmtpEmailNotificationSender(
        EmailConfig config,
        ILogger<SmtpEmailNotificationSender> logger) : IEmailNotificationSender
    {
        /// <inheritdoc />
        public async Task<EmailNotificationSendResult> SendAsync(
            string toEmail,
            string subject,
            string body,
            CancellationToken cancellationToken)
        {
            if (!config.Enabled)
            {
                return new EmailNotificationSendResult(false, "EMAIL_DISABLED", "E-mail delivery is disabled.");
            }

            if (config.Host.IsNullOrWhiteSpace() || config.FromEmail.IsNullOrWhiteSpace())
            {
                return new EmailNotificationSendResult(false, "EMAIL_NOT_CONFIGURED", "SMTP settings are incomplete.");
            }

            try
            {
                using MailMessage message = new();
                message.From = config.FromName.IsNullOrWhiteSpace()
                    ? new MailAddress(config.FromEmail!)
                    : new MailAddress(config.FromEmail!, config.FromName);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;

                using SmtpClient client = new(config.Host, config.Port)
                {
                    EnableSsl = config.UseSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                };

                if (config.UseAuthentication && !config.Username.IsNullOrWhiteSpace())
                {
                    client.Credentials = new NetworkCredential(config.Username, config.Password ?? string.Empty);
                }

                using CancellationTokenRegistration registration = cancellationToken.Register(client.SendAsyncCancel);
                await client.SendMailAsync(message).ConfigureAwait(false);
                return new EmailNotificationSendResult(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SMTP e-mail send failed. To={ToEmail}, Subject={Subject}", toEmail, subject);
                return new EmailNotificationSendResult(false, "EMAIL_SEND_FAILED", ex.Message);
            }
        }
    }
}
