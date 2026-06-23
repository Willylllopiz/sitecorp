using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.Catalogs;

public sealed class EducationLevel : CatalogItem
{
    private EducationLevel()
    {
    }

    public EducationLevel(string code, string description, int hierarchyLevel)
        : base(code, description)
    {
        if (hierarchyLevel < 0)
        {
            throw new DomainException("El nivel jerarquico educacional no puede ser negativo.");
        }

        HierarchyLevel = hierarchyLevel;
    }

    public int HierarchyLevel { get; private set; }
}
