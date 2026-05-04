namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="Permission"/> (columns).
    /// </summary>
    internal sealed class PermissionConfiguration :
        EntityTypeConfigurationBase<Permission>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<Permission> builder, ref int columnOrder)
        {
            builder.Property(e => e.ParentId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => e.ParentId);

            builder.HasOne(e => e.Parent)
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
