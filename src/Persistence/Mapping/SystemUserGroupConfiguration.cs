namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="SystemUserGroup"/> (columns and relationships).
    /// </summary>
    internal sealed class SystemUserGroupConfiguration :
        EntityTypeConfigurationBase<SystemUserGroup>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<SystemUserGroup> builder, ref int columnOrder)
        {
            builder.Property(e => e.Level)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.IsSystem)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.IsActive)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasMany(e => e.Permissions)
                .WithOne(p => p.SystemUserGroup)
                .HasForeignKey(p => p.SystemUserGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
