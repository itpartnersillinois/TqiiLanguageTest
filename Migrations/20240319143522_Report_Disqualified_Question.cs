using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations
{
    public partial class Report_Disqualified_Question : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsDisqualified",
                table: "ReportDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisqualified",
                table: "ReportDetails");
        }
    }
}
