using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class ScaleInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RubricInformation",
                table: "Answers");

            migrationBuilder.AddColumn<string>(
                name: "RubricRaterScaleName",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScoreText",
                table: "RaterAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RubricRaterScaleName",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "ScoreText",
                table: "RaterAnswers");

            migrationBuilder.AddColumn<string>(
                name: "RubricInformation",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
