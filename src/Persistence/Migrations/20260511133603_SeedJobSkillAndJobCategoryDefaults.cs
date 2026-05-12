using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Azoxia.AdaIsAkademi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedJobSkillAndJobCategoryDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DateTimeOffset seededAt = new DateTimeOffset(2026, 5, 11, 13, 36, 3, TimeSpan.Zero);
            const string seededBy = "Migration.SeedJobSkillAndJobCategoryDefaults";

            migrationBuilder.CreateTable(
                name: "JobSkill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_JobSkill", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_CreatedAt",
                table: "JobSkill",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_Name",
                table: "JobSkill",
                column: "Name");

            migrationBuilder.InsertData(
                table: "JobCategory",
                columns: new[] { "Id", "Name", "Description", "ParentId", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted", "DeletedAt", "DeletedBy" },
                values: new object[,]
                {
                    { 1001, "HotelOperations", "Core operations for hotel service delivery.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1002, "FrontOffice", "Reception and check-in/check-out operations.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1003, "Housekeeping", "Room cleaning and readiness operations.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1004, "FoodAndBeverageService", "Guest-facing food and beverage service.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1005, "KitchenProduction", "Back-of-house food production workflows.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1006, "BarOperations", "Bar setup, beverage service, and closing.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1007, "CafeOperations", "Daily cafe workflow and counter service.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1008, "RestaurantOperations", "Dining room and service floor management.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1009, "BanquetAndEvents", "Event, banquet, and group service operations.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1010, "ClubOperations", "Night venue and entertainment operations.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1011, "ConciergeServices", "Guest support and local arrangement services.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1012, "GuestRelations", "Guest satisfaction and issue resolution.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1013, "ReservationAndRevenue", "Reservation flow and revenue controls.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1014, "Stewarding", "Dishwashing and kitchen hygiene support.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1015, "PastryAndBakery", "Dessert and bakery production operations.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1016, "FacilityMaintenance", "Technical maintenance and repairs.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1017, "SecurityAndSafety", "Safety, security, and incident response.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1018, "SpaAndWellness", "Spa, wellness, and treatment operations.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1019, "ProcurementAndInventory", "Supply sourcing and stock governance.", null, seededAt, seededBy, null, null, false, null, null },
                    { 1020, "NightAudit", "Night shift reconciliation and reporting.", null, seededAt, seededBy, null, null, false, null, null },
                });

            migrationBuilder.InsertData(
                table: "JobSkill",
                columns: new[] { "Id", "Name", "Description", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted", "DeletedAt", "DeletedBy" },
                values: new object[,]
                {
                    { 2001, "GuestCommunication", "Professional guest-facing communication.", seededAt, seededBy, null, null, false, null, null },
                    { 2002, "TableService", "Standard table service execution.", seededAt, seededBy, null, null, false, null, null },
                    { 2003, "TrayService", "Safe and efficient tray handling.", seededAt, seededBy, null, null, false, null, null },
                    { 2004, "OrderTaking", "Accurate and complete order capture.", seededAt, seededBy, null, null, false, null, null },
                    { 2005, "PointOfSaleOperation", "POS terminal operation and troubleshooting.", seededAt, seededBy, null, null, false, null, null },
                    { 2006, "CashHandling", "Cash drawer and payment accuracy.", seededAt, seededBy, null, null, false, null, null },
                    { 2007, "CocktailPreparation", "Classic and signature cocktail preparation.", seededAt, seededBy, null, null, false, null, null },
                    { 2008, "EspressoPreparation", "Espresso-based beverage preparation.", seededAt, seededBy, null, null, false, null, null },
                    { 2009, "WineService", "Wine presentation and serving etiquette.", seededAt, seededBy, null, null, false, null, null },
                    { 2010, "FoodSafety", "Operational food hygiene and safety.", seededAt, seededBy, null, null, false, null, null },
                    { 2011, "HACCPCompliance", "HACCP checklist and control point compliance.", seededAt, seededBy, null, null, false, null, null },
                    { 2012, "KitchenPrep", "Ingredient preparation for service periods.", seededAt, seededBy, null, null, false, null, null },
                    { 2013, "ColdSectionPreparation", "Cold section and garde-manger prep.", seededAt, seededBy, null, null, false, null, null },
                    { 2014, "HotSectionCooking", "Hot line execution and station control.", seededAt, seededBy, null, null, false, null, null },
                    { 2015, "Grilling", "Protein and vegetable grill execution.", seededAt, seededBy, null, null, false, null, null },
                    { 2016, "Frying", "Fryer operation and oil quality handling.", seededAt, seededBy, null, null, false, null, null },
                    { 2017, "PlatingPresentation", "Consistent plating and visual standards.", seededAt, seededBy, null, null, false, null, null },
                    { 2018, "HousekeepingStandards", "Room and public-area cleaning standards.", seededAt, seededBy, null, null, false, null, null },
                    { 2019, "RoomTurnover", "Fast and complete room turnover routines.", seededAt, seededBy, null, null, false, null, null },
                    { 2020, "LaundryOperations", "Linen sorting, washing, and handling.", seededAt, seededBy, null, null, false, null, null },
                    { 2021, "InventoryControl", "Count, variance, and stock control.", seededAt, seededBy, null, null, false, null, null },
                    { 2022, "StockReceiving", "Goods receiving and quality checks.", seededAt, seededBy, null, null, false, null, null },
                    { 2023, "ReservationManagement", "Reservation system and booking flows.", seededAt, seededBy, null, null, false, null, null },
                    { 2024, "ComplaintResolution", "Structured guest complaint handling.", seededAt, seededBy, null, null, false, null, null },
                    { 2025, "UpsellingTechniques", "Contextual upsell and cross-sell skills.", seededAt, seededBy, null, null, false, null, null },
                    { 2026, "BanquetSetup", "Banquet hall setup and service readiness.", seededAt, seededBy, null, null, false, null, null },
                    { 2027, "EventCoordination", "Service-side event coordination.", seededAt, seededBy, null, null, false, null, null },
                    { 2028, "ShiftPlanning", "Shift handover and run-sheet planning.", seededAt, seededBy, null, null, false, null, null },
                    { 2029, "TeamLeadership", "Floor leadership and team guidance.", seededAt, seededBy, null, null, false, null, null },
                    { 2030, "FirstAidBasics", "Basic first-aid awareness and response.", seededAt, seededBy, null, null, false, null, null },
                    { 2031, "FireSafetyAwareness", "Fire safety and emergency protocol awareness.", seededAt, seededBy, null, null, false, null, null },
                    { 2032, "ClosingProcedures", "End-of-shift closing and reconciliation.", seededAt, seededBy, null, null, false, null, null },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"JobCategory\" WHERE \"Id\" IN (1001,1002,1003,1004,1005,1006,1007,1008,1009,1010,1011,1012,1013,1014,1015,1016,1017,1018,1019,1020);");

            migrationBuilder.DropTable(
                name: "JobSkill");
        }
    }
}
