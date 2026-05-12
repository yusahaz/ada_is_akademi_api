namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="Supervisor"/> (columns and relationships).
    /// </summary>
    internal sealed class SupervisorConfiguration :
        EntityTypeConfigurationBase<Supervisor>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<Supervisor> builder, ref int columnOrder)
        {
            builder.Property(e => e.EmployerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.SystemUserId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.LocationId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.IsActive)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.EmployerId, e.SystemUserId });

            builder.ToTable("Supervisor");

            builder.HasOne(e => e.Employer)
                .WithMany(p => p.Supervisors)
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.SystemUser)
                .WithMany()
                .HasForeignKey(e => e.SystemUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Location)
                .WithMany()
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
