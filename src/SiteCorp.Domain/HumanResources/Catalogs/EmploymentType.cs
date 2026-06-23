namespace SiteCorp.Domain.HumanResources.Catalogs;

public sealed class EmploymentType : CatalogItem
{
    private EmploymentType()
    {
    }

    public EmploymentType(string code, string description)
        : base(code, description)
    {
    }
}
