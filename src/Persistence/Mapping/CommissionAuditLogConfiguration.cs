namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="CommissionAuditLog"/>.
    /// </summary>
    internal sealed class CommissionAuditLogConfiguration :
        EntityTypeConfigurationBase<CommissionAuditLog>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<CommissionAuditLog> builder, ref int columnOrder)
        {
            builder.Property(e => e.EmployerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.AssignmentId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.CommissionReceivableId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.WorkerPayoutId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.EventType)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Note)
                .HasMaxLength(2048)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.ComplexProperty(e => e.Amount, a => ValueTypeComplexMapping.MapMoney(a, "Amount", "Currency"));

            builder.HasIndex(e => new { e.EmployerId, e.CreatedAt });
            builder.HasIndex(e => e.WorkerPayoutId);
            builder.HasIndex(e => e.CommissionReceivableId);

            builder.HasOne(e => e.Employer)
                .WithMany()
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ShiftAssignment)
                .WithMany()
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.CommissionReceivable)
                .WithMany()
                .HasForeignKey(e => e.CommissionReceivableId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.WorkerPayout)
                .WithMany()
                .HasForeignKey(e => e.WorkerPayoutId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
