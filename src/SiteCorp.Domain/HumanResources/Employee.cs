using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources;

public sealed class Employee
{
    private Employee()
    {
        EmployeeNumber = string.Empty;
        FullName = string.Empty;
        Position = string.Empty;
        Location = string.Empty;
    }

    public Employee(
        string employeeNumber,
        string fullName,
        int departmentId,
        string position,
        string location,
        DateOnly hireDate,
        EmploymentStatus status,
        int engagementScore)
    {
        if (departmentId <= 0)
        {
            throw new DomainException("El empleado debe pertenecer a un departamento valido.");
        }

        EmployeeNumber = RequireText(employeeNumber, "El codigo del empleado es obligatorio.");
        FullName = RequireText(fullName, "El nombre del empleado es obligatorio.");
        DepartmentId = departmentId;
        Position = RequireText(position, "El puesto del empleado es obligatorio.");
        Location = RequireText(location, "La ubicacion del empleado es obligatoria.");
        HireDate = hireDate;
        Status = status;
        ChangeEngagementScore(engagementScore);
    }

    public int Id { get; private set; }

    public string EmployeeNumber { get; private set; }

    public string FullName { get; private set; }

    public int DepartmentId { get; private set; }

    public string Position { get; private set; }

    public string Location { get; private set; }

    public DateOnly HireDate { get; private set; }

    public EmploymentStatus Status { get; private set; }

    public int EngagementScore { get; private set; }

    public void Activate() => Status = EmploymentStatus.Active;

    public void BeginOnboarding() => Status = EmploymentStatus.Onboarding;

    public void StartLeave() => Status = EmploymentStatus.OnLeave;

    public void StartOffboarding() => Status = EmploymentStatus.Offboarding;

    public void ChangeEngagementScore(int score)
    {
        if (score is < 0 or > 100)
        {
            throw new DomainException("El engagement debe estar entre 0 y 100.");
        }

        EngagementScore = score;
    }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }
}

