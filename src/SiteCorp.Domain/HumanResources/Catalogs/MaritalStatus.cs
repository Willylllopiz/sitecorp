namespace SiteCorp.Domain.HumanResources.Catalogs;

public sealed class MaritalStatus : CatalogItem
{
    private MaritalStatus()
    {
    }

    public MaritalStatus(string code, string description)
        : base(code, description)
    {
    }
}
