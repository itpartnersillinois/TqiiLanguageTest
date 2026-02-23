using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations.RegistrationDb
{
    /// <inheritdoc />
    public partial class EnrollmentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentEndDate",
                table: "Cohorts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentStartDate",
                table: "Cohorts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrollmentEndDate",
                table: "Cohorts");

            migrationBuilder.DropColumn(
                name: "EnrollmentStartDate",
                table: "Cohorts");
        }
    }
}
