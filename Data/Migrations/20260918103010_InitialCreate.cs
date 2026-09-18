using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TertiaryInstitutions.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Learners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Track = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learners", x => x.Id);
                    table.CheckConstraint("CK_Learner_Grade_Range", "\"Grade\" >= 8 AND \"Grade\" <= 12");
                });

            migrationBuilder.CreateTable(
                name: "AssessmentSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RealisticScore = table.Column<int>(type: "integer", nullable: false),
                    InvestigativeScore = table.Column<int>(type: "integer", nullable: false),
                    ArtisticScore = table.Column<int>(type: "integer", nullable: false),
                    SocialScore = table.Column<int>(type: "integer", nullable: false),
                    EnterprisingScore = table.Column<int>(type: "integer", nullable: false),
                    ConventionalScore = table.Column<int>(type: "integer", nullable: false),
                    ResultCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentSubmissions_Learners_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Learners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentSubmissions_LearnerId",
                table: "AssessmentSubmissions",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Learners_Email",
                table: "Learners",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentSubmissions");

            migrationBuilder.DropTable(
                name: "Learners");
        }
    }
}
