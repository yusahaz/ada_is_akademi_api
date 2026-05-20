namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Application.Validation;
    using Azoxia.Core.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Retries failed system-user notification dispatch rows.
    /// </summary>
    public class RetryFailedSystemUserNotificationsCommand :
        CommandBase<int>
    {
        #region Properties

        public int BatchSize { get; set; } = 100;

        #endregion Properties
    }

    internal class RetryFailedSystemUserNotificationsCommandValidator : IRequestValidator<RetryFailedSystemUserNotificationsCommand>
    {
        #region Methods

        /// <inheritdoc />
        public ValidationResult Validate(RetryFailedSystemUserNotificationsCommand request)
        {
            List<ValidationFailure> failures = [];
            if (request.BatchSize is < 1 or > 500)
            {
                failures.Add(ApplicationValidationCodes.RetryFailedSystemUserNotificationsBatchSize.ForField(nameof(RetryFailedSystemUserNotificationsCommand.BatchSize)));
            }

            return new ValidationResult(failures);
        }

        #endregion Methods
    }

    internal class RetryFailedSystemUserNotificationsCommandHandler(IServiceProvider serviceProvider) :
        CommandHandlerBase<RetryFailedSystemUserNotificationsCommand, int>(serviceProvider)
    {
        #region Utils

        /// <inheritdoc />
        protected override async Task<int> HandleAsync(RetryFailedSystemUserNotificationsCommand command, CancellationToken cancellationToken)
        {
            IPushNotificationSender sender = ServiceProvider.GetService<IPushNotificationSender>()
                ?? new NoopPushNotificationSender();
            IEmailNotificationSender emailSender = ServiceProvider.GetService<IEmailNotificationSender>()
                ?? new NoopEmailNotificationSender();

            List<SystemUserNotificationDispatch> failedRows = (await UnitOfWork
                    .GetRepository<SystemUserNotificationDispatch>()
                    .Filter(x => x.Status == NotificationDispatchStatus.Failed && x.RetryCount < 3)
                    .OrderBy(x => x.CreatedAt)
                    .Take(command.BatchSize)
                    .ToListAsync(cancellationToken))
                .ToList();

            int processed = 0;
            foreach (SystemUserNotificationDispatch row in failedRows)
            {
                SystemUser? systemUser = await UnitOfWork
                    .GetRepository<SystemUser>()
                    .Filter(x => x.Id == row.SystemUserId)
                    .Include(x => x.Devices)
                    .FirstOrDefaultAsync(cancellationToken);
                if (systemUser is null)
                {
                    continue;
                }

                row.MarkAsPendingRetry();

                List<string> tokens = systemUser.Devices
                    .Where(x => !x.DeviceToken.IsNullOrWhiteSpace())
                    .Select(x => x.DeviceToken!)
                    .Distinct()
                    .ToList();

                bool hasVerifiedEmail = systemUser.EmailVerifiedAt.HasValue && !systemUser.Email.IsNullOrWhiteSpace();
                if (tokens.Count > 0)
                {
                    PushNotificationSendResult result = await sender.SendAsync(tokens, row.Title, row.Body, cancellationToken);
                    if (result.IsSuccess)
                    {
                        row.MarkAsSent(NotificationChannel.Push);
                    }
                    else
                    {
                        row.MarkAsFailed(result.ErrorMessage ?? result.ErrorCode);
                        if (hasVerifiedEmail)
                        {
                            EmailNotificationSendResult emailResult = await emailSender.SendAsync(
                                systemUser.Email!,
                                row.Title,
                                row.Body,
                                cancellationToken);
                            if (emailResult.IsSuccess)
                            {
                                row.MarkAsSent(NotificationChannel.Email, "push_retry_failed");
                            }
                            else
                            {
                                row.MarkAsSent(NotificationChannel.InApp, "push_retry_failed_email_send_failed");
                            }
                        }
                        else
                        {
                            row.MarkAsSent(NotificationChannel.InApp, "push_retry_failed_and_unverified_email");
                        }
                    }
                }
                else if (hasVerifiedEmail)
                {
                    EmailNotificationSendResult emailResult = await emailSender.SendAsync(
                        systemUser.Email!,
                        row.Title,
                        row.Body,
                        cancellationToken);
                    if (emailResult.IsSuccess)
                    {
                        row.MarkAsSent(NotificationChannel.Email, "missing_push_token");
                    }
                    else
                    {
                        row.MarkAsSent(NotificationChannel.InApp, "missing_push_token_email_send_failed");
                    }
                }
                else
                {
                    row.MarkAsSent(NotificationChannel.InApp, "missing_push_token_and_unverified_email");
                }

                processed++;
            }

            if (processed == 0)
            {
                return 0;
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);
            await AdaIsReadModelCacheInvalidation.InvalidateSystemUserNotificationScopesAsync(
                CacheService,
                systemUserId: null,
                workerId: null,
                cancellationToken);

            return processed;
        }

        private sealed class NoopPushNotificationSender : IPushNotificationSender
        {
            public Task<PushNotificationSendResult> SendAsync(
                IReadOnlyList<string> deviceTokens,
                string title,
                string body,
                CancellationToken cancellationToken)
                => Task.FromResult(new PushNotificationSendResult(IsSuccess: true));
        }

        private sealed class NoopEmailNotificationSender : IEmailNotificationSender
        {
            public Task<EmailNotificationSendResult> SendAsync(
                string toEmail,
                string subject,
                string body,
                CancellationToken cancellationToken)
                => Task.FromResult(new EmailNotificationSendResult(IsSuccess: true));
        }

        #endregion Utils
    }
}
