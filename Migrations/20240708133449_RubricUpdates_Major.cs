using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class RubricUpdates_Major : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_QuestionRubrics_QuestionRubricId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_QuestionRubrics_QuestionRubricId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "QuestionRubrics");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QuestionRubricId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "QuestionRubricId",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "QuestionRubricId",
                table: "Questions",
                newName: "RaterScaleId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_QuestionRubricId",
                table: "Questions",
                newName: "IX_Questions_RaterScaleId");

            migrationBuilder.CreateTable(
                name: "RaterScales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descriptors = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    RaterScaleDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RaterScaleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RaterScaleTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaterScales", x => x.Id);
                });

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

            migrationBuilder.DropTable(
                name: "RaterScales");

            migrationBuilder.RenameColumn(
                name: "RaterScaleId",
                table: "Questions",
                newName: "QuestionRubricId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_RaterScaleId",
                table: "Questions",
                newName: "IX_Questions_QuestionRubricId");

            migrationBuilder.AddColumn<int>(
                name: "QuestionRubricId",
                table: "Answers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionRubrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Information = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionRubrics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionRubricId",
                table: "Answers",
                column: "QuestionRubricId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_QuestionRubrics_QuestionRubricId",
                table: "Answers",
                column: "QuestionRubricId",
                principalTable: "QuestionRubrics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_QuestionRubrics_QuestionRubricId",
                table: "Questions",
                column: "QuestionRubricId",
                principalTable: "QuestionRubrics",
                principalColumn: "Id");
        }
    }
}
