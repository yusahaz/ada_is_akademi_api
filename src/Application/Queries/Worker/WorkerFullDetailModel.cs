namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Full worker profile detail snapshot.
    /// </summary>
    public sealed record WorkerFullDetailModel(
        int Id,
        int SystemUserId,
        string? Nationality,
        string? University,
        DateTimeOffset? EmbeddingUpdatedAt,
        WorkerSystemUserSummaryModel SystemUser,
        IReadOnlyList<WorkerSkillDetailModel> Skills,
        IReadOnlyList<WorkerAvailabilityDetailModel> Availabilities,
        IReadOnlyList<WorkerCertificateDetailModel> Certificates,
        IReadOnlyList<WorkerEducationDetailModel> Educations,
        IReadOnlyList<WorkerExperienceDetailModel> Experiences,
        IReadOnlyList<WorkerLanguageDetailModel> Languages,
        IReadOnlyList<WorkerReferenceDetailModel> References) :
        ModelBase;
}
