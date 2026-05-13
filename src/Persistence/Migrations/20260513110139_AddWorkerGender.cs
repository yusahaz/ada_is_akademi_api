using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Worker",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Worker");
        }
    }
}
