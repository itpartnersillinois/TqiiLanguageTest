using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqiiLanguageTest.Migrations.RegistrationDb
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cohorts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberStudents = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cohorts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instructions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfInstruction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iein = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NativeLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationCohortId = table.Column<int>(type: "int", nullable: false),
                    RegistrationLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfTest = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationTests_Cohorts_RegistrationCohortId",
                        column: x => x.RegistrationCohortId,
                        principalTable: "Cohorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CohortPeople",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateRegistrationSent = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExternalComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InternalComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsDenied = table.Column<bool>(type: "bit", nullable: false),
                    IsWaitlisted = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationCohortId = table.Column<int>(type: "int", nullable: false),
                    RegistrationGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationPersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CohortPeople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CohortPeople_Cohorts_RegistrationCohortId",
                        column: x => x.RegistrationCohortId,
                        principalTable: "Cohorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CohortPeople_People_RegistrationPersonId",
                        column: x => x.RegistrationPersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationTestPeople",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExternalComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InternalComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsProficiencyExemption = table.Column<bool>(type: "bit", nullable: false),
                    IsProficiencyExemptionApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsProficiencyExemptionDenied = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationCohortPersonId = table.Column<int>(type: "int", nullable: false),
                    RegistrationTestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationTestPeople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationTestPeople_CohortPeople_RegistrationCohortPersonId",
                        column: x => x.RegistrationCohortPersonId,
                        principalTable: "CohortPeople",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrationTestPeople_RegistrationTests_RegistrationTestId",
                        column: x => x.RegistrationTestId,
                        principalTable: "RegistrationTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Document = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationTestPersonId = table.Column<int>(type: "int", nullable: false),
                    TestName = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_RegistrationTestPeople_RegistrationTestPersonId",
                        column: x => x.RegistrationTestPersonId,
                        principalTable: "RegistrationTestPeople",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CohortPeople_RegistrationCohortId",
                table: "CohortPeople",
                column: "RegistrationCohortId");

            migrationBuilder.CreateIndex(
                name: "IX_CohortPeople_RegistrationPersonId",
                table: "CohortPeople",
                column: "RegistrationPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_RegistrationTestPersonId",
                table: "Documents",
                column: "RegistrationTestPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationTestPeople_RegistrationCohortPersonId",
                table: "RegistrationTestPeople",
                column: "RegistrationCohortPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationTestPeople_RegistrationTestId",
                table: "RegistrationTestPeople",
                column: "RegistrationTestId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationTests_RegistrationCohortId",
                table: "RegistrationTests",
                column: "RegistrationCohortId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Instructions");

            migrationBuilder.DropTable(
                name: "RegistrationTestPeople");

            migrationBuilder.DropTable(
                name: "CohortPeople");

            migrationBuilder.DropTable(
                name: "RegistrationTests");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Cohorts");
        }
    }
}
