namespace Azoxia.AdaIsAkademi.Domain
{
    using Azoxia.Core.Domain;
    using Azoxia.Core.Extensions;

    /// <summary>
    /// Outbox-style notification dispatch row for system-user mobile/email/in-app delivery.
    /// </summary>
    public class SystemUserNotificationDispatch :
        EntityBase
    {
        #region Fields

        private const int MaxRetryCount = 3;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Blank row initializer for EF Core.
        /// </summary>
        protected SystemUserNotificationDispatch() { }

        /// <summary>
        /// Creates a pending dispatch row with template payload.
        /// </summary>
        protected internal SystemUserNotificationDispatch(
            int systemUserId,
            NotificationChannel channel,
            string templateCode,
            string title,
            string body,
            int? workerId = null,
            int? jobPostingId = null)
        {
            SystemUserId = systemUserId;
            WorkerId = workerId;
            JobPostingId = jobPostingId;
            Channel = channel;
            TemplateCode = templateCode;
            Title = title;
            Body = body;
            Status = NotificationDispatchStatus.Pending;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        #endregion Ctors

        #region Utils

        /// <summary>
        /// Records a failed delivery attempt with retry accounting when allowed.
        /// </summary>
        protected internal void MarkAsFailed(string? reason = null)
        {
            (Status == NotificationDispatchStatus.Pending || Status == NotificationDispatchStatus.Failed)
                .ThrowIfFalse(DomainErrorCodes.NotificationDispatchInvalidStatusTransition);
            (RetryCount < MaxRetryCount)
                .ThrowIfFalse(DomainErrorCodes.NotificationDispatchRetryLimitExceeded);

            Status = NotificationDispatchStatus.Failed;
            RetryCount += 1;
            FailureReason = reason;
            LastAttemptAt = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Returns a failed dispatch to pending when retries remain.
        /// </summary>
        protected internal void MarkAsPendingRetry()
        {
            (Status == NotificationDispatchStatus.Failed)
                .ThrowIfFalse(DomainErrorCodes.NotificationDispatchInvalidStatusTransition);
            (RetryCount < MaxRetryCount)
                .ThrowIfFalse(DomainErrorCodes.NotificationDispatchRetryLimitExceeded);

            Status = NotificationDispatchStatus.Pending;
            FailureReason = null;
        }

        /// <summary>
        /// Marks the dispatch as successfully sent and updates channel metadata.
        /// </summary>
        protected internal void MarkAsSent(NotificationChannel deliveredChannel, string? fallbackReason = null)
        {
            (Status == NotificationDispatchStatus.Pending || Status == NotificationDispatchStatus.Failed)
                .ThrowIfFalse(DomainErrorCodes.NotificationDispatchInvalidStatusTransition);

            Channel = deliveredChannel;
            FallbackReason = fallbackReason;
            Status = NotificationDispatchStatus.Sent;
            SentAt = DateTimeOffset.UtcNow;
            LastAttemptAt = SentAt;
            FailureReason = null;
        }

        #endregion Utils

        #region Properties

        /// <summary>
        /// Message body payload.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Delivery channel for this row.
        /// </summary>
        public NotificationChannel Channel { get; private set; }

        /// <summary>
        /// Row creation instant.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// Failure reason from last delivery attempt.
        /// </summary>
        public string? FailureReason { get; private set; }

        /// <summary>
        /// Fallback reason if final channel differs from preferred push.
        /// </summary>
        public string? FallbackReason { get; private set; }

        /// <summary>
        /// Optional related posting id.
        /// </summary>
        public int? JobPostingId { get; private set; }

        /// <summary>
        /// Last delivery attempt instant.
        /// </summary>
        public DateTimeOffset? LastAttemptAt { get; private set; }

        /// <summary>
        /// Number of failed attempts.
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Delivery completion instant.
        /// </summary>
        public DateTimeOffset? SentAt { get; private set; }

        /// <summary>
        /// Current dispatch status.
        /// </summary>
        public NotificationDispatchStatus Status { get; private set; }

        /// <summary>
        /// Linked system user id.
        /// </summary>
        public int SystemUserId { get; private set; }

        /// <summary>
        /// Template code used by notification composer.
        /// </summary>
        public string TemplateCode { get; private set; }

        /// <summary>
        /// Message title payload.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Linked worker id.
        /// </summary>
        public int? WorkerId { get; private set; }


        /// <summary>
        /// Related posting navigation.
        /// </summary>
        public virtual JobPosting? JobPosting { get; private set; }

        /// <summary>
        /// Related system user navigation.
        /// </summary>
        public virtual SystemUser SystemUser { get; private set; }

        /// <summary>
        /// Related worker navigation.
        /// </summary>
        public virtual Worker? Worker { get; private set; }

        #endregion Properties
    }
}
