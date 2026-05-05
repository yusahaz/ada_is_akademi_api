namespace Azoxia.AdaIsAkademi.Application
{
    /// <summary>
    /// Result model for push notification sending attempts.
    /// </summary>
    public sealed record PushNotificationSendResult(
        bool IsSuccess,
        string? ErrorCode = null,
        string? ErrorMessage = null);
}
