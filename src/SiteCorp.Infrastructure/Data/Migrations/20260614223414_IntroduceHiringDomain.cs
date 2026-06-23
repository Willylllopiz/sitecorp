using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteCorp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceHiringDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Departments_DepartmentId",
                table: "Positions");

            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.EnsureSchema(
                name: "org");

            migrationBuilder.EnsureSchema(
                name: "hr");

            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "DrivingLicenseCategories",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrivingLicenseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationLevels",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HierarchyLevel = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentTypes",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entities",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaritalStatuses",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaritalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoliticalAffiliations",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliticalAffiliations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RetireeRehireStatuses",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetireeRehireStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkinColors",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinColors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessGroups",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessGroups_Entities_Id",
                        column: x => x.Id,
                        principalSchema: "org",
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobTemplates",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTemplates_EducationLevels_EducationLevelId",
                        column: x => x.EducationLevelId,
                        principalSchema: "catalog",
                        principalTable: "EducationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTemplates_Entities_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "org",
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    NationalId_Number = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AddressStreet = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: false),
                    AddressNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    AddressCity = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AddressProvince = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AddressPostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NumberOfChildren = table.Column<int>(type: "int", nullable: false),
                    DefenseSituation = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    PreEmploymentCheck = table.Column<bool>(type: "bit", nullable: false),
                    CompletedDegree = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    HasCriminalRecord = table.Column<bool>(type: "bit", nullable: false),
                    HasEmploymentContract = table.Column<bool>(type: "bit", nullable: false),
                    DisciplinaryMeasures = table.Column<string>(type: "nvarchar(360)", maxLength: 360, nullable: true),
                    EducationLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaritalStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkinColorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PoliticalAffiliationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmploymentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DrivingLicenseCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RetireeRehireStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    PantsSize = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ShirtSize = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ShoeSize = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_DrivingLicenseCategories_DrivingLicenseCategoryId",
                        column: x => x.DrivingLicenseCategoryId,
                        principalSchema: "catalog",
                        principalTable: "DrivingLicenseCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persons_EducationLevels_EducationLevelId",
                        column: x => x.EducationLevelId,
                        principalSchema: "catalog",
                        principalTable: "EducationLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persons_EmploymentTypes_EmploymentTypeId",
                        column: x => x.EmploymentTypeId,
                        principalSchema: "catalog",
                        principalTable: "EmploymentTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persons_Genders_GenderId",
                        column: x => x.GenderId,
                        principalSchema: "catalog",
                        principalTable: "Genders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persons_MaritalStatuses_MaritalStatusId",
                        column: x => x.MaritalStatusId,
                        principalSchema: "catalog",
                        principalTable: "MaritalStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persons_PoliticalAffiliations_PoliticalAffiliationId",
                        column: x => x.PoliticalAffiliationId,
                        principalSchema: "catalog",
                        principalTable: "PoliticalAffiliations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persons_RetireeRehireStatuses_RetireeRehireStatusId",
                        column: x => x.RetireeRehireStatusId,
                        principalSchema: "catalog",
                        principalTable: "RetireeRehireStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Persons_SkinColors_SkinColorId",
                        column: x => x.SkinColorId,
                        principalSchema: "catalog",
                        principalTable: "SkinColors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressStreet = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: false),
                    AddressNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    AddressCity = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AddressProvince = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AddressPostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_BusinessGroups_BusinessGroupId",
                        column: x => x.BusinessGroupId,
                        principalSchema: "org",
                        principalTable: "BusinessGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Companies_Entities_Id",
                        column: x => x.Id,
                        principalSchema: "org",
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTemplatePositions",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalVacancies = table.Column<int>(type: "int", nullable: false),
                    FilledVacancies = table.Column<int>(type: "int", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SalaryCategory = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTemplatePositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTemplatePositions_JobTemplates_JobTemplateId",
                        column: x => x.JobTemplateId,
                        principalSchema: "hr",
                        principalTable: "JobTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobTemplatePositions_Positions_PositionId",
                        column: x => x.PositionId,
                        principalSchema: "hr",
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UploadDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "hr",
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessUnits",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressStreet = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: false),
                    AddressNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    AddressCity = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AddressProvince = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AddressPostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessUnits_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessUnits_Entities_Id",
                        column: x => x.Id,
                        principalSchema: "org",
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentHistory",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobTemplatePositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExitReason = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentHistory_Entities_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "org",
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmploymentHistory_JobTemplatePositions_JobTemplatePositionId",
                        column: x => x.JobTemplatePositionId,
                        principalSchema: "hr",
                        principalTable: "JobTemplatePositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmploymentHistory_Persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "hr",
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentHistory_Positions_PositionId",
                        column: x => x.PositionId,
                        principalSchema: "hr",
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Positions_Code",
                schema: "hr",
                table: "Positions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnits_CompanyId",
                schema: "org",
                table: "BusinessUnits",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_BusinessGroupId",
                schema: "org",
                table: "Companies",
                column: "BusinessGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PersonId",
                schema: "hr",
                table: "Documents",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DrivingLicenseCategories_Code",
                schema: "catalog",
                table: "DrivingLicenseCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_Code",
                schema: "catalog",
                table: "EducationLevels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentHistory_EntityId",
                schema: "hr",
                table: "EmploymentHistory",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentHistory_JobTemplatePositionId",
                schema: "hr",
                table: "EmploymentHistory",
                column: "JobTemplatePositionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentHistory_PersonId_EntityId_PositionId",
                schema: "hr",
                table: "EmploymentHistory",
                columns: new[] { "PersonId", "EntityId", "PositionId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentHistory_PositionId",
                schema: "hr",
                table: "EmploymentHistory",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentTypes_Code",
                schema: "catalog",
                table: "EmploymentTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genders_Code",
                schema: "catalog",
                table: "Genders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTemplatePositions_JobTemplateId_PositionId",
                schema: "hr",
                table: "JobTemplatePositions",
                columns: new[] { "JobTemplateId", "PositionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTemplatePositions_PositionId",
                schema: "hr",
                table: "JobTemplatePositions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTemplates_EducationLevelId",
                schema: "hr",
                table: "JobTemplates",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTemplates_EntityId",
                schema: "hr",
                table: "JobTemplates",
                column: "EntityId",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MaritalStatuses_Code",
                schema: "catalog",
                table: "MaritalStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_DrivingLicenseCategoryId",
                schema: "hr",
                table: "Persons",
                column: "DrivingLicenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_EducationLevelId",
                schema: "hr",
                table: "Persons",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_EmploymentTypeId",
                schema: "hr",
                table: "Persons",
                column: "EmploymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_GenderId",
                schema: "hr",
                table: "Persons",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_MaritalStatusId",
                schema: "hr",
                table: "Persons",
                column: "MaritalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_NationalId_Number",
                schema: "hr",
                table: "Persons",
                column: "NationalId_Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_PoliticalAffiliationId",
                schema: "hr",
                table: "Persons",
                column: "PoliticalAffiliationId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_RetireeRehireStatusId",
                schema: "hr",
                table: "Persons",
                column: "RetireeRehireStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_SkinColorId",
                schema: "hr",
                table: "Persons",
                column: "SkinColorId");

            migrationBuilder.CreateIndex(
                name: "IX_PoliticalAffiliations_Code",
                schema: "catalog",
                table: "PoliticalAffiliations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RetireeRehireStatuses_Code",
                schema: "catalog",
                table: "RetireeRehireStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkinColors_Code",
                schema: "catalog",
                table: "SkinColors",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessUnits",
                schema: "org");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmploymentHistory",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "org");

            migrationBuilder.DropTable(
                name: "JobTemplatePositions",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Persons",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "BusinessGroups",
                schema: "org");

            migrationBuilder.DropTable(
                name: "JobTemplates",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "DrivingLicenseCategories",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "EmploymentTypes",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "Genders",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "MaritalStatuses",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "PoliticalAffiliations",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "RetireeRehireStatuses",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "SkinColors",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "EducationLevels",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "Entities",
                schema: "org");

            migrationBuilder.DropTable(
                name: "Positions",
                schema: "hr");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Candidates = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TargetStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    EngagementScore = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Positions_DepartmentId",
                table: "Positions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeNumber",
                table: "Employees",
                column: "EmployeeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Departments_DepartmentId",
                table: "Positions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
