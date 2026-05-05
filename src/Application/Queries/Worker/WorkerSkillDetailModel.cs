namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;
    using System;

    /// <summary>
    /// Worker skill row.
    /// </summary>
    public sealed record WorkerSkillDetailModel(
        int Id,
        string Tag,
        DateTimeOffset CreatedAt) :
        ModelBase;
}
