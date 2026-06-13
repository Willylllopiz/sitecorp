using SiteCorp.Shared;

namespace SiteCorp.Web.Services;

public sealed class SampleHumanResourcesData
{
    private readonly IReadOnlyList<DepartmentSummary> _departments =
    [
        new(1, "Talento y Cultura", "Mariela Fuentes", 18, 2),
        new(2, "Operaciones", "Daniel Crespo", 42, 3),
        new(3, "Finanzas", "Paula Herrera", 15, 1),
        new(4, "Tecnologia", "Andres Molina", 26, 4)
    ];

    private readonly IReadOnlyList<Employee> _employees =
    [
        new(1, "SC-1001", "Laura Benitez", "Talento y Cultura", "HR Business Partner", "Habana", new DateOnly(2023, 3, 14), EmploymentStatus.Active, 91),
        new(2, "SC-1002", "Rafael Suarez", "Operaciones", "Supervisor de Turno", "Santiago", new DateOnly(2021, 8, 9), EmploymentStatus.Active, 78),
        new(3, "SC-1003", "Camila Torres", "Finanzas", "Analista de Nomina", "Habana", new DateOnly(2024, 1, 22), EmploymentStatus.OnLeave, 86),
        new(4, "SC-1004", "Miguel Duarte", "Tecnologia", "Desarrollador Backend", "Remoto", new DateOnly(2025, 11, 3), EmploymentStatus.Active, 84),
        new(5, "SC-1005", "Sofia Ramos", "Talento y Cultura", "Recruiter", "Habana", new DateOnly(2026, 6, 3), EmploymentStatus.Onboarding, 88),
        new(6, "SC-1006", "Ivan Castillo", "Operaciones", "Coordinador de Logistica", "Matanzas", new DateOnly(2022, 5, 18), EmploymentStatus.Active, 73)
    ];

    private readonly IReadOnlyList<Position> _positions =
    [
        new(1, "Especialista de Compensacion", "Talento y Cultura", "Habana", PositionStatus.Interviewing, 7, new DateOnly(2026, 7, 15)),
        new(2, "Ingeniero DevOps", "Tecnologia", "Remoto", PositionStatus.Open, 11, new DateOnly(2026, 8, 1)),
        new(3, "Analista Financiero", "Finanzas", "Habana", PositionStatus.Offer, 3, new DateOnly(2026, 7, 1)),
        new(4, "Jefe de Almacen", "Operaciones", "Santiago", PositionStatus.Open, 5, new DateOnly(2026, 7, 22))
    ];

    private readonly IReadOnlyList<LeaveRequest> _leaveRequests =
    [
        new(1, 2, "Rafael Suarez", new DateOnly(2026, 6, 18), new DateOnly(2026, 6, 21), "Vacaciones", LeaveRequestStatus.Pending),
        new(2, 3, "Camila Torres", new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 14), "Asuntos medicos", LeaveRequestStatus.Approved),
        new(3, 6, "Ivan Castillo", new DateOnly(2026, 7, 4), new DateOnly(2026, 7, 6), "Tramites personales", LeaveRequestStatus.Pending)
    ];

    public HumanResourcesSnapshot CreateSnapshot(bool isLive)
    {
        var metrics = new DashboardMetrics(
            TotalEmployees: _employees.Count,
            ActiveEmployees: _employees.Count(employee => employee.Status == EmploymentStatus.Active),
            OpenPositions: _positions.Count(position => position.Status is PositionStatus.Open or PositionStatus.Interviewing or PositionStatus.Offer),
            PendingLeaveRequests: _leaveRequests.Count(request => request.Status == LeaveRequestStatus.Pending),
            OnboardingThisMonth: _employees.Count(employee => employee.Status == EmploymentStatus.Onboarding),
            AverageEngagement: Math.Round(_employees.Average(employee => employee.EngagementScore), 1),
            Departments: _departments);

        return new HumanResourcesSnapshot(metrics, _employees, _positions, _leaveRequests, isLive);
    }
}

