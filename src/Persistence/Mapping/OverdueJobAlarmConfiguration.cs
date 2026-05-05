namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.Core.Persistence.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="OverdueJobAlarm"/>.
    /// </summary>
    internal sealed class OverdueJobAlarmConfiguration :
        EntityTypeConfigurationBase<OverdueJobAlarm>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<OverdueJobAlarm> builder, ref int columnOrder)
        {
            builder.Property(e => e.JobPostingId)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.AlarmDate)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.HasIndex(e => new { e.JobPostingId, e.AlarmDate })
                .IsUnique();

            builder.HasIndex(e => e.AlarmDate);

            builder.HasOne(e => e.JobPosting)
                .WithMany()
                .HasForeignKey(e => e.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion Utils
    }
}
