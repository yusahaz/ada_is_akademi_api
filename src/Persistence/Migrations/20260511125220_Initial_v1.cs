using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UtcTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Exception = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    EventId = table.Column<int>(type: "integer", nullable: true),
                    EventName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    LogoObjectKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Address_City = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Address_Country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Address_District = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Address_Line1 = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Address_Line2 = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Address_PostalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Contact_Email = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Contact_FirstName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Contact_LastName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Contact_Phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TaxNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCategory_JobCategory_ParentId",
                        column: x => x.ParentId,
                        principalTable: "JobCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommissionReceivable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployerId = table.Column<int>(type: "integer", nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionReceivable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommissionReceivable_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployerLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EmployerId = table.Column<int>(type: "integer", nullable: false),
                    GeofenceRadiusMetres = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Address_City = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Address_Country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Address_District = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Address_Line1 = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Address_Line2 = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Address_PostalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Contact_Email = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Contact_FirstName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Contact_LastName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Contact_Phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Coordinate_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Coordinate_Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployerLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployerLocation_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployerSocialLink",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployerId = table.Column<int>(type: "integer", nullable: false),
                    Platform = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployerSocialLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployerSocialLink_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    PasswordSalt = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    AccountStatus = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LastName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LastFailedLoginAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastSuccessfulLoginAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastPasswordChangeAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EmailVerificationToken = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EmailVerificationExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EmailVerifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EmployerId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemUser_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "JobPosting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployerId = table.Column<int>(type: "integer", nullable: false),
                    EmployerLocationId = table.Column<int>(type: "integer", nullable: false),
                    JobCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "character varying(16384)", maxLength: 16384, nullable: false),
                    ShiftDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ShiftStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ShiftEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    HeadCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DescriptionEmbedding = table.Column<float[]>(type: "real[]", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    WageAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    WageCurrency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPosting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPosting_EmployerLocation_EmployerLocationId",
                        column: x => x.EmployerLocationId,
                        principalTable: "EmployerLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPosting_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPosting_JobCategory_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalTable: "JobCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Supervisor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployerId = table.Column<int>(type: "integer", nullable: false),
                    SystemUserId = table.Column<int>(type: "integer", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supervisor_EmployerLocation_LocationId",
                        column: x => x.LocationId,
                        principalTable: "EmployerLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Supervisor_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Supervisor_SystemUser_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemUserDevice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SystemUserId = table.Column<int>(type: "integer", nullable: false),
                    DeviceIdentifier = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Platform = table.Column<int>(type: "integer", nullable: false),
                    DeviceToken = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    LastActiveAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUserDevice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemUserDevice_SystemUser_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SystemUserId = table.Column<int>(type: "integer", nullable: false),
                    Bio = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    ProfilePhotoObjectKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Nationality = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    University = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CvTemplateName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SkillEmbedding = table.Column<float[]>(type: "real[]", nullable: true),
                    EmbeddingUpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpectedSalaryMinAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ExpectedSalaryMinCurrency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    ExpectedSalaryMaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ExpectedSalaryMaxCurrency = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Worker_SystemUser_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobPostingSkill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobPostingId = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Tag = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostingSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostingSkill_JobPosting_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverdueJobAlarm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobPostingId = table.Column<int>(type: "integer", nullable: false),
                    AlarmDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverdueJobAlarm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OverdueJobAlarm_JobPosting_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemUserRefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SystemUserId = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUserRefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemUserRefreshToken_SystemUserDevice_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "SystemUserDevice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemUserRefreshToken_SystemUser_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CvUploadSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FileFormat = table.Column<int>(type: "integer", nullable: false),
                    ObjectKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExtractionRequestedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExtractionCompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ExtractedJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvUploadSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvUploadSession_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployerWorkerProfileViewStat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployerId = table.Column<int>(type: "integer", nullable: false),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    TotalViews = table.Column<int>(type: "integer", nullable: false),
                    LastRecordedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployerWorkerProfileViewStat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployerWorkerProfileViewStat_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployerWorkerProfileViewStat_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobPostingId = table.Column<int>(type: "integer", nullable: false),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AppliedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplication_JobPosting_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplication_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemUserNotificationDispatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: true),
                    SystemUserId = table.Column<int>(type: "integer", nullable: false),
                    JobPostingId = table.Column<int>(type: "integer", nullable: true),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TemplateCode = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Title = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    FallbackReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastAttemptAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUserNotificationDispatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemUserNotificationDispatch_JobPosting_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SystemUserNotificationDispatch_SystemUser_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SystemUserNotificationDispatch_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkerAvailability",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    TimeFrom = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TimeTo = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerAvailability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerAvailability_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerCertificate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    IssuingOrganization = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    IssuedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpiresAt = table.Column<DateOnly>(type: "date", nullable: true),
                    DocumentUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerCertificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerCertificate_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerEducation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    School = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Department = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    EducationType = table.Column<int>(type: "integer", nullable: false),
                    StartYear = table.Column<int>(type: "integer", nullable: false),
                    EndYear = table.Column<int>(type: "integer", nullable: true),
                    IsOngoing = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerEducation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerEducation_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerExperience",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Position = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerExperience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerExperience_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerInterestedJobCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    JobCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerInterestedJobCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerInterestedJobCategory_JobCategory_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalTable: "JobCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkerInterestedJobCategory_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerLanguage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerLanguage_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerReference",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    Company = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Position = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Contact_Email = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Contact_FirstName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Contact_LastName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Contact_Phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerReference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerReference_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerSkill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Tag = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerSkill_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "ShiftAssignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobPostingId = table.Column<int>(type: "integer", nullable: false),
                    JobApplicationId = table.Column<int>(type: "integer", nullable: false),
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    CheckInTokenHash = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    SupervisorCheckInTokenHash = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CheckedInAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CheckedOutAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SupervisorCheckedInAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AnomalyCode = table.Column<string>(type: "text", nullable: true),
                    IsAnomalyFlagged = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftAssignment_JobApplication_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftAssignment_JobPosting_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftAssignment_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_AppLogs_UtcTimestamp",
                table: "AppLogs",
                column: "UtcTimestamp");

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
                name: "IX_CommissionReceivable_EmployerId_PeriodStart_PeriodEnd",
                table: "CommissionReceivable",
                columns: new[] { "EmployerId", "PeriodStart", "PeriodEnd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CvUploadSession_ObjectKey",
                table: "CvUploadSession",
                column: "ObjectKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CvUploadSession_WorkerId_Status_CreatedAt",
                table: "CvUploadSession",
                columns: new[] { "WorkerId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Employer_CreatedAt",
                table: "Employer",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Employer_Name",
                table: "Employer",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_EmployerLocation_CreatedAt",
                table: "EmployerLocation",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmployerLocation_EmployerId",
                table: "EmployerLocation",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployerLocation_Name",
                table: "EmployerLocation",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_EmployerSocialLink_EmployerId_Platform",
                table: "EmployerSocialLink",
                columns: new[] { "EmployerId", "Platform" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployerWorkerProfileViewStat_EmployerId_WorkerId",
                table: "EmployerWorkerProfileViewStat",
                columns: new[] { "EmployerId", "WorkerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployerWorkerProfileViewStat_WorkerId",
                table: "EmployerWorkerProfileViewStat",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_JobPostingId_WorkerId",
                table: "JobApplication",
                columns: new[] { "JobPostingId", "WorkerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_WorkerId_Status",
                table: "JobApplication",
                columns: new[] { "WorkerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_CreatedAt",
                table: "JobCategory",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_Name",
                table: "JobCategory",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_ParentId",
                table: "JobCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosting_CreatedAt",
                table: "JobPosting",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JobPosting_EmployerId_Status",
                table: "JobPosting",
                columns: new[] { "EmployerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_JobPosting_EmployerLocationId_ShiftDate",
                table: "JobPosting",
                columns: new[] { "EmployerLocationId", "ShiftDate" });

            migrationBuilder.CreateIndex(
                name: "IX_JobPosting_JobCategoryId",
                table: "JobPosting",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostingSkill_JobPostingId",
                table: "JobPostingSkill",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_OverdueJobAlarm_AlarmDate",
                table: "OverdueJobAlarm",
                column: "AlarmDate");

            migrationBuilder.CreateIndex(
                name: "IX_OverdueJobAlarm_JobPostingId_AlarmDate",
                table: "OverdueJobAlarm",
                columns: new[] { "JobPostingId", "AlarmDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignment_JobApplicationId",
                table: "ShiftAssignment",
                column: "JobApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignment_JobPostingId",
                table: "ShiftAssignment",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignment_WorkerId_Status",
                table: "ShiftAssignment",
                columns: new[] { "WorkerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Supervisor_EmployerId_SystemUserId",
                table: "Supervisor",
                columns: new[] { "EmployerId", "SystemUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Supervisor_LocationId",
                table: "Supervisor",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Supervisor_SystemUserId",
                table: "Supervisor",
                column: "SystemUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUser_CreatedAt",
                table: "SystemUser",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUser_Email",
                table: "SystemUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemUser_EmployerId",
                table: "SystemUser",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserDevice_SystemUserId_DeviceIdentifier",
                table: "SystemUserDevice",
                columns: new[] { "SystemUserId", "DeviceIdentifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserNotificationDispatch_JobPostingId",
                table: "SystemUserNotificationDispatch",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserNotificationDispatch_SystemUserId_IsRead_CreatedAt",
                table: "SystemUserNotificationDispatch",
                columns: new[] { "SystemUserId", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserNotificationDispatch_SystemUserId_Status",
                table: "SystemUserNotificationDispatch",
                columns: new[] { "SystemUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserNotificationDispatch_WorkerId_Status_CreatedAt",
                table: "SystemUserNotificationDispatch",
                columns: new[] { "WorkerId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserRefreshToken_DeviceId",
                table: "SystemUserRefreshToken",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserRefreshToken_SystemUserId_DeviceId",
                table: "SystemUserRefreshToken",
                columns: new[] { "SystemUserId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserRefreshToken_SystemUserId_TokenHash",
                table: "SystemUserRefreshToken",
                columns: new[] { "SystemUserId", "TokenHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worker_CreatedAt",
                table: "Worker",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_SystemUserId",
                table: "Worker",
                column: "SystemUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerAvailability_WorkerId_DayOfWeek",
                table: "WorkerAvailability",
                columns: new[] { "WorkerId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkerCertificate_WorkerId",
                table: "WorkerCertificate",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEducation_WorkerId",
                table: "WorkerEducation",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerExperience_WorkerId",
                table: "WorkerExperience",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerInterestedJobCategory_JobCategoryId",
                table: "WorkerInterestedJobCategory",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerInterestedJobCategory_WorkerId_JobCategoryId",
                table: "WorkerInterestedJobCategory",
                columns: new[] { "WorkerId", "JobCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerLanguage_WorkerId_Language",
                table: "WorkerLanguage",
                columns: new[] { "WorkerId", "Language" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_WorkerReference_WorkerId",
                table: "WorkerReference",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerSkill_WorkerId",
                table: "WorkerSkill",
                column: "WorkerId");

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
                name: "AppLogs");

            migrationBuilder.DropTable(
                name: "CommissionAuditLog");

            migrationBuilder.DropTable(
                name: "CvUploadSession");

            migrationBuilder.DropTable(
                name: "EmployerSocialLink");

            migrationBuilder.DropTable(
                name: "EmployerWorkerProfileViewStat");

            migrationBuilder.DropTable(
                name: "JobPostingSkill");

            migrationBuilder.DropTable(
                name: "OverdueJobAlarm");

            migrationBuilder.DropTable(
                name: "Supervisor");

            migrationBuilder.DropTable(
                name: "SystemUserNotificationDispatch");

            migrationBuilder.DropTable(
                name: "SystemUserRefreshToken");

            migrationBuilder.DropTable(
                name: "WorkerAvailability");

            migrationBuilder.DropTable(
                name: "WorkerCertificate");

            migrationBuilder.DropTable(
                name: "WorkerEducation");

            migrationBuilder.DropTable(
                name: "WorkerExperience");

            migrationBuilder.DropTable(
                name: "WorkerInterestedJobCategory");

            migrationBuilder.DropTable(
                name: "WorkerLanguage");

            migrationBuilder.DropTable(
                name: "WorkerReference");

            migrationBuilder.DropTable(
                name: "WorkerSkill");

            migrationBuilder.DropTable(
                name: "WorkerSocialLink");

            migrationBuilder.DropTable(
                name: "CommissionReceivable");

            migrationBuilder.DropTable(
                name: "WorkerPayout");

            migrationBuilder.DropTable(
                name: "SystemUserDevice");

            migrationBuilder.DropTable(
                name: "ShiftAssignment");

            migrationBuilder.DropTable(
                name: "JobApplication");

            migrationBuilder.DropTable(
                name: "JobPosting");

            migrationBuilder.DropTable(
                name: "Worker");

            migrationBuilder.DropTable(
                name: "EmployerLocation");

            migrationBuilder.DropTable(
                name: "JobCategory");

            migrationBuilder.DropTable(
                name: "SystemUser");

            migrationBuilder.DropTable(
                name: "Employer");
        }
    }
}
