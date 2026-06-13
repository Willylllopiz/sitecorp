using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.Authentication;

public sealed class Role
{
    private Role()
    {
    }

    public Role(int companyId, string name, string? description, bool isSystemRole = false)
    {
        if (companyId <= 0)
        {
            throw new DomainException("El rol debe pertenecer a una empresa valida.");
        }

        CompanyId = companyId;
        Rename(name);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        IsSystemRole = isSystemRole;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }

    public int CompanyId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string NormalizedName { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsSystemRole { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del rol es obligatorio.");
        }

        Name = name.Trim();
        NormalizedName = Name.ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        if (IsSystemRole)
        {
            throw new DomainException("Los roles del sistema no se pueden desactivar.");
        }

        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

