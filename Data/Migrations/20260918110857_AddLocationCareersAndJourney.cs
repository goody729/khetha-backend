using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TertiaryInstitutions.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationCareersAndJourney : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Learners",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Learners",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JourneyProgresses",
                columns: table => new
                {
                    LearnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExploreCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssessCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ShortlistCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApplyCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EnrollCompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyProgresses", x => x.LearnerId);
                    table.ForeignKey(
                        name: "FK_JourneyProgresses_Learners_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Learners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedCareers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CareerId = table.Column<int>(type: "integer", nullable: false),
                    SavedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedCareers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedCareers_Learners_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Learners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedCareers_LearnerId_CareerId",
                table: "SavedCareers",
                columns: new[] { "LearnerId", "CareerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JourneyProgresses");

            migrationBuilder.DropTable(
                name: "SavedCareers");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Learners");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Learners");
        }
    }
}
