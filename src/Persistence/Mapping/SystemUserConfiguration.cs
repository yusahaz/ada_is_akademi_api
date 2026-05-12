namespace Azoxia.AdaIsAkademi.Persistence.Mapping
{
    using Azoxia.AdaIsAkademi.Domain;

    using Azoxia.Core.Persistence.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// EF Core mapping for <see cref="SystemUser"/> (columns and relationships).
    /// </summary>
    internal sealed class SystemUserConfiguration :
        EntityTypeConfigurationBase<SystemUser>
    {
        #region Utils

        /// <inheritdoc />
        protected override void Configure(EntityTypeBuilder<SystemUser> builder, ref int columnOrder)
        {
            builder.Property(e => e.Email)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.PasswordHash)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.PasswordSalt)
                .HasMaxLength(256)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.Type)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.AccountStatus)
                .HasConversion<int>()
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.FirstName)
                .HasMaxLength(256)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.LastName)
                .HasMaxLength(256)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.Phone)
                .HasMaxLength(64)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.FailedLoginAttempts)
                .HasColumnOrder(columnOrder++)
                .IsRequired();

            builder.Property(e => e.LastFailedLoginAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.LastSuccessfulLoginAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.LastPasswordChangeAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.EmailVerificationToken)
                .HasMaxLength(512)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.EmailVerificationExpiresAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.EmailVerifiedAt)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.Property(e => e.EmployerId)
                .HasColumnOrder(columnOrder++)
                .IsRequired(false);

            builder.HasIndex(e => e.Email)
                .IsUnique();

            builder.HasOne(e => e.Employer)
                .WithMany()
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(e => e.Devices)
                .WithOne(d => d.SystemUser)
                .HasForeignKey(d => d.SystemUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.RefreshTokens)
                .WithOne(t => t.SystemUser)
                .HasForeignKey(t => t.SystemUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion Utils
    }
}
