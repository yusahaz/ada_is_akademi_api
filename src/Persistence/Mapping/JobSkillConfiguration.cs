namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Persistence.Mapping;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="JobSkill"/>.
    /// </summary>
    internal sealed class JobSkillConfiguration :
        EntityTypeConfigurationBase<JobSkill>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<JobSkill> builder, ref int columnOrder)
        {
            // Base mapping is sufficient (Name/Description/Audit + indexes).
        }

        #endregion Utils
    }
}
