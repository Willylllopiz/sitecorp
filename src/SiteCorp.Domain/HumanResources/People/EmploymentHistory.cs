using SiteCorp.Domain.Common;
using SiteCorp.Domain.HumanResources.ValueObjects;

namespace SiteCorp.Domain.HumanResources.People;

public sealed class EmploymentHistory
{
    private EmploymentHistory()
    {
        Period = new EmploymentPeriod(DateOnly.FromDateTime(DateTime.UtcNow));
        Notes = string.Empty;
    }

    public EmploymentHistory(
        Guid personId,
        Guid entityId,
        Guid positionId,
        Guid jobTemplatePositionId,
        DateOnly startDate,
        string? notes = null)
    {
        ValidateRequired(personId, "El historial laboral debe pertenecer a una persona.");
        ValidateRequired(entityId, "El historial laboral debe pertenecer a una entidad.");
        ValidateRequired(positionId, "El historial laboral debe referenciar un cargo.");
        ValidateRequired(jobTemplatePositionId, "El historial laboral debe referenciar una plaza de plantilla.");

        Id = Guid.NewGuid();
        PersonId = personId;
        EntityId = entityId;
        PositionId = positionId;
        JobTemplatePositionId = jobTemplatePositionId;
        Period = new EmploymentPeriod(startDate);
        Notes = Normalize(notes);
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid PersonId { get; private set; }

    public Guid EntityId { get; private set; }

    public Guid PositionId { get; private set; }

    public Guid JobTemplatePositionId { get; private set; }

    public EmploymentPeriod Period { get; private set; }

    public string? ExitReason { get; private set; }

    public string? Notes { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public void End(DateOnly endDate, string exitReason)
    {
        if (!IsActive)
        {
            throw new DomainException("Esta relacion laboral ya esta cerrada.");
        }

        Period = new EmploymentPeriod(Period.StartDate, endDate);
        ExitReason = string.IsNullOrWhiteSpace(exitReason)
            ? throw new DomainException("El motivo de salida es obligatorio.")
            : exitReason.Trim();
        IsActive = false;
    }

    private static void ValidateRequired(Guid id, string message)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException(message);
        }
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
