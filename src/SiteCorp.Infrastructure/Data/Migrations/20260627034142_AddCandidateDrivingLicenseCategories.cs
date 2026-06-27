using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteCorp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateDrivingLicenseCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_DrivingLicenseCategories_DrivingLicenseCategoryId",
                schema: "hr",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_DrivingLicenseCategoryId",
                schema: "hr",
                table: "Persons");

            migrationBuilder.CreateTable(
                name: "PersonDrivingLicenseCategories",
                schema: "hr",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DrivingLicenseCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonDrivingLicenseCategories", x => new { x.PersonId, x.DrivingLicenseCategoryId });
                    table.ForeignKey(
                        name: "FK_PersonDrivingLicenseCategories_DrivingLicenseCategories_DrivingLicenseCategoryId",
                        column: x => x.DrivingLicenseCategoryId,
                        principalSchema: "catalog",
                        principalTable: "DrivingLicenseCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonDrivingLicenseCategories_Persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "hr",
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonDrivingLicenseCategories_DrivingLicenseCategoryId",
                schema: "hr",
                table: "PersonDrivingLicenseCategories",
                column: "DrivingLicenseCategoryId");

            migrationBuilder.Sql("""
                INSERT INTO hr.PersonDrivingLicenseCategories (PersonId, DrivingLicenseCategoryId)
                SELECT person.Id, person.DrivingLicenseCategoryId
                FROM hr.Persons AS person
                INNER JOIN catalog.DrivingLicenseCategories AS category
                    ON category.Id = person.DrivingLicenseCategoryId
                WHERE person.DrivingLicenseCategoryId IS NOT NULL
                    AND category.Code NOT IN ('NINGUNA', 'NO', 'NO_APLICA', 'NO APLICA');
                """);

            migrationBuilder.DropColumn(
                name: "DrivingLicenseCategoryId",
                schema: "hr",
                table: "Persons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DrivingLicenseCategoryId",
                schema: "hr",
                table: "Persons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE person
                SET DrivingLicenseCategoryId = (
                    SELECT TOP 1 category.DrivingLicenseCategoryId
                    FROM hr.PersonDrivingLicenseCategories AS category
                    WHERE category.PersonId = person.Id
                    ORDER BY category.DrivingLicenseCategoryId
                )
                FROM hr.Persons AS person;
                """);

            migrationBuilder.DropTable(
                name: "PersonDrivingLicenseCategories",
                schema: "hr");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_DrivingLicenseCategoryId",
                schema: "hr",
                table: "Persons",
                column: "DrivingLicenseCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_DrivingLicenseCategories_DrivingLicenseCategoryId",
                schema: "hr",
                table: "Persons",
                column: "DrivingLicenseCategoryId",
                principalSchema: "catalog",
                principalTable: "DrivingLicenseCategories",
                principalColumn: "Id");
        }
    }
}
