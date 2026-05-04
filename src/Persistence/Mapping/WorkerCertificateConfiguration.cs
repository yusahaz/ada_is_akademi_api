namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="WorkerCertificate"/> (columns).
    /// </summary>
    internal sealed class WorkerCertificateConfiguration :
        EntityTypeConfigurationBase<WorkerCertificate>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<WorkerCertificate> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Name)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.IssuingOrganization)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.IssuedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ExpiresAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.DocumentUrl)
                .HasMaxLength(2048)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.CreatedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => e.WorkerId);
        }

        #endregion Utils
    }
}
