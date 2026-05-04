namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Structured JSON payload for notification message preview content.
    /// </summary>
    public sealed record WorkerNotificationPreviewMessageModel(
        string PostingTitle,
        DateOnly ShiftDate,
        string TemplateCode) :
        ModelBase;
}
