namespace SiteCorp.Domain.HumanResources.Organization;

public sealed class BusinessGroup : OrganizationEntity
{
    private BusinessGroup()
    {
    }

    public BusinessGroup(string name, string? description = null)
        : base(name, description, OrganizationEntityType.BusinessGroup)
    {
    }
}
