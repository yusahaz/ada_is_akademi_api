namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Authenticated system-user inbox row for notification listing.
    /// </summary>
    public sealed record SystemUserNotificationListItemModel(
        int Id,
        string Title,
        string Body,
        string TemplateCode,
        NotificationChannel Channel,
        NotificationDispatchStatus Status,
        bool IsRead,
        DateTimeOffset CreatedAt,
        DateTimeOffset? ReadAt,
        DateTimeOffset? SentAt,
        int? WorkerId,
        int? JobPostingId) :
        ModelBase;
}
