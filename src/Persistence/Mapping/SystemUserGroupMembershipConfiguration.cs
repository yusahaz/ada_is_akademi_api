namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="SystemUserGroupMembership"/> (columns).
    /// </summary>
    internal sealed class SystemUserGroupMembershipConfiguration :
        EntityTypeConfigurationBase<SystemUserGroupMembership>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<SystemUserGroupMembership> builder, ref int columnOrder)
        {
            builder.Property(e => e.SystemUserGroupId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.SystemUserId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ScopeType)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ScopeId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.IsActive)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.SystemUserGroupId, e.SystemUserId, e.ScopeType, e.ScopeId });

            builder.HasOne(e => e.SystemUserGroup)
                .WithMany()
                .HasForeignKey(e => e.SystemUserGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.SystemUser)
                .WithMany()
                .HasForeignKey(e => e.SystemUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
