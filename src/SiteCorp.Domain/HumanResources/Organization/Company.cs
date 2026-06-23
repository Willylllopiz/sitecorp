using SiteCorp.Domain.Common;
using SiteCorp.Domain.HumanResources.ValueObjects;

namespace SiteCorp.Domain.HumanResources.Organization;

public sealed class Company : OrganizationEntity
{
    private Company()
    {
        Address = new Address("Sin direccion", null, "Sin ciudad", "Sin provincia", null);
    }

    public Company(Guid businessGroupId, string name, string? description, Address address)
        : base(name, description, OrganizationEntityType.Company)
    {
        if (businessGroupId == Guid.Empty)
        {
            throw new DomainException("La empresa debe pertenecer a un grupo empresarial.");
        }

        BusinessGroupId = businessGroupId;
        Address = address;
    }

    public Guid BusinessGroupId { get; private set; }

    public Address Address { get; private set; }

    public void ChangeAddress(Address address)
    {
        Address = address;
    }
}
