namespace SiteCorp.Domain.HumanResources.Catalogs;

public sealed class Gender : CatalogItem
{
    private Gender()
    {
    }

    public Gender(string code, string description)
        : base(code, description)
    {
    }
}
