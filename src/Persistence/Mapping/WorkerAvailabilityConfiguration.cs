namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="WorkerAvailability"/> (columns).
    /// </summary>
    internal sealed class WorkerAvailabilityConfiguration :
        EntityTypeConfigurationBase<WorkerAvailability>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<WorkerAvailability> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.DayOfWeek)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.TimeFrom)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.TimeTo)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.WorkerId, e.DayOfWeek });
        }

        #endregion Utils
    }
}
