namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="Worker"/> (columns and relationships).
    /// </summary>
    internal sealed class WorkerConfiguration :
        EntityTypeConfigurationBase<Worker>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<Worker> builder, ref int columnOrder)
        {
            builder.Property(e => e.SystemUserId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Bio)
                .HasMaxLength(3000)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.ProfilePhotoObjectKey)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.Nationality)
                .HasMaxLength(128)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.University)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.SkillEmbedding)
                .HasColumnType("real[]")
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.EmbeddingUpdatedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.ExpectedSalaryMinAmount)
                .HasColumnType("numeric(18,2)")
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.ExpectedSalaryMinCurrency)
                .HasMaxLength(16)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.ExpectedSalaryMaxAmount)
                .HasColumnType("numeric(18,2)")
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.ExpectedSalaryMaxCurrency)
                .HasMaxLength(16)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => e.SystemUserId)
                .IsUnique();

            builder.HasOne(e => e.SystemUser)
                .WithMany()
                .HasForeignKey(e => e.SystemUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Skills)
                .WithOne(s => s.Worker)
                .HasForeignKey(s => s.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Availabilities)
                .WithOne(a => a.Worker)
                .HasForeignKey(a => a.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Certificates)
                .WithOne(c => c.Worker)
                .HasForeignKey(c => c.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Educations)
                .WithOne(ed => ed.Worker)
                .HasForeignKey(ed => ed.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Experiences)
                .WithOne(ex => ex.Worker)
                .HasForeignKey(ex => ex.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Languages)
                .WithOne(l => l.Worker)
                .HasForeignKey(l => l.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.References)
                .WithOne(r => r.Worker)
                .HasForeignKey(r => r.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.SocialLinks)
                .WithOne(s => s.Worker)
                .HasForeignKey(s => s.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
