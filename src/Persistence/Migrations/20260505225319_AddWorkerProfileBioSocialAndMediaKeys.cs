using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerProfileBioSocialAndMediaKeys : Migration
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
                .Annotation("Relational:ColumnOrder", 15)
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<string>(
                name: "University",
                table: "Worker",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<float[]>(
                name: "SkillEmbedding",
                table: "Worker",
                type: "real[]",
                nullable: true,
                oldClrType: typeof(float[]),
                oldType: "real[]",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "Worker",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Worker",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean")
                .Annotation("Relational:ColumnOrder", 16)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<string>(
                name: "ExpectedSalaryMinCurrency",
                table: "Worker",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedSalaryMinAmount",
                table: "Worker",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "ExpectedSalaryMaxCurrency",
                table: "Worker",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedSalaryMaxAmount",
                table: "Worker",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EmbeddingUpdatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 5);

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
                .Annotation("Relational:ColumnOrder", 18)
                .OldAnnotation("Relational:ColumnOrder", 16);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 17)
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
                .Annotation("Relational:ColumnOrder", 13)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 12)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Worker",
                type: "character varying(3000)",
                maxLength: 3000,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoObjectKey",
                table: "Worker",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Employer",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Employer",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Employer",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean")
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Employer",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Employer",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Employer",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Employer",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<string>(
                name: "LogoObjectKey",
                table: "Employer",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.CreateTable(
                name: "WorkerSocialLink",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    Platform = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerSocialLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerSocialLink_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkerSocialLink_WorkerId_Platform",
                table: "WorkerSocialLink",
                columns: new[] { "WorkerId", "Platform" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkerSocialLink");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Worker");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoObjectKey",
                table: "Worker");

            migrationBuilder.DropColumn(
                name: "LogoObjectKey",
                table: "Employer");

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
                .OldAnnotation("Relational:ColumnOrder", 15);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<string>(
                name: "University",
                table: "Worker",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 3)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<float[]>(
                name: "SkillEmbedding",
                table: "Worker",
                type: "real[]",
                nullable: true,
                oldClrType: typeof(float[]),
                oldType: "real[]",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "Worker",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Worker",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean")
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 16);

            migrationBuilder.AlterColumn<string>(
                name: "ExpectedSalaryMinCurrency",
                table: "Worker",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedSalaryMinAmount",
                table: "Worker",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<string>(
                name: "ExpectedSalaryMaxCurrency",
                table: "Worker",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedSalaryMaxAmount",
                table: "Worker",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EmbeddingUpdatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5)
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
                .OldAnnotation("Relational:ColumnOrder", 18);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 15)
                .OldAnnotation("Relational:ColumnOrder", 17);

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
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Worker",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Employer",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Employer",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Employer",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean")
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Employer",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Employer",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Employer",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Employer",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 6);
        }
    }
}
