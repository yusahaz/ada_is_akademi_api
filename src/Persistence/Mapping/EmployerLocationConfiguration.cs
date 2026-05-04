namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="EmployerLocation"/> (columns and relationships).
    /// </summary>
    internal sealed class EmployerLocationConfiguration :
        EntityTypeConfigurationBase<EmployerLocation>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<EmployerLocation> builder, ref int columnOrder)
        {
            builder.Property(e => e.EmployerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.ComplexProperty(e => e.Address, a => ValueTypeComplexMapping.MapAddress(a, "Address"));
            builder.ComplexProperty(e => e.Coordinate, g => ValueTypeComplexMapping.MapGeoCoordinate(g, "Coordinate"));

            builder.ComplexProperty(e => e.Contact!, c =>
            {
                c.IsRequired(false);
                ValueTypeComplexMapping.MapContact(c, "Contact");
            });

            builder.Property(e => e.GeofenceRadiusMetres)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => e.EmployerId);

            builder.HasOne(e => e.Employer)
                .WithMany(p => p.Locations)
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
