namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Persistence.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="ShiftAssignment"/> (columns and relationships).
    /// </summary>
    internal sealed class ShiftAssignmentConfiguration :
        EntityTypeConfigurationBase<ShiftAssignment>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<ShiftAssignment> builder, ref int columnOrder)
        {
            builder.Property(e => e.JobPostingId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.JobApplicationId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.CheckInTokenHash)
                .HasMaxLength(1024)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.SupervisorCheckInTokenHash)
                .HasMaxLength(1024)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.AssignedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.CheckedInAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.CheckedOutAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.SupervisorCheckedInAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => e.JobApplicationId)
                .IsUnique();

            builder.HasIndex(e => new { e.WorkerId, e.Status });

            builder.HasOne(e => e.JobPosting)
                .WithMany()
                .HasForeignKey(e => e.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.JobApplication)
                .WithMany()
                .HasForeignKey(e => e.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Worker)
                .WithMany()
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
