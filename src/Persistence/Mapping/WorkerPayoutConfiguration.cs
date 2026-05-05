namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="WorkerPayout"/>.
    /// </summary>
    internal sealed class WorkerPayoutConfiguration :
        EntityTypeConfigurationBase<WorkerPayout>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<WorkerPayout> builder, ref int columnOrder)
        {
            builder.Property(e => e.AssignmentId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.EmployerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.WorkerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.RetryCount)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.ProcessingMarkedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.ConfirmationDueAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.PaidAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.FailedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.LastFailureReason)
                .HasMaxLength(1024)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.ComplexProperty(e => e.GrossAmount, a => ValueTypeComplexMapping.MapMoney(a, "GrossAmount", "GrossCurrency"));
            builder.ComplexProperty(e => e.CommissionAmount, a => ValueTypeComplexMapping.MapMoney(a, "CommissionAmount", "CommissionCurrency"));
            builder.ComplexProperty(e => e.NetAmount, a => ValueTypeComplexMapping.MapMoney(a, "NetAmount", "NetCurrency"));

            builder.HasIndex(e => e.AssignmentId)
                .IsUnique();

            builder.HasIndex(e => new { e.EmployerId, e.Status });
            builder.HasIndex(e => new { e.WorkerId, e.Status });

            builder.HasOne(e => e.ShiftAssignment)
                .WithMany()
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Employer)
                .WithMany()
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Worker)
                .WithMany()
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
