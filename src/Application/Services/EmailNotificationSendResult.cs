namespace Azoxia.AdaIsAkademi.Application
{
    /// <summary>
    /// Result model for one e-mail send attempt.
    /// </summary>
    public sealed record EmailNotificationSendResult(
        bool IsSuccess,
        string? ErrorCode = null,
        string? ErrorMessage = null);
}
