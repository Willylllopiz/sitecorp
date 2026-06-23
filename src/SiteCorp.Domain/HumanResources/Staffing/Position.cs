using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.Staffing;

public sealed class Position
{
    private Position()
    {
        Code = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        Category = string.Empty;
    }

    public Position(string code, string name, string? description, string category)
    {
        Id = Guid.NewGuid();
        Code = RequireText(code, "El codigo del cargo es obligatorio.").ToUpperInvariant();
        Name = RequireText(name, "El nombre del cargo es obligatorio.");
        Description = Normalize(description);
        Category = RequireText(category, "La categoria del cargo es obligatoria.");
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public string Category { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(string code, string name, string? description, string category)
    {
        Code = RequireText(code, "El codigo del cargo es obligatorio.").ToUpperInvariant();
        Name = RequireText(name, "El nombre del cargo es obligatorio.");
        Description = Normalize(description);
        Category = RequireText(category, "La categoria del cargo es obligatoria.");
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
