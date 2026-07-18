using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteCorp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateDocumentAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasDisciplinaryMeasures",
                schema: "hr",
                table: "Persons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                UPDATE hr.Persons
                SET HasDisciplinaryMeasures = 1
                WHERE DisciplinaryMeasures IS NOT NULL
                    AND LTRIM(RTRIM(DisciplinaryMeasures)) <> '';
                """);

            migrationBuilder.DropColumn(
                name: "DisciplinaryMeasures",
                schema: "hr",
                table: "Persons");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                schema: "hr",
                table: "Documents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "ContentBase64",
                schema: "hr",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                schema: "hr",
                table: "Documents",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                schema: "hr",
                table: "Documents",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE hr.Documents
                SET FileName = COALESCE(NULLIF(FilePath, ''), N'Documento existente'),
                    ContentType = N'application/octet-stream'
                WHERE FileName = '';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentBase64",
                schema: "hr",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ContentType",
                schema: "hr",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileName",
                schema: "hr",
                table: "Documents");

            migrationBuilder.AddColumn<string>(
                name: "DisciplinaryMeasures",
                schema: "hr",
                table: "Persons",
                type: "nvarchar(360)",
                maxLength: 360,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE hr.Persons
                SET DisciplinaryMeasures = N'Registradas'
                WHERE HasDisciplinaryMeasures = 1;
                """);

            migrationBuilder.DropColumn(
                name: "HasDisciplinaryMeasures",
                schema: "hr",
                table: "Persons");

            migrationBuilder.Sql("""
                UPDATE hr.Documents
                SET FilePath = N''
                WHERE FilePath IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                schema: "hr",
                table: "Documents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
