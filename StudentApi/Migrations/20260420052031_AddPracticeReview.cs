using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPracticeReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PracticeReview",
                columns: table => new
                {
                    IdPracticeReview = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudentApplication = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    ReadyToWork = table.Column<bool>(type: "boolean", nullable: false),
                    SpecialityRelevance = table.Column<int>(type: "integer", nullable: false),
                    SupervisionQuality = table.Column<int>(type: "integer", nullable: false),
                    ExperienceUsefulness = table.Column<int>(type: "integer", nullable: false),
                    OverallScore = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeReview", x => x.IdPracticeReview);
                    table.ForeignKey(
                        name: "FK_PracticeReview_StudentApplication_IdStudentApplication",
                        column: x => x.IdStudentApplication,
                        principalTable: "StudentApplication",
                        principalColumn: "IdStudentApplication",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PracticeReview_IdStudentApplication",
                table: "PracticeReview",
                column: "IdStudentApplication",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticeReview");
        }
    }
}
