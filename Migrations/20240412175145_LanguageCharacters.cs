using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class LanguageCharacters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Characters",
                table: "LanguageOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EnforceStrictGrading",
                table: "LanguageOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Popout",
                table: "LanguageOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Characters",
                table: "LanguageOptions");

            migrationBuilder.DropColumn(
                name: "EnforceStrictGrading",
                table: "LanguageOptions");

            migrationBuilder.DropColumn(
                name: "Popout",
                table: "LanguageOptions");
        }
    }
}
