using System.ComponentModel.DataAnnotations.Schema;
using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.Catalogs;

[NotMapped]
public abstract class CatalogItem
{
    protected CatalogItem()
    {
        Code = string.Empty;
        Description = string.Empty;
    }

    protected CatalogItem(string code, string description)
    {
        Id = Guid.NewGuid();
        Code = RequireText(code, "El codigo del catalogo es obligatorio.").ToUpperInvariant();
        Description = RequireText(description, "La descripcion del catalogo es obligatoria.");
        IsActive = true;
    }

    public Guid Id { get; protected set; }

    public string Code { get; private set; }

    public string Description { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(string code, string description)
    {
        Code = RequireText(code, "El codigo del catalogo es obligatorio.").ToUpperInvariant();
        Description = RequireText(description, "La descripcion del catalogo es obligatoria.");
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
}
