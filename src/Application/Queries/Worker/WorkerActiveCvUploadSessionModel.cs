namespace Azoxia.AdaIsAkademi.Application
{
    using System;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Latest non-terminal CV upload session for the authenticated worker, if any.
    /// </summary>
    public sealed record WorkerActiveCvUploadSessionModel(
        int CvUploadSessionId,
        CvUploadSessionStatus Status,
        string FileName,
        DateTimeOffset CreatedAt) :
        ModelBase;
}
