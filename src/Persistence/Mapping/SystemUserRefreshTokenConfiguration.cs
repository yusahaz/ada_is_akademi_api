namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="SystemUserRefreshToken"/> (columns).
    /// </summary>
    internal sealed class SystemUserRefreshTokenConfiguration :
        EntityTypeConfigurationBase<SystemUserRefreshToken>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<SystemUserRefreshToken> builder, ref int columnOrder)
        {
            builder.Property(e => e.SystemUserId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.DeviceId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.TokenHash)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ExpiresAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.IsRevoked)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.SystemUserId, e.TokenHash })
                .IsUnique();

            builder.HasIndex(e => new { e.SystemUserId, e.DeviceId });

            builder.HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
