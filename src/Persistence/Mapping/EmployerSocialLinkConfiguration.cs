namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="EmployerSocialLink"/> (columns and relationships).
    /// </summary>
    internal sealed class EmployerSocialLinkConfiguration :
        EntityTypeConfigurationBase<EmployerSocialLink>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<EmployerSocialLink> builder, ref int columnOrder)
        {
            builder.Property(e => e.EmployerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Platform)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Url)
                .HasMaxLength(2048)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.EmployerId, e.Platform })
                .IsUnique();

            builder.HasOne(e => e.Employer)
                .WithMany(w => w.SocialLinks)
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
