namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Worker snapshot for read APIs.
    /// </summary>
    public sealed record WorkerDetailModel(
        int Id,
        int SystemUserId,
        string? Nationality,
        string? University,
        DateTimeOffset? EmbeddingUpdatedAt,
        IReadOnlyList<string> SkillTags) :
        ModelBase;
}
