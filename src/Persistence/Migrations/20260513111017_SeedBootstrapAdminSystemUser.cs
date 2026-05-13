using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <summary>
    /// Inserts a single bootstrap <see cref="Azoxia.AdaIsAkademi.Domain.SystemUserType.Admin"/> row when no user with the same email exists.
    /// Default password matches <c>Ada!Test123*</c> (same as seed runner); change immediately in production.
    /// </summary>
    public partial class SeedBootstrapAdminSystemUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PBKDF2-SHA256, 100000 iterations, 32-byte hash, 16-byte salt — aligned with Domain.SystemUser.
            // Salt is deterministic (SHA256 prefix) so the migration is reproducible across environments.
            const string bootstrapEmail = "admin@adaisakademi.dev";
            const string passwordHashB64 = "NfT6OyIkNb8BkD8lRvllE30zMF5c+S5SHblw2L9fCf8=";
            const string passwordSaltB64 = "f8+G+4bxnjX6N0ShvcTvQg==";

            migrationBuilder.Sql(
                $"""
                INSERT INTO "SystemUser" (
                    "Email",
                    "PasswordHash",
                    "PasswordSalt",
                    "Type",
                    "AccountStatus",
                    "FirstName",
                    "LastName",
                    "Phone",
                    "FailedLoginAttempts",
                    "LastFailedLoginAt",
                    "LastSuccessfulLoginAt",
                    "LastPasswordChangeAt",
                    "EmailVerificationToken",
                    "EmailVerificationExpiresAt",
                    "EmailVerifiedAt",
                    "EmployerId",
                    "CreatedAt",
                    "CreatedBy",
                    "UpdatedAt",
                    "UpdatedBy",
                    "IsDeleted",
                    "DeletedAt",
                    "DeletedBy"
                )
                SELECT
                    lower(trim('{bootstrapEmail}')),
                    '{passwordHashB64}',
                    '{passwordSaltB64}',
                    10,
                    10,
                    'Administrator',
                    NULL,
                    NULL,
                    0,
                    NULL,
                    NULL,
                    TIMESTAMPTZ '2026-05-13 11:10:17+00',
                    NULL,
                    NULL,
                    TIMESTAMPTZ '2026-05-13 11:10:17+00',
                    NULL,
                    TIMESTAMPTZ '2026-05-13 11:10:17+00',
                    NULL,
                    'Migration.SeedBootstrapAdminSystemUser',
                    NULL,
                    NULL,
                    FALSE,
                    NULL,
                    NULL
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "SystemUser" u
                    WHERE lower(trim(u."Email")) = lower(trim('{bootstrapEmail}'))
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "SystemUser"
                WHERE lower(trim("Email")) = lower(trim('admin@adaisakademi.dev'))
                  AND "Type" = 10;
                """);
        }
    }
}
