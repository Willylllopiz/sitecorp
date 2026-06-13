using SiteCorp.Domain.Common;
using DomainDepartment = SiteCorp.Domain.HumanResources.Department;
using DomainEmployee = SiteCorp.Domain.HumanResources.Employee;
using DomainEmploymentStatus = SiteCorp.Domain.HumanResources.EmploymentStatus;
using DomainLeaveRequest = SiteCorp.Domain.HumanResources.LeaveRequest;
using DomainLeaveRequestStatus = SiteCorp.Domain.HumanResources.LeaveRequestStatus;
using DomainPosition = SiteCorp.Domain.HumanResources.Position;
using DomainPositionStatus = SiteCorp.Domain.HumanResources.PositionStatus;
using ContractDepartmentSummary = SiteCorp.Shared.DepartmentSummary;
using ContractEmployee = SiteCorp.Shared.Employee;
using ContractEmploymentStatus = SiteCorp.Shared.EmploymentStatus;
using ContractLeaveRequest = SiteCorp.Shared.LeaveRequest;
using ContractLeaveRequestStatus = SiteCorp.Shared.LeaveRequestStatus;
using ContractPosition = SiteCorp.Shared.Position;
using ContractPositionStatus = SiteCorp.Shared.PositionStatus;

namespace SiteCorp.Application.HumanResources;

public sealed class HumanResourcesService(IHumanResourcesRepository repository)
{
    public async Task<SiteCorp.Shared.HumanResourcesSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var departments = await repository.GetDepartmentsAsync(cancellationToken);
        var employees = await repository.GetEmployeesAsync(cancellationToken);
        var positions = await repository.GetPositionsAsync(cancellationToken);
        var leaveRequests = await repository.GetLeaveRequestsAsync(cancellationToken);

        var metrics = BuildDashboardMetrics(departments, employees, positions, leaveRequests);

        return new SiteCorp.Shared.HumanResourcesSnapshot(
            metrics,
            employees.Select(employee => MapEmployee(employee, departments)).ToList(),
            positions.Select(position => MapPosition(position, departments)).ToList(),
            leaveRequests.Select(request => MapLeaveRequest(request, employees)).ToList(),
            IsLive: true);
    }

    public async Task<SiteCorp.Shared.DashboardMetrics> GetDashboardMetricsAsync(CancellationToken cancellationToken = default)
    {
        var departments = await repository.GetDepartmentsAsync(cancellationToken);
        var employees = await repository.GetEmployeesAsync(cancellationToken);
        var positions = await repository.GetPositionsAsync(cancellationToken);
        var leaveRequests = await repository.GetLeaveRequestsAsync(cancellationToken);

        return BuildDashboardMetrics(departments, employees, positions, leaveRequests);
    }

    public async Task<IReadOnlyList<ContractDepartmentSummary>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        var departments = await repository.GetDepartmentsAsync(cancellationToken);
        var employees = await repository.GetEmployeesAsync(cancellationToken);
        var positions = await repository.GetPositionsAsync(cancellationToken);

        return BuildDepartmentSummaries(departments, employees, positions);
    }

    public async Task<IReadOnlyList<ContractEmployee>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var departments = await repository.GetDepartmentsAsync(cancellationToken);
        var employees = await repository.GetEmployeesAsync(cancellationToken);

        return employees.Select(employee => MapEmployee(employee, departments)).ToList();
    }

    public async Task<IReadOnlyList<ContractPosition>> GetPositionsAsync(CancellationToken cancellationToken = default)
    {
        var departments = await repository.GetDepartmentsAsync(cancellationToken);
        var positions = await repository.GetPositionsAsync(cancellationToken);

        return positions.Select(position => MapPosition(position, departments)).ToList();
    }

    public async Task<IReadOnlyList<ContractLeaveRequest>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default)
    {
        var employees = await repository.GetEmployeesAsync(cancellationToken);
        var leaveRequests = await repository.GetLeaveRequestsAsync(cancellationToken);

        return leaveRequests.Select(request => MapLeaveRequest(request, employees)).ToList();
    }

    public async Task<ContractLeaveRequest> CreateLeaveRequestAsync(SiteCorp.Shared.LeaveRequestDraft draft, CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetEmployeeByIdAsync(draft.EmployeeId, cancellationToken)
            ?? throw new DomainException("No existe un empleado con ese identificador.");

        var leaveRequest = DomainLeaveRequest.Create(draft.EmployeeId, draft.StartDate, draft.EndDate, draft.Reason);

        await repository.AddLeaveRequestAsync(leaveRequest, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapLeaveRequest(leaveRequest, [employee]);
    }

    private static SiteCorp.Shared.DashboardMetrics BuildDashboardMetrics(
        IReadOnlyList<DomainDepartment> departments,
        IReadOnlyList<DomainEmployee> employees,
        IReadOnlyList<DomainPosition> positions,
        IReadOnlyList<DomainLeaveRequest> leaveRequests)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var activeEmployees = employees.Count(employee => employee.Status == DomainEmploymentStatus.Active);
        var averageEngagement = employees.Count == 0
            ? 0
            : Math.Round(employees.Average(employee => employee.EngagementScore), 1);

        return new SiteCorp.Shared.DashboardMetrics(
            TotalEmployees: employees.Count,
            ActiveEmployees: activeEmployees,
            OpenPositions: positions.Count(position => position.CountsAsOpen),
            PendingLeaveRequests: leaveRequests.Count(request => request.Status == DomainLeaveRequestStatus.Pending),
            OnboardingThisMonth: employees.Count(employee =>
                employee.Status == DomainEmploymentStatus.Onboarding
                && employee.HireDate.Year == today.Year
                && employee.HireDate.Month == today.Month),
            AverageEngagement: averageEngagement,
            Departments: BuildDepartmentSummaries(departments, employees, positions));
    }

    private static List<ContractDepartmentSummary> BuildDepartmentSummaries(
        IReadOnlyList<DomainDepartment> departments,
        IReadOnlyList<DomainEmployee> employees,
        IReadOnlyList<DomainPosition> positions)
    {
        return departments
            .OrderBy(department => department.Name)
            .Select(department => new ContractDepartmentSummary(
                department.Id,
                department.Name,
                department.ManagerName,
                employees.Count(employee => employee.DepartmentId == department.Id),
                positions.Count(position => position.DepartmentId == department.Id && position.CountsAsOpen)))
            .ToList();
    }

    private static ContractEmployee MapEmployee(DomainEmployee employee, IReadOnlyList<DomainDepartment> departments)
    {
        var departmentName = departments.FirstOrDefault(department => department.Id == employee.DepartmentId)?.Name
            ?? "Sin departamento";

        return new ContractEmployee(
            employee.Id,
            employee.EmployeeNumber,
            employee.FullName,
            departmentName,
            employee.Position,
            employee.Location,
            employee.HireDate,
            MapEmploymentStatus(employee.Status),
            employee.EngagementScore);
    }

    private static ContractPosition MapPosition(DomainPosition position, IReadOnlyList<DomainDepartment> departments)
    {
        var departmentName = departments.FirstOrDefault(department => department.Id == position.DepartmentId)?.Name
            ?? "Sin departamento";

        return new ContractPosition(
            position.Id,
            position.Title,
            departmentName,
            position.Location,
            MapPositionStatus(position.Status),
            position.Candidates,
            position.TargetStartDate);
    }

    private static ContractLeaveRequest MapLeaveRequest(DomainLeaveRequest request, IReadOnlyList<DomainEmployee> employees)
    {
        var employeeName = employees.FirstOrDefault(employee => employee.Id == request.EmployeeId)?.FullName
            ?? "Empleado no encontrado";

        return new ContractLeaveRequest(
            request.Id,
            request.EmployeeId,
            employeeName,
            request.StartDate,
            request.EndDate,
            request.Reason,
            MapLeaveRequestStatus(request.Status));
    }

    private static ContractEmploymentStatus MapEmploymentStatus(DomainEmploymentStatus status) => status switch
    {
        DomainEmploymentStatus.Active => ContractEmploymentStatus.Active,
        DomainEmploymentStatus.Onboarding => ContractEmploymentStatus.Onboarding,
        DomainEmploymentStatus.OnLeave => ContractEmploymentStatus.OnLeave,
        DomainEmploymentStatus.Offboarding => ContractEmploymentStatus.Offboarding,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    private static ContractPositionStatus MapPositionStatus(DomainPositionStatus status) => status switch
    {
        DomainPositionStatus.Open => ContractPositionStatus.Open,
        DomainPositionStatus.Interviewing => ContractPositionStatus.Interviewing,
        DomainPositionStatus.Offer => ContractPositionStatus.Offer,
        DomainPositionStatus.OnHold => ContractPositionStatus.OnHold,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    private static ContractLeaveRequestStatus MapLeaveRequestStatus(DomainLeaveRequestStatus status) => status switch
    {
        DomainLeaveRequestStatus.Pending => ContractLeaveRequestStatus.Pending,
        DomainLeaveRequestStatus.Approved => ContractLeaveRequestStatus.Approved,
        DomainLeaveRequestStatus.Rejected => ContractLeaveRequestStatus.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };
}

