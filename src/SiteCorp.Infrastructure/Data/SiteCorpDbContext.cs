using Microsoft.EntityFrameworkCore;
using SiteCorp.Domain.Authentication;
using SiteCorp.Domain.HumanResources;

namespace SiteCorp.Infrastructure.Data;

public sealed class SiteCorpDbContext(DbContextOptions<SiteCorpDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(builder =>
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

            builder.HasOne<Company>()
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

            builder.HasOne<Company>()
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

        modelBuilder.Entity<Department>(builder =>
        {
            builder.ToTable("Departments");
            builder.HasKey(department => department.Id);
            builder.Property(department => department.Name).HasMaxLength(120).IsRequired();
            builder.Property(department => department.ManagerName).HasMaxLength(160).IsRequired();
        });

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.ToTable("Employees");
            builder.HasKey(employee => employee.Id);
            builder.HasIndex(employee => employee.EmployeeNumber).IsUnique();
            builder.Property(employee => employee.EmployeeNumber).HasMaxLength(24).IsRequired();
            builder.Property(employee => employee.FullName).HasMaxLength(160).IsRequired();
            builder.Property(employee => employee.Position).HasMaxLength(140).IsRequired();
            builder.Property(employee => employee.Location).HasMaxLength(100).IsRequired();
            builder.Property(employee => employee.Status).HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(employee => employee.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Position>(builder =>
        {
            builder.ToTable("Positions");
            builder.HasKey(position => position.Id);
            builder.Property(position => position.Title).HasMaxLength(140).IsRequired();
            builder.Property(position => position.Location).HasMaxLength(100).IsRequired();
            builder.Property(position => position.Status).HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(position => position.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LeaveRequest>(builder =>
        {
            builder.ToTable("LeaveRequests");
            builder.HasKey(request => request.Id);
            builder.Property(request => request.Reason).HasMaxLength(240).IsRequired();
            builder.Property(request => request.Status).HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(request => request.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
