using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.ValueObjects;

public sealed class EmploymentPeriod
{
    private EmploymentPeriod()
    {
    }

    public EmploymentPeriod(DateOnly startDate, DateOnly? endDate = null)
    {
        if (endDate is not null && endDate < startDate)
        {
            throw new DomainException("La fecha de salida no puede ser anterior a la fecha de entrada.");
        }

        StartDate = startDate;
        EndDate = endDate;
    }

    public DateOnly StartDate { get; private set; }

    public DateOnly? EndDate { get; private set; }

    public bool IsActive() => EndDate is null;

    public int DaysWorked(DateOnly? currentDate = null)
    {
        var endDate = EndDate ?? currentDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        return endDate.DayNumber - StartDate.DayNumber;
    }
}
