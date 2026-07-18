using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SiteCorp.Infrastructure.Data;

#nullable disable

namespace SiteCorp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(SiteCorpDbContext))]
    [Migration("20260718120000_AddStaffingAreas")]
    public partial class AddStaffingAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffingAreas",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffingAreas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffingAreas_Name",
                schema: "hr",
                table: "StaffingAreas",
                column: "Name",
                unique: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AreaId",
                schema: "hr",
                table: "JobTemplatePositions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("""
                DECLARE @DefaultAreaId uniqueidentifier;

                SELECT @DefaultAreaId = Id
                FROM hr.StaffingAreas
                WHERE Name = N'General';

                IF @DefaultAreaId IS NULL
                BEGIN
                    SET @DefaultAreaId = NEWID();

                    INSERT INTO hr.StaffingAreas (Id, Name, Priority, IsActive)
                    VALUES (@DefaultAreaId, N'General', 999, 1);
                END;

                UPDATE hr.JobTemplatePositions
                SET AreaId = @DefaultAreaId
                WHERE AreaId IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                schema: "hr",
                table: "JobTemplatePositions",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTemplatePositions_AreaId",
                schema: "hr",
                table: "JobTemplatePositions",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobTemplatePositions_StaffingAreas_AreaId",
                schema: "hr",
                table: "JobTemplatePositions",
                column: "AreaId",
                principalSchema: "hr",
                principalTable: "StaffingAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobTemplatePositions_StaffingAreas_AreaId",
                schema: "hr",
                table: "JobTemplatePositions");

            migrationBuilder.DropIndex(
                name: "IX_JobTemplatePositions_AreaId",
                schema: "hr",
                table: "JobTemplatePositions");

            migrationBuilder.DropColumn(
                name: "AreaId",
                schema: "hr",
                table: "JobTemplatePositions");

            migrationBuilder.DropTable(
                name: "StaffingAreas",
                schema: "hr");
        }
    }
}
