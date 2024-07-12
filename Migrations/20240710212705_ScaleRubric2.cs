using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class ScaleRubric2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_RaterScales_RaterScaleId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_RaterScaleId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "RaterScaleDescription",
                table: "RaterScales");

            migrationBuilder.DropColumn(
                name: "RaterScaleTitle",
                table: "RaterScales");

            migrationBuilder.DropColumn(
                name: "RaterScaleId",
                table: "Questions");

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "RaterScales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "RaterScales",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaterScales_QuestionId",
                table: "RaterScales",
                column: "QuestionId",
                unique: true,
                filter: "[QuestionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_RaterScales_Questions_QuestionId",
                table: "RaterScales",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaterScales_Questions_QuestionId",
                table: "RaterScales");

            migrationBuilder.DropIndex(
                name: "IX_RaterScales_QuestionId",
                table: "RaterScales");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "RaterScales");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "RaterScales");

            migrationBuilder.AddColumn<string>(
                name: "RaterScaleDescription",
                table: "RaterScales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RaterScaleTitle",
                table: "RaterScales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RaterScaleId",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_RaterScaleId",
                table: "Questions",
                column: "RaterScaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_RaterScales_RaterScaleId",
                table: "Questions",
                column: "RaterScaleId",
                principalTable: "RaterScales",
                principalColumn: "Id");
        }
    }
}
