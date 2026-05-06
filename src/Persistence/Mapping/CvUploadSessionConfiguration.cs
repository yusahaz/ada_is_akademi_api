namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="CvUploadSession"/>.
    /// </summary>
    internal sealed class CvUploadSessionConfiguration :
        EntityTypeConfigurationBase<CvUploadSession>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<CvUploadSession> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.FileFormat)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ObjectKey)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.FileName)
                .HasMaxLength(256)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ContentType)
                .HasMaxLength(128)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.FileSizeBytes)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ExtractionRequestedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.ExtractionCompletedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.ReviewedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.FailureReason)
                .HasMaxLength(1024)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.ExtractedJson)
                .HasColumnType("jsonb")
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => new { e.WorkerId, e.Status, e.CreatedAt });
            builder.HasIndex(e => e.ObjectKey).IsUnique();

            builder.HasOne(e => e.Worker)
                .WithMany()
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
