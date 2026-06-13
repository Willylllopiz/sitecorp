using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.Authentication;

public sealed class Permission
{
    private Permission()
    {
    }

    public Permission(string code, string name, string module, string? description = null)
    {
        Code = RequireText(code, "El codigo del permiso es obligatorio.");
        Name = RequireText(name, "El nombre del permiso es obligatorio.");
        Module = RequireText(module, "El modulo del permiso es obligatorio.");
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string Module { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }
}

