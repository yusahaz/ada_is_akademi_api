namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="EmployerWorkerProfileViewStat"/> (columns and relationships).
    /// </summary>
    internal sealed class EmployerWorkerProfileViewStatConfiguration :
        EntityTypeConfigurationBase<EmployerWorkerProfileViewStat>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<EmployerWorkerProfileViewStat> builder, ref int columnOrder)
        {
            builder.Property(e => e.EmployerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.TotalViews)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.LastRecordedUtc)
                .HasColumnOrder(columnOrder++);

            builder.HasIndex(e => new { e.EmployerId, e.WorkerId })
                .IsUnique();

            builder.HasOne<Employer>()
                .WithMany()
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Worker>()
                .WithMany()
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
