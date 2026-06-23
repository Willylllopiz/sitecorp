using Microsoft.EntityFrameworkCore;
using SiteCorp.Domain.Authentication;
using SiteCorp.Domain.HumanResources.Catalogs;
using SiteCorp.Domain.HumanResources.Organization;
using SiteCorp.Domain.HumanResources.People;
using SiteCorp.Domain.HumanResources.Staffing;
using AuthCompany = SiteCorp.Domain.Authentication.Company;
using OrgCompany = SiteCorp.Domain.HumanResources.Organization.Company;
using StaffingPosition = SiteCorp.Domain.HumanResources.Staffing.Position;

namespace SiteCorp.Infrastructure.Data;

public sealed class SiteCorpDbContext(DbContextOptions<SiteCorpDbContext> options) : DbContext(options)
{
    public DbSet<AuthCompany> Companies => Set<AuthCompany>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<OrganizationEntity> OrganizationEntities => Set<OrganizationEntity>();

    public DbSet<BusinessGroup> BusinessGroups => Set<BusinessGroup>();

    public DbSet<OrgCompany> OrganizationCompanies => Set<OrgCompany>();

    public DbSet<BusinessUnit> BusinessUnits => Set<BusinessUnit>();

    public DbSet<Gender> Genders => Set<Gender>();

    public DbSet<SkinColor> SkinColors => Set<SkinColor>();

    public DbSet<PoliticalAffiliation> PoliticalAffiliations => Set<PoliticalAffiliation>();

    public DbSet<MaritalStatus> MaritalStatuses => Set<MaritalStatus>();

    public DbSet<EducationLevel> EducationLevels => Set<EducationLevel>();

    public DbSet<EmploymentType> EmploymentTypes => Set<EmploymentType>();

    public DbSet<DrivingLicenseCategory> DrivingLicenseCategories => Set<DrivingLicenseCategory>();

    public DbSet<RetireeRehireStatus> RetireeRehireStatuses => Set<RetireeRehireStatus>();

    public DbSet<Person> Persons => Set<Person>();

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<EmploymentHistory> EmploymentHistories => Set<EmploymentHistory>();

    public DbSet<StaffingPosition> StaffingPositions => Set<StaffingPosition>();

    public DbSet<JobTemplate> JobTemplates => Set<JobTemplate>();

    public DbSet<JobTemplatePosition> JobTemplatePositions => Set<JobTemplatePosition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureAuthentication(modelBuilder);
        ConfigureOrganization(modelBuilder);
        ConfigureCatalogs(modelBuilder);
        ConfigurePeople(modelBuilder);
        ConfigureStaffing(modelBuilder);
    }

    private static void ConfigureAuthentication(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthCompany>(builder =>
        {
            builder.ToTable("Companies", "auth");
            builder.HasKey(company => company.Id);
            builder.Property(company => company.Name).HasMaxLength(160).IsRequired();
            builder.Property(company => company.LegalName).HasMaxLength(200);
            builder.Property(company => company.TaxId).HasMaxLength(60);
            builder.Property(company => company.Email).HasMaxLength(180);
            builder.Property(company => company.Phone).HasMaxLength(40);
            builder.Property(company => company.Website).HasMaxLength(200);
            builder.Property(company => company.AddressLine).HasMaxLength(240);
            builder.Property(company => company.City).HasMaxLength(120);
            builder.Property(company => company.Country).HasMaxLength(120).IsRequired();
            builder.Property(company => company.IsActive).HasDefaultValue(true);
            builder.Property(company => company.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("Users", "auth");
            builder.HasKey(user => user.Id);
            builder.HasIndex(user => new { user.CompanyId, user.NormalizedEmail }).IsUnique();
            builder.HasIndex(user => new { user.CompanyId, user.NormalizedUserName }).IsUnique();
            builder.Property(user => user.UserName).HasMaxLength(80).IsRequired();
            builder.Property(user => user.NormalizedUserName).HasMaxLength(80).IsRequired();
            builder.Property(user => user.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(user => user.LastName).HasMaxLength(100).IsRequired();
            builder.Property(user => user.Email).HasMaxLength(180).IsRequired();
            builder.Property(user => user.NormalizedEmail).HasMaxLength(180).IsRequired();
            builder.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
            builder.Property(user => user.PhoneNumber).HasMaxLength(40);
            builder.Property(user => user.IsActive).HasDefaultValue(true);
            builder.Property(user => user.CreatedAt).IsRequired();
            builder.Ignore(user => user.FullName);

            builder.HasOne<AuthCompany>()
                .WithMany()
                .HasForeignKey(user => user.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(builder =>
        {
            builder.ToTable("Roles", "auth");
            builder.HasKey(role => role.Id);
            builder.HasIndex(role => new { role.CompanyId, role.NormalizedName }).IsUnique();
            builder.Property(role => role.Name).HasMaxLength(100).IsRequired();
            builder.Property(role => role.NormalizedName).HasMaxLength(100).IsRequired();
            builder.Property(role => role.Description).HasMaxLength(240);
            builder.Property(role => role.IsActive).HasDefaultValue(true);
            builder.Property(role => role.CreatedAt).IsRequired();

            builder.HasOne<AuthCompany>()
                .WithMany()
                .HasForeignKey(role => role.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Permission>(builder =>
        {
            builder.ToTable("Permissions", "auth");
            builder.HasKey(permission => permission.Id);
            builder.HasIndex(permission => permission.Code).IsUnique();
            builder.Property(permission => permission.Code).HasMaxLength(120).IsRequired();
            builder.Property(permission => permission.Name).HasMaxLength(140).IsRequired();
            builder.Property(permission => permission.Module).HasMaxLength(80).IsRequired();
            builder.Property(permission => permission.Description).HasMaxLength(240);
            builder.Property(permission => permission.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<UserRole>(builder =>
        {
            builder.ToTable("UserRoles", "auth");
            builder.HasKey(userRole => new { userRole.UserId, userRole.RoleId });
            builder.Property(userRole => userRole.AssignedAt).IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(userRole => userRole.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Role>()
                .WithMany()
                .HasForeignKey(userRole => userRole.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(userRole => userRole.AssignedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<RolePermission>(builder =>
        {
            builder.ToTable("RolePermissions", "auth");
            builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });
            builder.Property(rolePermission => rolePermission.AssignedAt).IsRequired();

            builder.HasOne<Role>()
                .WithMany()
                .HasForeignKey(rolePermission => rolePermission.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Permission>()
                .WithMany()
                .HasForeignKey(rolePermission => rolePermission.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(builder =>
        {
            builder.ToTable("RefreshTokens", "auth");
            builder.HasKey(refreshToken => refreshToken.Id);
            builder.HasIndex(refreshToken => refreshToken.TokenHash).IsUnique();
            builder.Property(refreshToken => refreshToken.TokenHash).HasMaxLength(500).IsRequired();
            builder.Property(refreshToken => refreshToken.CreatedByIp).HasMaxLength(64);
            builder.Property(refreshToken => refreshToken.RevokedByIp).HasMaxLength(64);
            builder.Property(refreshToken => refreshToken.UserAgent).HasMaxLength(500);
            builder.Property(refreshToken => refreshToken.CreatedAt).IsRequired();
            builder.Property(refreshToken => refreshToken.ExpiresAt).IsRequired();
            builder.Ignore(refreshToken => refreshToken.IsActive);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(refreshToken => refreshToken.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<RefreshToken>()
                .WithMany()
                .HasForeignKey(refreshToken => refreshToken.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureOrganization(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrganizationEntity>(builder =>
        {
            builder.ToTable("Entities", "org");
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.Name).HasMaxLength(160).IsRequired();
            builder.Property(entity => entity.Description).HasMaxLength(320);
            builder.Property(entity => entity.EntityType).HasConversion<string>().HasMaxLength(40).IsRequired();
            builder.Property(entity => entity.CreatedDate).IsRequired();
            builder.Property(entity => entity.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<BusinessGroup>(builder =>
        {
            builder.ToTable("BusinessGroups", "org");
        });

        modelBuilder.Entity<OrgCompany>(builder =>
        {
            builder.ToTable("Companies", "org");
            builder.HasIndex(company => company.BusinessGroupId);

            builder.OwnsOne(company => company.Address, owned =>
            {
                owned.Property(address => address.Street).HasColumnName("AddressStreet").HasMaxLength(240).IsRequired();
                owned.Property(address => address.Number).HasColumnName("AddressNumber").HasMaxLength(40);
                owned.Property(address => address.City).HasColumnName("AddressCity").HasMaxLength(120).IsRequired();
                owned.Property(address => address.Province).HasColumnName("AddressProvince").HasMaxLength(120).IsRequired();
                owned.Property(address => address.PostalCode).HasColumnName("AddressPostalCode").HasMaxLength(20);
            });

            builder.Navigation(company => company.Address).IsRequired();

            builder.HasOne<BusinessGroup>()
                .WithMany()
                .HasForeignKey(company => company.BusinessGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BusinessUnit>(builder =>
        {
            builder.ToTable("BusinessUnits", "org");
            builder.HasIndex(unit => unit.CompanyId);

            builder.OwnsOne(unit => unit.Address, owned =>
            {
                owned.Property(address => address.Street).HasColumnName("AddressStreet").HasMaxLength(240).IsRequired();
                owned.Property(address => address.Number).HasColumnName("AddressNumber").HasMaxLength(40);
                owned.Property(address => address.City).HasColumnName("AddressCity").HasMaxLength(120).IsRequired();
                owned.Property(address => address.Province).HasColumnName("AddressProvince").HasMaxLength(120).IsRequired();
                owned.Property(address => address.PostalCode).HasColumnName("AddressPostalCode").HasMaxLength(20);
            });

            builder.Navigation(unit => unit.Address).IsRequired();

            builder.HasOne<OrgCompany>()
                .WithMany()
                .HasForeignKey(unit => unit.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCatalogs(ModelBuilder modelBuilder)
    {
        ConfigureCatalog<Gender>(modelBuilder, "Genders");
        ConfigureCatalog<SkinColor>(modelBuilder, "SkinColors");
        ConfigureCatalog<PoliticalAffiliation>(modelBuilder, "PoliticalAffiliations");
        ConfigureCatalog<MaritalStatus>(modelBuilder, "MaritalStatuses");
        ConfigureCatalog<EmploymentType>(modelBuilder, "EmploymentTypes");
        ConfigureCatalog<DrivingLicenseCategory>(modelBuilder, "DrivingLicenseCategories");
        ConfigureCatalog<RetireeRehireStatus>(modelBuilder, "RetireeRehireStatuses");

        modelBuilder.Entity<EducationLevel>(builder =>
        {
            builder.ToTable("EducationLevels", "catalog");
            builder.HasKey(item => item.Id);
            builder.HasIndex(item => item.Code).IsUnique();
            builder.Property(item => item.Code).HasMaxLength(40).IsRequired();
            builder.Property(item => item.Description).HasMaxLength(180).IsRequired();
            builder.Property(item => item.IsActive).HasDefaultValue(true);
            builder.Property(item => item.HierarchyLevel).IsRequired();
        });
    }

    private static void ConfigureCatalog<TCatalog>(ModelBuilder modelBuilder, string tableName)
        where TCatalog : CatalogItem
    {
        modelBuilder.Entity<TCatalog>(builder =>
        {
            builder.ToTable(tableName, "catalog");
            builder.HasKey(item => item.Id);
            builder.HasIndex(item => item.Code).IsUnique();
            builder.Property(item => item.Code).HasMaxLength(40).IsRequired();
            builder.Property(item => item.Description).HasMaxLength(180).IsRequired();
            builder.Property(item => item.IsActive).HasDefaultValue(true);
        });
    }

    private static void ConfigurePeople(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(builder =>
        {
            builder.ToTable("Persons", "hr");
            builder.HasKey(person => person.Id);
            builder.Property(person => person.BirthDate).IsRequired();
            builder.Property(person => person.NumberOfChildren).IsRequired();
            builder.Property(person => person.DefenseSituation).HasMaxLength(180);
            builder.Property(person => person.CompletedDegree).HasMaxLength(180);
            builder.Property(person => person.DisciplinaryMeasures).HasMaxLength(360);
            builder.Property(person => person.CreatedAt).IsRequired();

            builder.OwnsOne(person => person.FullName, owned =>
            {
                owned.Property(name => name.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired();
                owned.Property(name => name.LastName).HasColumnName("LastName").HasMaxLength(140).IsRequired();
                owned.Ignore(name => name.Value);
            });

            builder.OwnsOne(person => person.NationalId, owned =>
            {
                owned.HasIndex(nationalId => nationalId.Number).IsUnique();
                owned.Property(nationalId => nationalId.Number).HasColumnName("NationalId_Number").HasMaxLength(40).IsRequired();
            });

            builder.OwnsOne(person => person.Address, owned =>
            {
                owned.Property(address => address.Street).HasColumnName("AddressStreet").HasMaxLength(240).IsRequired();
                owned.Property(address => address.Number).HasColumnName("AddressNumber").HasMaxLength(40);
                owned.Property(address => address.City).HasColumnName("AddressCity").HasMaxLength(120).IsRequired();
                owned.Property(address => address.Province).HasColumnName("AddressProvince").HasMaxLength(120).IsRequired();
                owned.Property(address => address.PostalCode).HasColumnName("AddressPostalCode").HasMaxLength(20);
            });

            builder.OwnsOne(person => person.PhysicalData, owned =>
            {
                owned.Property(data => data.Height).HasColumnName("Height").HasPrecision(9, 2);
                owned.Property(data => data.Weight).HasColumnName("Weight").HasPrecision(9, 2);
                owned.Property(data => data.PantsSize).HasColumnName("PantsSize").HasMaxLength(40);
                owned.Property(data => data.ShirtSize).HasColumnName("ShirtSize").HasMaxLength(40);
                owned.Property(data => data.ShoeSize).HasColumnName("ShoeSize").HasMaxLength(40);
            });

            builder.Navigation(person => person.FullName).IsRequired();
            builder.Navigation(person => person.NationalId).IsRequired();
            builder.Navigation(person => person.Address).IsRequired();
            builder.Navigation(person => person.PhysicalData).IsRequired();

            builder.HasOne<EducationLevel>().WithMany().HasForeignKey(person => person.EducationLevelId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<MaritalStatus>().WithMany().HasForeignKey(person => person.MaritalStatusId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<Gender>().WithMany().HasForeignKey(person => person.GenderId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<SkinColor>().WithMany().HasForeignKey(person => person.SkinColorId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<PoliticalAffiliation>().WithMany().HasForeignKey(person => person.PoliticalAffiliationId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<EmploymentType>().WithMany().HasForeignKey(person => person.EmploymentTypeId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<DrivingLicenseCategory>().WithMany().HasForeignKey(person => person.DrivingLicenseCategoryId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<RetireeRehireStatus>().WithMany().HasForeignKey(person => person.RetireeRehireStatusId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Document>(builder =>
        {
            builder.ToTable("Documents", "hr");
            builder.HasKey(document => document.Id);
            builder.HasIndex(document => document.PersonId);
            builder.Property(document => document.DocumentType).HasMaxLength(80).IsRequired();
            builder.Property(document => document.FilePath).HasMaxLength(500).IsRequired();
            builder.Property(document => document.UploadDate).IsRequired();
            builder.Property(document => document.IsValid).HasDefaultValue(true);

            builder.HasOne<Person>()
                .WithMany()
                .HasForeignKey(document => document.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmploymentHistory>(builder =>
        {
            builder.ToTable("EmploymentHistory", "hr");
            builder.HasKey(history => history.Id);
            builder.HasIndex(history => new { history.PersonId, history.EntityId, history.PositionId })
                .IsUnique()
                .HasFilter("[IsActive] = 1");
            builder.Property(history => history.ExitReason).HasMaxLength(240);
            builder.Property(history => history.Notes).HasMaxLength(500);
            builder.Property(history => history.IsActive).HasDefaultValue(true);
            builder.Property(history => history.CreatedAt).IsRequired();

            builder.OwnsOne(history => history.Period, owned =>
            {
                owned.Property(period => period.StartDate).HasColumnName("StartDate").IsRequired();
                owned.Property(period => period.EndDate).HasColumnName("EndDate");
            });

            builder.Navigation(history => history.Period).IsRequired();

            builder.HasOne<Person>().WithMany().HasForeignKey(history => history.PersonId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<OrganizationEntity>().WithMany().HasForeignKey(history => history.EntityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<StaffingPosition>().WithMany().HasForeignKey(history => history.PositionId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<JobTemplatePosition>().WithMany().HasForeignKey(history => history.JobTemplatePositionId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureStaffing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StaffingPosition>(builder =>
        {
            builder.ToTable("Positions", "hr");
            builder.HasKey(position => position.Id);
            builder.HasIndex(position => position.Code).IsUnique();
            builder.Property(position => position.Code).HasMaxLength(40).IsRequired();
            builder.Property(position => position.Name).HasMaxLength(140).IsRequired();
            builder.Property(position => position.Description).HasMaxLength(320);
            builder.Property(position => position.Category).HasMaxLength(100).IsRequired();
            builder.Property(position => position.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<JobTemplate>(builder =>
        {
            builder.ToTable("JobTemplates", "hr");
            builder.HasKey(template => template.Id);
            builder.HasIndex(template => template.EntityId)
                .IsUnique()
                .HasFilter("[IsActive] = 1");
            builder.Property(template => template.ApprovedBy).HasMaxLength(160).IsRequired();
            builder.Property(template => template.CreatedAt).IsRequired();
            builder.Property(template => template.IsActive).HasDefaultValue(true);

            builder.HasOne<OrganizationEntity>().WithMany().HasForeignKey(template => template.EntityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<EducationLevel>().WithMany().HasForeignKey(template => template.EducationLevelId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JobTemplatePosition>(builder =>
        {
            builder.ToTable("JobTemplatePositions", "hr");
            builder.HasKey(position => position.Id);
            builder.HasIndex(position => new { position.JobTemplateId, position.PositionId }).IsUnique();
            builder.Property(position => position.RowVersion).IsRowVersion();

            builder.OwnsOne(position => position.Vacancy, owned =>
            {
                owned.Property(vacancy => vacancy.TotalVacancies).HasColumnName("TotalVacancies").IsRequired();
                owned.Property(vacancy => vacancy.FilledVacancies).HasColumnName("FilledVacancies").IsRequired();
                owned.Property(vacancy => vacancy.BaseSalary).HasColumnName("BaseSalary").HasPrecision(18, 2).IsRequired();
                owned.Property(vacancy => vacancy.SalaryCategory).HasColumnName("SalaryCategory").HasMaxLength(80).IsRequired();
            });

            builder.Navigation(position => position.Vacancy).IsRequired();

            builder.HasOne<JobTemplate>().WithMany().HasForeignKey(position => position.JobTemplateId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<StaffingPosition>().WithMany().HasForeignKey(position => position.PositionId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
