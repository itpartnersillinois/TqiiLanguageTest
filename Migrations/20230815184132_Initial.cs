using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InteractiveReadingAnswer",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "InteractiveReadingImage",
                table: "Questions",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "InteractiveReadingOptions",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "IntroductionImage",
                table: "Questions",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "IntroductionText",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "QuestionImage",
                table: "Questions",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "QuestionType",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "RecordingImage",
                table: "Questions",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InteractiveReadingAnswer",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "InteractiveReadingImage",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "InteractiveReadingOptions",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IntroductionImage",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IntroductionText",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionImage",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionType",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "RecordingImage",
                table: "Questions");
        }
    }
}
