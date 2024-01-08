using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class Raters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RaterNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaterNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaterTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateFinished = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalScore = table.Column<int>(type: "int", nullable: false),
                    IsExtraScorer = table.Column<bool>(type: "bit", nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RaterNameId = table.Column<int>(type: "int", nullable: false),
                    TestUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaterTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaterTests_RaterNames_RaterNameId",
                        column: x => x.RaterNameId,
                        principalTable: "RaterNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaterTests_TestUsers_TestUserId",
                        column: x => x.TestUserId,
                        principalTable: "TestUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaterAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerId = table.Column<int>(type: "int", nullable: false),
                    DateFinished = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RaterTestId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaterAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaterAnswers_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaterAnswers_RaterTests_RaterTestId",
                        column: x => x.RaterTestId,
                        principalTable: "RaterTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaterAnswers_AnswerId",
                table: "RaterAnswers",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_RaterAnswers_RaterTestId",
                table: "RaterAnswers",
                column: "RaterTestId");

            migrationBuilder.CreateIndex(
                name: "IX_RaterTests_RaterNameId",
                table: "RaterTests",
                column: "RaterNameId");

            migrationBuilder.CreateIndex(
                name: "IX_RaterTests_TestUserId",
                table: "RaterTests",
                column: "TestUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaterAnswers");

            migrationBuilder.DropTable(
                name: "RaterTests");

            migrationBuilder.DropTable(
                name: "RaterNames");
        }
    }
}
