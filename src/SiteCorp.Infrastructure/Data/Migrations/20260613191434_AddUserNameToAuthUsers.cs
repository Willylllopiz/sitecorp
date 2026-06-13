using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteCorp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToAuthUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                schema: "auth",
                table: "Users",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "auth",
                table: "Users",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId_NormalizedUserName",
                schema: "auth",
                table: "Users",
                columns: new[] { "CompanyId", "NormalizedUserName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_CompanyId_NormalizedUserName",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "auth",
                table: "Users");
        }
    }
}
