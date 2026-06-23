using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.Organization;

public abstract class OrganizationEntity
{
    protected OrganizationEntity()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    protected OrganizationEntity(string name, string? description, OrganizationEntityType entityType)
    {
        Id = Guid.NewGuid();
        Name = RequireText(name, "El nombre de la entidad es obligatorio.");
        Description = Normalize(description);
        EntityType = entityType;
        CreatedDate = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    public Guid Id { get; protected set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public OrganizationEntityType EntityType { get; private set; }

    public DateTimeOffset CreatedDate { get; private set; }

    public bool IsActive { get; private set; }

    public void Rename(string name, string? description)
    {
        Name = RequireText(name, "El nombre de la entidad es obligatorio.");
        Description = Normalize(description);
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    protected static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }

    protected static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
