using Microsoft.EntityFrameworkCore;
using SiteCorp.Application.HumanResources;
using SiteCorp.Domain.HumanResources;

namespace SiteCorp.Infrastructure.Data;

public sealed class EfHumanResourcesRepository(SiteCorpDbContext dbContext) : IHumanResourcesRepository
{
    public async Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Departments
            .AsNoTracking()
            .OrderBy(department => department.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.FullName)
            .ToListAsync(cancellationToken);
    }

    public Task<Employee?> GetEmployeeByIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return dbContext.Employees.FirstOrDefaultAsync(employee => employee.Id == employeeId, cancellationToken);
    }

    public async Task<IReadOnlyList<Position>> GetPositionsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Positions
            .AsNoTracking()
            .OrderBy(position => position.TargetStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.LeaveRequests
            .AsNoTracking()
            .OrderByDescending(request => request.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddLeaveRequestAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        await dbContext.LeaveRequests.AddAsync(leaveRequest, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}

