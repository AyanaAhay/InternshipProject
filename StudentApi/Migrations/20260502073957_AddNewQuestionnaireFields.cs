using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNewQuestionnaireFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "DesiredPracticeAreaIds",
                table: "Questionnaire",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherDesiredPracticeArea",
                table: "Questionnaire",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PracticeWishes",
                table: "Questionnaire",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThesisTopic",
                table: "Questionnaire",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatToLearn",
                table: "Questionnaire",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "PlacePractice",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseNumber",
                table: "Education",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredPracticeAreaIds",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "OtherDesiredPracticeArea",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "PracticeWishes",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "ThesisTopic",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "WhatToLearn",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "PlacePractice");

            migrationBuilder.DropColumn(
                name: "CourseNumber",
                table: "Education");
        }
    }
}
