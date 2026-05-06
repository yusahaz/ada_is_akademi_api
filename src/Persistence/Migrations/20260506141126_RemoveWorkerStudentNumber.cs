using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWorkerStudentNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Worker"
                DROP COLUMN IF EXISTS "StudentNumber";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentNumber",
                table: "Worker",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 6);
        }
    }
}
