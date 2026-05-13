namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Application.Models;
    using Azoxia.Core.ValueTypes;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Worker self-service summary including matching preferences omitted from employer read models.
    /// </summary>
    public sealed record WorkerSelfDetailModel(
        int Id,
        int SystemUserId,
        string? Nationality,
        WorkerGender Gender,
        string? University,
        string? CvOptions,
        string? Bio,
        string? ProfilePhotoObjectKey,
        DateTimeOffset? EmbeddingUpdatedAt,
        IReadOnlyList<string> SkillTags,
        Money? ExpectedSalaryMin,
        Money? ExpectedSalaryMax,
        int ProfileCompletionPercent,
        IReadOnlyList<WorkerInterestedJobCategoryItemModel> InterestedJobCategories) :
        ModelBase;
}
