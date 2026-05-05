namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="WorkerInterestedJobCategory"/> (columns and relationships).
    /// </summary>
    internal sealed class WorkerInterestedJobCategoryConfiguration :
        EntityTypeConfigurationBase<WorkerInterestedJobCategory>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<WorkerInterestedJobCategory> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.JobCategoryId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.WorkerId, e.JobCategoryId })
                .IsUnique();

            builder.HasOne(e => e.Worker)
                .WithMany(w => w.InterestedJobCategories)
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.JobCategory)
                .WithMany()
                .HasForeignKey(e => e.JobCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
