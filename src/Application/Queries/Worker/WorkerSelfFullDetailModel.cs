namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;
    using Azoxia.Core.ValueTypes;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Full worker profile snapshot for authenticated worker actor (includes matching preferences).
    /// </summary>
    public sealed record WorkerSelfFullDetailModel(
        int Id,
        int SystemUserId,
        string? Nationality,
        WorkerGender Gender,
        string? University,
        string? CvOptions,
        string? Bio,
        string? ProfilePhotoObjectKey,
        DateTimeOffset? EmbeddingUpdatedAt,
        WorkerSystemUserSummaryModel SystemUser,
        IReadOnlyList<WorkerSkillDetailModel> Skills,
        IReadOnlyList<WorkerAvailabilityDetailModel> Availabilities,
        IReadOnlyList<WorkerCertificateDetailModel> Certificates,
        IReadOnlyList<WorkerEducationDetailModel> Educations,
        IReadOnlyList<WorkerExperienceDetailModel> Experiences,
        IReadOnlyList<WorkerLanguageDetailModel> Languages,
        IReadOnlyList<WorkerReferenceDetailModel> References,
        IReadOnlyList<WorkerSocialLinkItemModel> SocialLinks,
        Money? ExpectedSalaryMin,
        Money? ExpectedSalaryMax,
        int ProfileCompletionPercent,
        IReadOnlyList<WorkerInterestedJobCategoryItemModel> InterestedJobCategories) :
        ModelBase;
}
