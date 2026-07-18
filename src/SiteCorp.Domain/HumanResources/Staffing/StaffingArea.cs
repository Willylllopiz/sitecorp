using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.Staffing;

public sealed class StaffingArea
{
    private StaffingArea()
    {
        Name = string.Empty;
    }

    public StaffingArea(string name, int priority)
    {
        Id = Guid.NewGuid();
        Name = RequireText(name);
        Priority = RequirePriority(priority);
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public int Priority { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(string name, int priority)
    {
        Name = RequireText(name);
        Priority = RequirePriority(priority);
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string RequireText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("El nombre del area es obligatorio.");
        }

        return value.Trim();
    }

    private static int RequirePriority(int priority)
    {
        if (priority < 0)
        {
            throw new DomainException("La prioridad del area no puede ser negativa.");
        }

        return priority;
    }
}
