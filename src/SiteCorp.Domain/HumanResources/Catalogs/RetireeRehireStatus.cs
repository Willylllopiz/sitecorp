namespace SiteCorp.Domain.HumanResources.Catalogs;

public sealed class RetireeRehireStatus : CatalogItem
{
    private RetireeRehireStatus()
    {
    }

    public RetireeRehireStatus(string code, string description)
        : base(code, description)
    {
    }
}
