using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SiteCorp.Domain.Authentication;
using SiteCorp.Domain.HumanResources.Catalogs;
using SiteCorp.Domain.HumanResources.Organization;
using SiteCorp.Domain.HumanResources.People;
using SiteCorp.Domain.HumanResources.Staffing;
using SiteCorp.Domain.HumanResources.ValueObjects;
using AuthCompany = SiteCorp.Domain.Authentication.Company;
using OrgCompany = SiteCorp.Domain.HumanResources.Organization.Company;
using StaffingPosition = SiteCorp.Domain.HumanResources.Staffing.Position;

namespace SiteCorp.Infrastructure.Data;

public static class SiteCorpDatabaseInitializer
{
    public static async Task ApplySiteCorpMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SiteCorpDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SiteCorpDbContext>>();

        try
        {
            await dbContext.Database.MigrateAsync();
            await SeedAsync(dbContext);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "No se pudieron aplicar las migraciones de SQL Server de SiteCorp.");
        }
    }

    private static async Task SeedAsync(SiteCorpDbContext dbContext)
    {
        await SeedAuthenticationAsync(dbContext);
        await SeedCatalogsAsync(dbContext);
        await SeedHumanResourcesAsync(dbContext);
    }

    private static async Task SeedAuthenticationAsync(SiteCorpDbContext dbContext)
    {
        const string adminUserName = "admin";
        const string adminPassword = "admin!123";
        const string superAdminRoleName = "SuperAdmin";

        var company = await dbContext.Companies.FirstOrDefaultAsync(company => company.Name == "SiteCorp Local");

        if (company is null)
        {
            company = new AuthCompany("SiteCorp Local", "Cuba");
            company.UpdateProfile(
                name: "SiteCorp Local",
                legalName: "SiteCorp Local",
                taxId: null,
                email: "admin@sitecorp.local",
                phone: null,
                website: null,
                addressLine: null,
                city: "Habana",
                country: "Cuba");

            await dbContext.Companies.AddAsync(company);
            await dbContext.SaveChangesAsync();
        }

        var permissions = new[]
        {
            new Permission("auth.manage", "Administrar seguridad", "Auth", "Gestion de usuarios, roles y permisos."),
            new Permission("companies.manage", "Administrar empresas", "Companies", "Gestion de empresas registradas."),
            new Permission("hr.manage", "Administrar Recursos Humanos", "HumanResources", "Gestion completa del modulo de Recursos Humanos.")
        };

        foreach (var permission in permissions)
        {
            if (!await dbContext.Permissions.AnyAsync(item => item.Code == permission.Code))
            {
                await dbContext.Permissions.AddAsync(permission);
            }
        }

        await dbContext.SaveChangesAsync();

        var superAdminRole = await dbContext.Roles.FirstOrDefaultAsync(role =>
            role.CompanyId == company.Id && role.NormalizedName == superAdminRoleName.ToUpperInvariant());

        if (superAdminRole is null)
        {
            superAdminRole = new Role(company.Id, superAdminRoleName, "Acceso total para pruebas iniciales.", isSystemRole: true);
            await dbContext.Roles.AddAsync(superAdminRole);
            await dbContext.SaveChangesAsync();
        }

        var permissionCodes = permissions.Select(item => item.Code).ToList();
        var storedPermissions = await dbContext.Permissions
            .Where(permission => permissionCodes.Contains(permission.Code))
            .ToListAsync();

        foreach (var permission in storedPermissions)
        {
            var rolePermissionExists = await dbContext.RolePermissions.AnyAsync(item =>
                item.RoleId == superAdminRole.Id && item.PermissionId == permission.Id);

            if (!rolePermissionExists)
            {
                await dbContext.RolePermissions.AddAsync(new RolePermission(superAdminRole.Id, permission.Id));
            }
        }

        await dbContext.SaveChangesAsync();

        var adminUser = await dbContext.Users.FirstOrDefaultAsync(user =>
            user.CompanyId == company.Id && user.NormalizedUserName == adminUserName.ToUpperInvariant());

        if (adminUser is null)
        {
            adminUser = new User(
                company.Id,
                adminUserName,
                "Admin",
                "SiteCorp",
                "admin@sitecorp.local",
                "bootstrap-password-hash");

            var passwordHasher = new PasswordHasher<User>();
            adminUser.ChangePasswordHash(passwordHasher.HashPassword(adminUser, adminPassword), mustChangePassword: false);
            adminUser.ConfirmEmail();

            await dbContext.Users.AddAsync(adminUser);
            await dbContext.SaveChangesAsync();
        }

        var userRoleExists = await dbContext.UserRoles.AnyAsync(item =>
            item.UserId == adminUser.Id && item.RoleId == superAdminRole.Id);

        if (!userRoleExists)
        {
            await dbContext.UserRoles.AddAsync(new UserRole(adminUser.Id, superAdminRole.Id));
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedCatalogsAsync(SiteCorpDbContext dbContext)
    {
        await EnsureCatalogAsync(dbContext.Genders, new Gender("M", "Masculino"), item => item.Code);
        await EnsureCatalogAsync(dbContext.Genders, new Gender("F", "Femenino"), item => item.Code);

        await EnsureCatalogAsync(dbContext.SkinColors, new SkinColor("ND", "No declarado"), item => item.Code);
        await EnsureCatalogAsync(dbContext.PoliticalAffiliations, new PoliticalAffiliation("NINGUNA", "Ninguna"), item => item.Code);
        await EnsureCatalogAsync(dbContext.MaritalStatuses, new MaritalStatus("SOLTERO", "Soltero(a)"), item => item.Code);
        await EnsureCatalogAsync(dbContext.MaritalStatuses, new MaritalStatus("CASADO", "Casado(a)"), item => item.Code);
        await EnsureCatalogAsync(dbContext.EmploymentTypes, new EmploymentType("CONTRATADO", "Contratado"), item => item.Code);
        await EnsureCatalogAsync(dbContext.DrivingLicenseCategories, new DrivingLicenseCategory("NINGUNA", "Sin licencia"), item => item.Code);
        await EnsureCatalogAsync(dbContext.RetireeRehireStatuses, new RetireeRehireStatus("NO_APLICA", "No aplica"), item => item.Code);

        if (!await dbContext.EducationLevels.AnyAsync(item => item.Code == "MEDIO"))
        {
            await dbContext.EducationLevels.AddAsync(new EducationLevel("MEDIO", "Nivel medio", 2));
        }

        if (!await dbContext.EducationLevels.AnyAsync(item => item.Code == "UNIVERSITARIO"))
        {
            await dbContext.EducationLevels.AddAsync(new EducationLevel("UNIVERSITARIO", "Universitario", 4));
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedHumanResourcesAsync(SiteCorpDbContext dbContext)
    {
        var humanResourcesArea = await EnsureStaffingAreaAsync(dbContext, "Recursos Humanos", 10);

        if (await dbContext.OrganizationEntities.AnyAsync())
        {
            return;
        }

        var university = await dbContext.EducationLevels.SingleAsync(item => item.Code == "UNIVERSITARIO");
        var married = await dbContext.MaritalStatuses.FirstAsync(item => item.Code == "CASADO");
        var female = await dbContext.Genders.FirstAsync(item => item.Code == "F");
        var skinColor = await dbContext.SkinColors.FirstAsync(item => item.Code == "ND");
        var affiliation = await dbContext.PoliticalAffiliations.FirstAsync(item => item.Code == "NINGUNA");
        var employmentType = await dbContext.EmploymentTypes.FirstAsync(item => item.Code == "CONTRATADO");
        var retireeStatus = await dbContext.RetireeRehireStatuses.FirstAsync(item => item.Code == "NO_APLICA");

        var group = new BusinessGroup("Grupo SiteCorp Demo", "Grupo empresarial inicial para pruebas.");
        await dbContext.BusinessGroups.AddAsync(group);
        await dbContext.SaveChangesAsync();

        var company = new OrgCompany(
            group.Id,
            "SiteCorp MIPYME",
            "Empresa demo de servicios de capital humano.",
            new Address("Calle Principal", "101", "Habana", "La Habana", "10400"));
        await dbContext.OrganizationCompanies.AddAsync(company);
        await dbContext.SaveChangesAsync();

        var unit = new BusinessUnit(
            company.Id,
            "Servicios de Capital Humano",
            "Unidad operativa de Recursos Humanos.",
            new Address("Calle Principal", "101", "Habana", "La Habana", "10400"));
        await dbContext.BusinessUnits.AddAsync(unit);
        await dbContext.SaveChangesAsync();

        var position = new StaffingPosition(
            "HR-ESP",
            "Especialista de Recursos Humanos",
            "Gestion operativa de capital humano para clientes MIPYME.",
            "Tecnico");
        await dbContext.StaffingPositions.AddAsync(position);
        await dbContext.SaveChangesAsync();

        var template = new JobTemplate(unit.Id, university.Id, new DateOnly(2026, 1, 1), "Direccion General");
        await dbContext.JobTemplates.AddAsync(template);
        await dbContext.SaveChangesAsync();

        var templatePosition = new JobTemplatePosition(
            template.Id,
            humanResourcesArea.Id,
            position.Id,
            new VacancyInfo(totalVacancies: 2, filledVacancies: 0, baseSalary: 45000, salaryCategory: "CAT-TECNICO"));
        await dbContext.JobTemplatePositions.AddAsync(templatePosition);
        await dbContext.SaveChangesAsync();

        var person = new Person(
            new FullName("Laura", "Benitez", "Perez"),
            new NationalId("SC-DEMO-1001"),
            new DateOnly(1992, 3, 12),
            new Address("Avenida Central", "44", "Habana", "La Habana", null),
            numberOfChildren: 1,
            defenseSituation: "No aplica",
            preEmploymentCheck: false,
            completedDegree: "Licenciatura en Psicologia",
            hasCriminalRecord: false,
            hasEmploymentContract: false,
            hasDisciplinaryMeasures: false,
            university.Id,
            specialty: "Psicologia",
            married.Id,
            female.Id,
            skinColor.Id,
            affiliation.Id,
            employmentType.Id,
            Array.Empty<Guid>(),
            retireeStatus.Id,
            new PhysicalData(168, 62, "M", "M", "38"));
        await dbContext.Persons.AddAsync(person);
        await dbContext.SaveChangesAsync();

        templatePosition.OccupyVacancy();
        await dbContext.EmploymentHistories.AddAsync(new EmploymentHistory(
            person.Id,
            unit.Id,
            position.Id,
            templatePosition.Id,
            new DateOnly(2026, 6, 1),
            "Contratacion inicial de prueba."));
        await dbContext.SaveChangesAsync();
    }

    private static async Task EnsureCatalogAsync<TCatalog>(
        DbSet<TCatalog> dbSet,
        TCatalog item,
        Func<TCatalog, string> codeSelector)
        where TCatalog : CatalogItem
    {
        var code = codeSelector(item);

        if (!await dbSet.AnyAsync(existing => existing.Code == code))
        {
            await dbSet.AddAsync(item);
        }
    }

    private static async Task<StaffingArea> EnsureStaffingAreaAsync(SiteCorpDbContext dbContext, string name, int priority)
    {
        var area = await dbContext.StaffingAreas.FirstOrDefaultAsync(existing => existing.Name == name);

        if (area is not null)
        {
            return area;
        }

        area = new StaffingArea(name, priority);
        await dbContext.StaffingAreas.AddAsync(area);
        await dbContext.SaveChangesAsync();
        return area;
    }
}
