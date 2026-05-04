namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="JobCategory"/> (columns and relationships).
    /// </summary>
    internal sealed class JobCategoryConfiguration :
        EntityTypeConfigurationBase<JobCategory>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<JobCategory> builder, ref int columnOrder)
        {
            builder.Property(e => e.ParentId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => e.ParentId);

            builder.HasOne(e => e.Parent)
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
