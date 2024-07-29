using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class FinalReviewer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinalScorer",
                table: "RaterTests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinalScorer",
                table: "RaterTests");
        }
    }
}
