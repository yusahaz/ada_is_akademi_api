namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="WorkerLanguage"/> (columns).
    /// </summary>
    internal sealed class WorkerLanguageConfiguration :
        EntityTypeConfigurationBase<WorkerLanguage>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<WorkerLanguage> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Language)
                .HasMaxLength(64)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Level)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.WorkerId, e.Language })
                .IsUnique();
        }

        #endregion Utils
    }
}
