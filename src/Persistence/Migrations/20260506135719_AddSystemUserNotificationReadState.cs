using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemUserNotificationReadState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "SystemUserNotificationDispatch",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 14);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReadAt",
                table: "SystemUserNotificationDispatch",
                type: "timestamp with time zone",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 15);

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserNotificationDispatch_SystemUserId_IsRead_CreatedAt",
                table: "SystemUserNotificationDispatch",
                columns: new[] { "SystemUserId", "IsRead", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SystemUserNotificationDispatch_SystemUserId_IsRead_CreatedAt",
                table: "SystemUserNotificationDispatch");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "SystemUserNotificationDispatch");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "SystemUserNotificationDispatch");
        }
    }
}
