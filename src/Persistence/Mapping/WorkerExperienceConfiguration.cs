namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="WorkerExperience"/> (columns).
    /// </summary>
    internal sealed class WorkerExperienceConfiguration :
        EntityTypeConfigurationBase<WorkerExperience>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<WorkerExperience> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.CompanyName)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Position)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.StartDate)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.EndDate)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.Description)
                .HasMaxLength(4000)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => e.WorkerId);
        }

        #endregion Utils
    }
}
