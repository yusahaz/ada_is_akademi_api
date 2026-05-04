namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="JobPosting"/> (columns and relationships).
    /// </summary>
    internal sealed class JobPostingConfiguration :
        EntityTypeConfigurationBase<JobPosting>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<JobPosting> builder, ref int columnOrder)
        {
            builder.Property(e => e.EmployerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.EmployerLocationId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.JobCategoryId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.ComplexProperty(e => e.Wage, m => ValueTypeComplexMapping.MapMoney(m, "WageAmount", "WageCurrency"));

            builder.Property(e => e.Title)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(16384)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ShiftDate)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ShiftStartTime)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ShiftEndTime)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.HeadCount)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.DescriptionEmbedding)
                .HasColumnType("real[]")
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => new { e.EmployerId, e.Status });
            builder.HasIndex(e => new { e.EmployerLocationId, e.ShiftDate });

            builder.HasOne(e => e.Employer)
                .WithMany(p => p.JobPostings)
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.EmployerLocation)
                .WithMany()
                .HasForeignKey(e => e.EmployerLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.JobCategory)
                .WithMany()
                .HasForeignKey(e => e.JobCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Applications)
                .WithOne(a => a.JobPosting)
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Skills)
                .WithOne(s => s.JobPosting)
                .HasForeignKey(s => s.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
