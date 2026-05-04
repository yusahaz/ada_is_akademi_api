namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="JobApplication"/> (columns, relationships, and indexes).
    /// </summary>
    internal sealed class JobApplicationConfiguration :
        EntityTypeConfigurationBase<JobApplication>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<JobApplication> builder, ref int columnOrder)
        {
            builder.Property(e => e.JobPostingId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.AppliedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Note)
                .HasMaxLength(4000)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.RejectionReason)
                .HasMaxLength(4000)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => new { e.JobPostingId, e.WorkerId })
                .IsUnique();

            builder.HasIndex(e => new { e.WorkerId, e.Status });

            builder.HasOne(e => e.Worker)
                .WithMany()
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
