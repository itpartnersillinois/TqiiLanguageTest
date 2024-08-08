using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class TranscriptLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TranscriptLink",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranscriptLink",
                table: "Questions");
        }
    }
}
