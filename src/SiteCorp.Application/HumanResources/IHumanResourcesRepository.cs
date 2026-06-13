using SiteCorp.Domain.HumanResources;

namespace SiteCorp.Application.HumanResources;

public interface IHumanResourcesRepository
{
    Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByIdAsync(int employeeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Position>> GetPositionsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default);

    Task AddLeaveRequestAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

