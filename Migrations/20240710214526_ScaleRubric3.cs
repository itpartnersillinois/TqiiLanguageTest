using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class ScaleRubric3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaterScales_Questions_QuestionId",
                table: "RaterScales");

            migrationBuilder.DropIndex(
                name: "IX_RaterScales_QuestionId",
                table: "RaterScales");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "RaterScales",
                newName: "QuestionInformationId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_RaterScales_RaterScaleId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_RaterScaleId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "RaterScaleId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "QuestionInformationId",
                table: "RaterScales",
                newName: "QuestionId");

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
    }
}
