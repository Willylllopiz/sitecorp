namespace SiteCorp.Domain.HumanResources.Catalogs;

public sealed class DrivingLicenseCategory : CatalogItem
{
    private DrivingLicenseCategory()
    {
    }

    public DrivingLicenseCategory(string code, string description)
        : base(code, description)
    {
    }
}
