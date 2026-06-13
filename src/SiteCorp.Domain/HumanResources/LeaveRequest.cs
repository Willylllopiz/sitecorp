using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources;

public sealed class LeaveRequest
{
    private LeaveRequest()
    {
        Reason = string.Empty;
    }

    private LeaveRequest(int employeeId, DateOnly startDate, DateOnly endDate, string reason)
    {
        if (employeeId <= 0)
        {
            throw new DomainException("La solicitud debe pertenecer a un empleado valido.");
        }

        if (endDate < startDate)
        {
            throw new DomainException("La fecha final no puede ser anterior a la fecha inicial.");
        }

        EmployeeId = employeeId;
        StartDate = startDate;
        EndDate = endDate;
        Reason = RequireText(reason, "El motivo de la licencia es obligatorio.");
        Status = LeaveRequestStatus.Pending;
    }

    public int Id { get; private set; }

    public int EmployeeId { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public string Reason { get; private set; }

    public LeaveRequestStatus Status { get; private set; }

    public static LeaveRequest Create(int employeeId, DateOnly startDate, DateOnly endDate, string reason)
    {
        return new LeaveRequest(employeeId, startDate, endDate, reason);
    }

    public void Approve()
    {
        EnsurePending();
        Status = LeaveRequestStatus.Approved;
    }

    public void Reject()
    {
        EnsurePending();
        Status = LeaveRequestStatus.Rejected;
    }

    private void EnsurePending()
    {
        if (Status != LeaveRequestStatus.Pending)
        {
            throw new DomainException("Solo las solicitudes pendientes pueden cambiar de estado.");
        }
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

