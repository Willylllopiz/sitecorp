using SiteCorp.Domain.Common;
using SiteCorp.Domain.HumanResources.ValueObjects;

namespace SiteCorp.Domain.HumanResources.Organization;

public sealed class BusinessUnit : OrganizationEntity
{
    private BusinessUnit()
    {
        Address = new Address("Sin direccion", null, "Sin ciudad", "Sin provincia", null);
    }

    public BusinessUnit(Guid companyId, string name, string? description, Address address)
        : base(name, description, OrganizationEntityType.BusinessUnit)
    {
        if (companyId == Guid.Empty)
        {
            throw new DomainException("La UEB debe pertenecer a una empresa.");
        }

        CompanyId = companyId;
        Address = address;
    }

    public Guid CompanyId { get; private set; }

    public Address Address { get; private set; }

    public void ChangeAddress(Address address)
    {
        Address = address;
    }
}
