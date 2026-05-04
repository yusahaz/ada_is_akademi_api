namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="WorkerEducation"/> (columns).
    /// </summary>
    internal sealed class WorkerEducationConfiguration :
        EntityTypeConfigurationBase<WorkerEducation>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<WorkerEducation> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.School)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Department)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.EducationType)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.StartYear)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.EndYear)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.IsOngoing)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => e.WorkerId);
        }

        #endregion Utils
    }
}
