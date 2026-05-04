namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="JobPostingSkill"/> (columns and relationships).
    /// </summary>
    internal sealed class JobPostingSkillConfiguration :
        EntityTypeConfigurationBase<JobPostingSkill>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<JobPostingSkill> builder, ref int columnOrder)
        {
            builder.Property(e => e.JobPostingId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.ComplexProperty(e => e.Tag, t => ValueTypeComplexMapping.MapStringValueColumn(t, nameof(JobPostingSkill.Tag), 256));

            builder.Property(e => e.IsRequired)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            // Unique (JobPostingId, skill tag) is enforced in <see cref="JobPosting.AddSkill"/>; EF cannot express a stable HasIndex on complex <see cref="JobPostingSkill.Tag"/> without colliding with complex mapping (EF 10). Add a DB-level unique index via migration/SQL if required.
        }

        #endregion Utils
    }
}
