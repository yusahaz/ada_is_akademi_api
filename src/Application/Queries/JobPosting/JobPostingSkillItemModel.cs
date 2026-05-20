namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Skill requirement row on a job posting detail read model.
    /// </summary>
    /// <param name="SkillId">Persisted job-posting skill row key.</param>
    /// <param name="Tag">Normalized skill tag text.</param>
    /// <param name="IsRequired">Whether the skill is mandatory for the posting.</param>
    public sealed record JobPostingSkillItemModel(
        int SkillId,
        string Tag,
        bool IsRequired) :
        ModelBase;
}
