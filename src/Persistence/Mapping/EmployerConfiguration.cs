namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="Employer"/> (columns and relationships).
    /// </summary>
    internal sealed class EmployerConfiguration :
        EntityTypeConfigurationBase<Employer>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<Employer> builder, ref int columnOrder)
        {
            builder.ComplexProperty(e => e.Address, a => ValueTypeComplexMapping.MapAddress(a, "Address"));
            builder.ComplexProperty(e => e.Contact, c => ValueTypeComplexMapping.MapContact(c, "Contact"));
            builder.ComplexProperty(e => e.TaxNumber, t => ValueTypeComplexMapping.MapStringValueColumn(t, nameof(Employer.TaxNumber), 32));

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();
        }

        #endregion Utils
    }
}
