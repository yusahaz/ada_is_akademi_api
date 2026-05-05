namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Worker snapshot for employer-scoped APIs (no expected salary or interested categories).
    /// </summary>
    public sealed record WorkerEmployerSafeDetailModel(
        int Id,
        int SystemUserId,
        string? Nationality,
        string? University,
        DateTimeOffset? EmbeddingUpdatedAt,
        IReadOnlyList<string> SkillTags) :
        ModelBase;
}
