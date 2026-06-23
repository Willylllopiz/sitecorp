namespace SiteCorp.Domain.HumanResources.Catalogs;

public sealed class PoliticalAffiliation : CatalogItem
{
    private PoliticalAffiliation()
    {
    }

    public PoliticalAffiliation(string code, string description)
        : base(code, description)
    {
    }
}
