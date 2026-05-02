using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentDocument",
                columns: table => new
                {
                    IdStudentDocument = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudentApplication = table.Column<int>(type: "integer", nullable: false),
                    IdStudent = table.Column<int>(type: "integer", nullable: false),
                    IdDocumentType = table.Column<int>(type: "integer", nullable: true),
                    IdSpecialization = table.Column<int>(type: "integer", nullable: true),
                    UploadStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    VerificationStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ContractStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocument", x => x.IdStudentDocument);
                    table.ForeignKey(
                        name: "FK_StudentDocument_StudentApplication_IdStudentApplication",
                        column: x => x.IdStudentApplication,
                        principalTable: "StudentApplication",
                        principalColumn: "IdStudentApplication",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentDocument_Student_IdStudent",
                        column: x => x.IdStudent,
                        principalTable: "Student",
                        principalColumn: "IdStudent",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocument_IdStudent",
                table: "StudentDocument",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocument_IdStudentApplication",
                table: "StudentDocument",
                column: "IdStudentApplication");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentDocument");
        }
    }
}
