namespace SiteCorp.Shared;

public enum EmploymentStatus
{
    Active,
    Onboarding,
    OnLeave,
    Offboarding
}

public enum PositionStatus
{
    Open,
    Interviewing,
    Offer,
    OnHold
}

public enum LeaveRequestStatus
{
    Pending,
    Approved,
    Rejected
}

public sealed record DepartmentSummary(
    int Id,
    string Name,
    string ManagerName,
    int HeadCount,
    int OpenRoles);

public sealed record Employee(
    int Id,
    string EmployeeNumber,
    string FullName,
    string Department,
    string Position,
    string Location,
    DateOnly HireDate,
    EmploymentStatus Status,
    int EngagementScore);

public sealed record Position(
    int Id,
    string Title,
    string Department,
    string Location,
    PositionStatus Status,
    int Candidates,
    DateOnly TargetStartDate);

public sealed record LeaveRequest(
    int Id,
    int EmployeeId,
    string EmployeeName,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason,
    LeaveRequestStatus Status);

public sealed record LeaveRequestDraft(
    int EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason);

public sealed record DashboardMetrics(
    int TotalEmployees,
    int ActiveEmployees,
    int OpenPositions,
    int PendingLeaveRequests,
    int OnboardingThisMonth,
    double AverageEngagement,
    IReadOnlyList<DepartmentSummary> Departments);

public sealed record HumanResourcesSnapshot(
    DashboardMetrics Metrics,
    IReadOnlyList<Employee> Employees,
    IReadOnlyList<Position> Positions,
    IReadOnlyList<LeaveRequest> LeaveRequests,
    bool IsLive);

