using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteCorp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitCandidateLastNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                schema: "hr",
                table: "Persons",
                newName: "FirstLastName");

            migrationBuilder.AddColumn<string>(
                name: "SecondLastName",
                schema: "hr",
                table: "Persons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondLastName",
                schema: "hr",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "FirstLastName",
                schema: "hr",
                table: "Persons",
                newName: "LastName");
        }
    }
}
