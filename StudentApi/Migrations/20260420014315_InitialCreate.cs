using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudentApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    IdStudent = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Patronymic = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Birthdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Login = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HadLamaPractice = table.Column<bool>(type: "boolean", nullable: false),
                    IsLamaEmployee = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.IdStudent);
                });

            migrationBuilder.CreateTable(
                name: "Questionnaire",
                columns: table => new
                {
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<int>(type: "integer", nullable: false),
                    Citizenship = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Birthplace = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SourceInfo = table.Column<string>(type: "text", nullable: false),
                    Residency = table.Column<string>(type: "text", nullable: false),
                    RegistrationPlace = table.Column<string>(type: "text", nullable: false),
                    VacationSideJob = table.Column<bool>(type: "boolean", nullable: false),
                    VolunteeringReadiness = table.Column<bool>(type: "boolean", nullable: false),
                    CriminalLiability = table.Column<bool>(type: "boolean", nullable: false),
                    AdminLiability = table.Column<bool>(type: "boolean", nullable: false),
                    ChronicConditions = table.Column<string>(type: "text", nullable: true),
                    MedContraindications = table.Column<string>(type: "text", nullable: true),
                    DataProcessingConsent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaire", x => x.IdQuestionnaire);
                    table.ForeignKey(
                        name: "FK_Questionnaire_Student_IdStudent",
                        column: x => x.IdStudent,
                        principalTable: "Student",
                        principalColumn: "IdStudent",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Education",
                columns: table => new
                {
                    IdEducation = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: false),
                    DegreeOfEducation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EducationalInstitution = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Faculty = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Specialization = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EducationStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    EducationEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    GroupNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SurnameTutor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NameTutor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PatronymicTutor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education", x => x.IdEducation);
                    table.ForeignKey(
                        name: "FK_Education_Questionnaire_IdQuestionnaire",
                        column: x => x.IdQuestionnaire,
                        principalTable: "Questionnaire",
                        principalColumn: "IdQuestionnaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlacePractice",
                columns: table => new
                {
                    IdPlacePractice = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: false),
                    OrganizationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PracticeStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    PracticeEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    MainFunctions = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacePractice", x => x.IdPlacePractice);
                    table.ForeignKey(
                        name: "FK_PlacePractice_Questionnaire_IdQuestionnaire",
                        column: x => x.IdQuestionnaire,
                        principalTable: "Questionnaire",
                        principalColumn: "IdQuestionnaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceWork",
                columns: table => new
                {
                    IdPlaceWork = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: false),
                    OrganizationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    WorkStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    WorkEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Position = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MainFunctions = table.Column<string>(type: "text", nullable: true),
                    ReasonForDismissal = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceWork", x => x.IdPlaceWork);
                    table.ForeignKey(
                        name: "FK_PlaceWork_Questionnaire_IdQuestionnaire",
                        column: x => x.IdQuestionnaire,
                        principalTable: "Questionnaire",
                        principalColumn: "IdQuestionnaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticePriority",
                columns: table => new
                {
                    IdPracticePriority = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: false),
                    Wording = table.Column<string>(type: "text", nullable: false),
                    Estimation = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticePriority", x => x.IdPracticePriority);
                    table.ForeignKey(
                        name: "FK_PracticePriority_Questionnaire_IdQuestionnaire",
                        column: x => x.IdQuestionnaire,
                        principalTable: "Questionnaire",
                        principalColumn: "IdQuestionnaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PsychologicalQuestions",
                columns: table => new
                {
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: false),
                    LateInstances = table.Column<string>(type: "text", nullable: true),
                    ValuedQualities = table.Column<string>(type: "text", nullable: true),
                    UnacceptableQualities = table.Column<string>(type: "text", nullable: true),
                    Friendliness = table.Column<string>(type: "text", nullable: true),
                    SubordinateAction = table.Column<string>(type: "text", nullable: true),
                    WorkTimeDedication = table.Column<string>(type: "text", nullable: true),
                    StressfulWorkReadiness = table.Column<string>(type: "text", nullable: true),
                    DisciplineImportance = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsychologicalQuestions", x => x.IdQuestionnaire);
                    table.ForeignKey(
                        name: "FK_PsychologicalQuestions_Questionnaire_IdQuestionnaire",
                        column: x => x.IdQuestionnaire,
                        principalTable: "Questionnaire",
                        principalColumn: "IdQuestionnaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Relative",
                columns: table => new
                {
                    IdRelative = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: false),
                    RelationDegree = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Patronymic = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Birthdate = table.Column<DateTime>(type: "date", nullable: true),
                    PlaceStudy = table.Column<string>(type: "text", nullable: true),
                    PlaceWork = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relative", x => x.IdRelative);
                    table.ForeignKey(
                        name: "FK_Relative_Questionnaire_IdQuestionnaire",
                        column: x => x.IdQuestionnaire,
                        principalTable: "Questionnaire",
                        principalColumn: "IdQuestionnaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    IdSkill = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: false),
                    SkillName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.IdSkill);
                    table.ForeignKey(
                        name: "FK_Skill_Questionnaire_IdQuestionnaire",
                        column: x => x.IdQuestionnaire,
                        principalTable: "Questionnaire",
                        principalColumn: "IdQuestionnaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentApplication",
                columns: table => new
                {
                    IdStudentApplication = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<int>(type: "integer", nullable: false),
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: true),
                    IdScheduledPractice = table.Column<int>(type: "integer", nullable: true),
                    IdPracticeType = table.Column<int>(type: "integer", nullable: true),
                    IdSpecialization = table.Column<int>(type: "integer", nullable: true),
                    StudentApplicationStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentApplication", x => x.IdStudentApplication);
                    table.ForeignKey(
                        name: "FK_StudentApplication_Questionnaire_IdQuestionnaire",
                        column: x => x.IdQuestionnaire,
                        principalTable: "Questionnaire",
                        principalColumn: "IdQuestionnaire",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StudentApplication_Student_IdStudent",
                        column: x => x.IdStudent,
                        principalTable: "Student",
                        principalColumn: "IdStudent",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentProject",
                columns: table => new
                {
                    IdStudentProject = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuestionnaire = table.Column<int>(type: "integer", nullable: false),
                    ProjectName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DateParticipation = table.Column<DateTime>(type: "date", nullable: true),
                    Organizer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsOurOrganizationEvent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProject", x => x.IdStudentProject);
                    table.ForeignKey(
                        name: "FK_StudentProject_Questionnaire_IdQuestionnaire",
                        column: x => x.IdQuestionnaire,
                        principalTable: "Questionnaire",
                        principalColumn: "IdQuestionnaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Education_IdQuestionnaire",
                table: "Education",
                column: "IdQuestionnaire");

            migrationBuilder.CreateIndex(
                name: "IX_PlacePractice_IdQuestionnaire",
                table: "PlacePractice",
                column: "IdQuestionnaire");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceWork_IdQuestionnaire",
                table: "PlaceWork",
                column: "IdQuestionnaire");

            migrationBuilder.CreateIndex(
                name: "IX_PracticePriority_IdQuestionnaire",
                table: "PracticePriority",
                column: "IdQuestionnaire");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaire_IdStudent",
                table: "Questionnaire",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_Relative_IdQuestionnaire",
                table: "Relative",
                column: "IdQuestionnaire");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_IdQuestionnaire",
                table: "Skill",
                column: "IdQuestionnaire");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Login",
                table: "Student",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentApplication_IdQuestionnaire",
                table: "StudentApplication",
                column: "IdQuestionnaire");

            migrationBuilder.CreateIndex(
                name: "IX_StudentApplication_IdStudent",
                table: "StudentApplication",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProject_IdQuestionnaire",
                table: "StudentProject",
                column: "IdQuestionnaire");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Education");

            migrationBuilder.DropTable(
                name: "PlacePractice");

            migrationBuilder.DropTable(
                name: "PlaceWork");

            migrationBuilder.DropTable(
                name: "PracticePriority");

            migrationBuilder.DropTable(
                name: "PsychologicalQuestions");

            migrationBuilder.DropTable(
                name: "Relative");

            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropTable(
                name: "StudentApplication");

            migrationBuilder.DropTable(
                name: "StudentProject");

            migrationBuilder.DropTable(
                name: "Questionnaire");

            migrationBuilder.DropTable(
                name: "Student");
        }
    }
}
