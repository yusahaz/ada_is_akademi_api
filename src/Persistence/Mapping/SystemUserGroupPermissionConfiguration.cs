namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="SystemUserGroupPermission"/> (columns).
    /// </summary>
    internal sealed class SystemUserGroupPermissionConfiguration :
        EntityTypeConfigurationBase<SystemUserGroupPermission>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<SystemUserGroupPermission> builder, ref int columnOrder)
        {
            builder.Property(e => e.SystemUserGroupId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.PermissionId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Effect)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.SystemUserGroupId, e.PermissionId })
                .IsUnique();

            builder.HasOne(e => e.Permission)
                .WithMany()
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
