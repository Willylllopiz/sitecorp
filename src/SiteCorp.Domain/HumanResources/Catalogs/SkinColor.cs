namespace SiteCorp.Domain.HumanResources.Catalogs;

public sealed class SkinColor : CatalogItem
{
    private SkinColor()
    {
    }

    public SkinColor(string code, string description)
        : base(code, description)
    {
    }
}
