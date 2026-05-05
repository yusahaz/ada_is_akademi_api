namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// CSV export row for system-user notification dispatch reporting.
    /// </summary>
    public sealed record SystemUserNotificationDispatchExportItemModel(
        int DispatchId,
        int SystemUserId,
        string SystemUserType,
        string Email,
        string Channel,
        string Status,
        string TemplateCode,
        string Title,
        int RetryCount,
        string? FallbackReason,
        DateTimeOffset CreatedAt,
        DateTimeOffset? LastAttemptAt,
        DateTimeOffset? SentAt) :
        ModelBase;
}
