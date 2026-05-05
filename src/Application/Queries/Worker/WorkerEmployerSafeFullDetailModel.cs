namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Full worker profile snapshot for employers (matching preferences excluded).
    /// </summary>
    public sealed record WorkerEmployerSafeFullDetailModel(
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
        IReadOnlyList<WorkerReferenceDetailModel> References,
        int EmployerSourcedProfileViewCount) :
        ModelBase;
}
