using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations.RegistrationDb
{
    public partial class RegistrationComplete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documents_RegistrationTestPersonId",
                table: "Documents");

            migrationBuilder.AddColumn<bool>(
                name: "IsRegistrationCompleted",
                table: "CohortPeople",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_RegistrationTestPersonId",
                table: "Documents",
                column: "RegistrationTestPersonId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documents_RegistrationTestPersonId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IsRegistrationCompleted",
                table: "CohortPeople");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_RegistrationTestPersonId",
                table: "Documents",
                column: "RegistrationTestPersonId");
        }
    }
}
