namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="SystemUserDevice"/> (columns).
    /// </summary>
    internal sealed class SystemUserDeviceConfiguration :
        EntityTypeConfigurationBase<SystemUserDevice>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<SystemUserDevice> builder, ref int columnOrder)
        {
            builder.Property(e => e.SystemUserId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.DeviceIdentifier)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Platform)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.DeviceToken)
                .HasMaxLength(2048)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.LastActiveAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.SystemUserId, e.DeviceIdentifier })
                .IsUnique();
        }

        #endregion Utils
    }
}
