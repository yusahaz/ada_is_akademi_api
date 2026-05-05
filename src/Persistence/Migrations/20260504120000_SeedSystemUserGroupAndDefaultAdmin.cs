using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AdaIsAkademiDbContext))]
    [Migration("20260504120000_SeedSystemUserGroupAndDefaultAdmin")]
    public partial class SeedSystemUserGroupAndDefaultAdmin : Migration
    {
        #region Utils

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "SystemUserGroupMembership"
                WHERE "SystemUserId" IN (
                          SELECT "Id"
                          FROM "SystemUser"
                          WHERE "Email" = 'admin@adaisakademi.local'
                      )
                  AND "SystemUserGroupId" IN (
                          SELECT "Id"
                          FROM "SystemUserGroup"
                          WHERE "Name" = 'Default Admin'
                      );
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM "SystemUser"
                WHERE "Email" = 'admin@adaisakademi.local';
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM "SystemUserGroup"
                WHERE "Name" = 'Default Admin';
                """);
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO "SystemUserGroup"
                    ("Name", "Description", "Level", "IsSystem", "IsActive", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted", "DeletedAt", "DeletedBy")
                SELECT
                    'Default Admin',
                    'Default global administrator group seeded by migration.',
                    0,
                    TRUE,
                    TRUE,
                    NOW(),
                    'migration',
                    NULL,
                    NULL,
                    FALSE,
                    NULL,
                    NULL
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "SystemUserGroup"
                    WHERE "Name" = 'Default Admin'
                );
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO "SystemUser"
                    ("Email", "PasswordHash", "PasswordSalt", "Type", "AccountStatus", "FirstName", "LastName", "Phone", "FailedLoginAttempts", "LastFailedLoginAt", "LastSuccessfulLoginAt", "LastPasswordChangeAt", "EmailVerificationToken", "EmailVerificationExpiresAt", "EmailVerifiedAt", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted", "DeletedAt", "DeletedBy")
                SELECT
                    'admin@adaisakademi.local',
                    '1XkwIXbHv0sNIdWBFF0n4Y9S4oAtHDhCdXJP23nnfMs=',
                    'FDubF1AS7z8DvO5oFr01CA==',
                    10,
                    10,
                    'Default',
                    'Admin',
                    NULL,
                    0,
                    NULL,
                    NULL,
                    NOW(),
                    NULL,
                    NULL,
                    NOW(),
                    NOW(),
                    'migration',
                    NULL,
                    NULL,
                    FALSE,
                    NULL,
                    NULL
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "SystemUser"
                    WHERE "Email" = 'admin@adaisakademi.local'
                );
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO "SystemUserGroupMembership"
                    ("SystemUserGroupId", "SystemUserId", "ScopeType", "ScopeId", "IsActive")
                SELECT
                    g."Id",
                    u."Id",
                    0,
                    NULL,
                    TRUE
                FROM "SystemUserGroup" g
                CROSS JOIN "SystemUser" u
                WHERE g."Name" = 'Default Admin'
                  AND u."Email" = 'admin@adaisakademi.local'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "SystemUserGroupMembership" m
                      WHERE m."SystemUserGroupId" = g."Id"
                        AND m."SystemUserId" = u."Id"
                        AND m."ScopeType" = 0
                        AND m."ScopeId" IS NULL
                  );
                """);
        }

        #endregion Utils
    }
}
