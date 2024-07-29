using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class RubricScaleNameOnQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RubricRaterScaleName",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RubricRaterScaleName",
                table: "Questions");
        }
    }
}
