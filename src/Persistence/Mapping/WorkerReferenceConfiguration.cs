namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="WorkerReference"/> (columns).
    /// </summary>
    internal sealed class WorkerReferenceConfiguration :
        EntityTypeConfigurationBase<WorkerReference>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<WorkerReference> builder, ref int columnOrder)
        {
            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.ComplexProperty(e => e.Contact, c => ValueTypeComplexMapping.MapContact(c, "Contact"));

            builder.Property(e => e.Company)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Position)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => e.WorkerId);
        }

        #endregion Utils
    }
}
