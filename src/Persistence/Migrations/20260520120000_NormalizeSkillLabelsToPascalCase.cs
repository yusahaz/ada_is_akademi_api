using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeSkillLabelsToPascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // WorkerSkill / JobPostingSkill: UPPER/underscore tags -> space-separated PascalCase (InvariantCulture-style via initcap).
            migrationBuilder.Sql(
                """
                UPDATE "WorkerSkill"
                SET "Tag" = regexp_replace(
                    initcap(lower(regexp_replace(btrim("Tag"), '[_\-\/]+', ' ', 'g'))),
                    '\s+',
                    ' ',
                    'g')
                WHERE btrim("Tag") <> '';
                """);

            migrationBuilder.Sql(
                """
                UPDATE "JobPostingSkill"
                SET "Tag" = regexp_replace(
                    initcap(lower(regexp_replace(btrim("Tag"), '[_\-\/]+', ' ', 'g'))),
                    '\s+',
                    ' ',
                    'g')
                WHERE btrim("Tag") <> '';
                """);

            // JobSkill dictionary: only rows that are not already mixed-case compounds (seed PascalCase names are left intact).
            migrationBuilder.Sql(
                """
                UPDATE "JobSkill"
                SET "Name" = regexp_replace(
                    initcap(lower(regexp_replace(btrim("Name"), '[_\-\/]+', ' ', 'g'))),
                    '\s+',
                    '',
                    'g')
                WHERE btrim("Name") <> ''
                  AND (
                    ("Name" = upper("Name") AND "Name" !~ '[a-z]')
                    OR ("Name" ~ '[_\-\/]' AND "Name" !~ '[a-z]')
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Irreversible data normalization.
        }
    }
}
