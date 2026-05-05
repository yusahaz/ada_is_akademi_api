using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WorkerExpectedSalaryPerBoundCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Worker",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 13)
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Worker",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean")
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedSalaryMinAmount",
                table: "Worker",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Worker",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 16)
                .OldAnnotation("Relational:ColumnOrder", 15);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 15)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Worker",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AddColumn<string>(
                name: "ExpectedSalaryMaxCurrency",
                table: "Worker",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AddColumn<string>(
                name: "ExpectedSalaryMinCurrency",
                table: "Worker",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.Sql("""
                UPDATE "Worker"
                SET "ExpectedSalaryMinCurrency" = "ExpectedSalaryCurrencyCode"
                WHERE "ExpectedSalaryMinAmount" IS NOT NULL AND "ExpectedSalaryCurrencyCode" IS NOT NULL;

                UPDATE "Worker"
                SET "ExpectedSalaryMaxCurrency" = "ExpectedSalaryCurrencyCode"
                WHERE "ExpectedSalaryMaxAmount" IS NOT NULL AND "ExpectedSalaryCurrencyCode" IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "ExpectedSalaryCurrencyCode",
                table: "Worker");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedSalaryMaxCurrency",
                table: "Worker");

            migrationBuilder.DropColumn(
                name: "ExpectedSalaryMinCurrency",
                table: "Worker");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Worker",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Worker",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean")
                .Annotation("Relational:ColumnOrder", 13)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedSalaryMinAmount",
                table: "Worker",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Worker",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 15)
                .OldAnnotation("Relational:ColumnOrder", 16);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 15);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Worker",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AddColumn<string>(
                name: "ExpectedSalaryCurrencyCode",
                table: "Worker",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 6);
        }
    }
}
