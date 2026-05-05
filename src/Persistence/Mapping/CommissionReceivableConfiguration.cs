namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Persistence.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="CommissionReceivable"/>.
    /// </summary>
    internal sealed class CommissionReceivableConfiguration :
        EntityTypeConfigurationBase<CommissionReceivable>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<CommissionReceivable> builder, ref int columnOrder)
        {
            builder.Property(e => e.EmployerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.PeriodStart)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.PeriodEnd)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.ComplexProperty(e => e.Amount, a => ValueTypeComplexMapping.MapMoney(a, "Amount", "Currency"));

            builder.HasIndex(e => new { e.EmployerId, e.PeriodStart, e.PeriodEnd })
                .IsUnique();

            builder.HasOne(e => e.Employer)
                .WithMany()
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
