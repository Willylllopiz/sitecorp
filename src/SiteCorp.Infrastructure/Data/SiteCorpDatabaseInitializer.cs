using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SiteCorp.Domain.Authentication;
using SiteCorp.Domain.HumanResources;

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
            company = new Company("SiteCorp Local", "Cuba");
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

        var storedPermissions = await dbContext.Permissions
            .Where(permission => permissions.Select(item => item.Code).Contains(permission.Code))
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

    private static async Task SeedHumanResourcesAsync(SiteCorpDbContext dbContext)
    {
        if (await dbContext.Departments.AnyAsync())
        {
            return;
        }

        var talent = new Department("Talento y Cultura", "Mariela Fuentes");
        var operations = new Department("Operaciones", "Daniel Crespo");
        var finance = new Department("Finanzas", "Paula Herrera");
        var technology = new Department("Tecnologia", "Andres Molina");

        await dbContext.Departments.AddRangeAsync(talent, operations, finance, technology);
        await dbContext.SaveChangesAsync();

        await dbContext.Employees.AddRangeAsync(
            new Employee("SC-1001", "Laura Benitez", talent.Id, "HR Business Partner", "Habana", new DateOnly(2023, 3, 14), EmploymentStatus.Active, 91),
            new Employee("SC-1002", "Rafael Suarez", operations.Id, "Supervisor de Turno", "Santiago", new DateOnly(2021, 8, 9), EmploymentStatus.Active, 78),
            new Employee("SC-1003", "Camila Torres", finance.Id, "Analista de Nomina", "Habana", new DateOnly(2024, 1, 22), EmploymentStatus.OnLeave, 86),
            new Employee("SC-1004", "Miguel Duarte", technology.Id, "Desarrollador Backend", "Remoto", new DateOnly(2025, 11, 3), EmploymentStatus.Active, 84),
            new Employee("SC-1005", "Sofia Ramos", talent.Id, "Recruiter", "Habana", new DateOnly(2026, 6, 3), EmploymentStatus.Onboarding, 88),
            new Employee("SC-1006", "Ivan Castillo", operations.Id, "Coordinador de Logistica", "Matanzas", new DateOnly(2022, 5, 18), EmploymentStatus.Active, 73));

        await dbContext.Positions.AddRangeAsync(
            new Position("Especialista de Compensacion", talent.Id, "Habana", PositionStatus.Interviewing, 7, new DateOnly(2026, 7, 15)),
            new Position("Ingeniero DevOps", technology.Id, "Remoto", PositionStatus.Open, 11, new DateOnly(2026, 8, 1)),
            new Position("Analista Financiero", finance.Id, "Habana", PositionStatus.Offer, 3, new DateOnly(2026, 7, 1)),
            new Position("Jefe de Almacen", operations.Id, "Santiago", PositionStatus.Open, 5, new DateOnly(2026, 7, 22)));

        await dbContext.SaveChangesAsync();

        var rafael = await dbContext.Employees.SingleAsync(employee => employee.EmployeeNumber == "SC-1002");
        var camila = await dbContext.Employees.SingleAsync(employee => employee.EmployeeNumber == "SC-1003");
        var ivan = await dbContext.Employees.SingleAsync(employee => employee.EmployeeNumber == "SC-1006");

        var camilaLeave = LeaveRequest.Create(camila.Id, new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 14), "Asuntos medicos");
        camilaLeave.Approve();

        await dbContext.LeaveRequests.AddRangeAsync(
            LeaveRequest.Create(rafael.Id, new DateOnly(2026, 6, 18), new DateOnly(2026, 6, 21), "Vacaciones"),
            camilaLeave,
            LeaveRequest.Create(ivan.Id, new DateOnly(2026, 7, 4), new DateOnly(2026, 7, 6), "Tramites personales"));

        await dbContext.SaveChangesAsync();
    }
}
