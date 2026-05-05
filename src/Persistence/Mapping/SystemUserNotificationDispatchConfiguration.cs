namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="SystemUserNotificationDispatch"/>.
    /// </summary>
    internal sealed class SystemUserNotificationDispatchConfiguration :
        EntityTypeConfigurationBase<SystemUserNotificationDispatch>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<SystemUserNotificationDispatch> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.SystemUserId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.JobPostingId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.Channel)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.TemplateCode)
                .HasMaxLength(256)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Title)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Body)
                .HasMaxLength(4000)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.FallbackReason)
                .HasMaxLength(256)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.FailureReason)
                .HasMaxLength(1024)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.RetryCount)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.LastAttemptAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.SentAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => new { e.WorkerId, e.Status, e.CreatedAt });
            builder.HasIndex(e => new { e.SystemUserId, e.Status });
            builder.HasIndex(e => e.JobPostingId);

            builder.HasOne(e => e.Worker)
                .WithMany()
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SystemUser)
                .WithMany()
                .HasForeignKey(e => e.SystemUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.JobPosting)
                .WithMany()
                .HasForeignKey(e => e.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
