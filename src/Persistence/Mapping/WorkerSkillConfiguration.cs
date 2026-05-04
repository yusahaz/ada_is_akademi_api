namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="WorkerSkill"/> (columns).
    /// </summary>
    internal sealed class WorkerSkillConfiguration :
        EntityTypeConfigurationBase<WorkerSkill>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<WorkerSkill> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.ComplexProperty(e => e.Tag, t => ValueTypeComplexMapping.MapStringValueColumn(t, nameof(WorkerSkill.Tag), 256));

            builder.Property(e => e.CreatedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            // Uniqueness (WorkerId, tag) is domain-enforced; no HasIndex on complex <see cref="WorkerSkill.Tag"/> (see JobPostingSkill mapping note).
        }

        #endregion Utils
    }
}
