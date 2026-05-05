using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerPayoutAndCommissionAudit : Migration
    {
        #region Utils

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommissionAuditLog");

            migrationBuilder.DropTable(
                name: "WorkerPayout");

            migrationBuilder.DropColumn(
                name: "AnomalyCode",
                table: "ShiftAssignment");

            migrationBuilder.DropColumn(
                name: "IsAnomalyFlagged",
                table: "ShiftAssignment");

            migrationBuilder.DropColumn(
                name: "SupervisorCheckInTokenHash",
                table: "ShiftAssignment");

            migrationBuilder.DropColumn(
                name: "SupervisorCheckedInAt",
                table: "ShiftAssignment");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ShiftAssignment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CheckedOutAt",
                table: "ShiftAssignment",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CheckedInAt",
                table: "ShiftAssignment",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "AssignedAt",
                table: "ShiftAssignment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 7);
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ShiftAssignment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CheckedOutAt",
                table: "ShiftAssignment",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CheckedInAt",
                table: "ShiftAssignment",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "AssignedAt",
                table: "ShiftAssignment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AddColumn<string>(
                name: "AnomalyCode",
                table: "ShiftAssignment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnomalyFlagged",
                table: "ShiftAssignment",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorCheckInTokenHash",
                table: "ShiftAssignment",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SupervisorCheckedInAt",
                table: "ShiftAssignment",
                type: "timestamp with time zone",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 10);

            migrationBuilder.CreateTable(
                name: "WorkerPayout",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssignmentId = table.Column<int>(type: "integer", nullable: false),
                    EmployerId = table.Column<int>(type: "integer", nullable: false),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessingMarkedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ConfirmationDueAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastFailureReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CommissionAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionCurrency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossCurrency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetCurrency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerPayout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerPayout_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkerPayout_ShiftAssignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "ShiftAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkerPayout_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommissionAuditLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployerId = table.Column<int>(type: "integer", nullable: false),
                    AssignmentId = table.Column<int>(type: "integer", nullable: true),
                    CommissionReceivableId = table.Column<int>(type: "integer", nullable: true),
                    WorkerPayoutId = table.Column<int>(type: "integer", nullable: true),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionAuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommissionAuditLog_CommissionReceivable_CommissionReceivabl~",
                        column: x => x.CommissionReceivableId,
                        principalTable: "CommissionReceivable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionAuditLog_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionAuditLog_ShiftAssignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "ShiftAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionAuditLog_WorkerPayout_WorkerPayoutId",
                        column: x => x.WorkerPayoutId,
                        principalTable: "WorkerPayout",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionAuditLog_AssignmentId",
                table: "CommissionAuditLog",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionAuditLog_CommissionReceivableId",
                table: "CommissionAuditLog",
                column: "CommissionReceivableId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionAuditLog_EmployerId_CreatedAt",
                table: "CommissionAuditLog",
                columns: new[] { "EmployerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionAuditLog_WorkerPayoutId",
                table: "CommissionAuditLog",
                column: "WorkerPayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerPayout_AssignmentId",
                table: "WorkerPayout",
                column: "AssignmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerPayout_EmployerId_Status",
                table: "WorkerPayout",
                columns: new[] { "EmployerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkerPayout_WorkerId_Status",
                table: "WorkerPayout",
                columns: new[] { "WorkerId", "Status" });
        }

        #endregion Utils
    }
}
