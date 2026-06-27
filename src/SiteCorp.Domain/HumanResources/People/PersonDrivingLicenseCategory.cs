using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.People;

public sealed class PersonDrivingLicenseCategory
{
    private PersonDrivingLicenseCategory()
    {
    }

    public PersonDrivingLicenseCategory(Guid personId, Guid drivingLicenseCategoryId)
    {
        ValidateRequired(personId, "La categoria de licencia debe pertenecer a un candidato.");
        ValidateRequired(drivingLicenseCategoryId, "La categoria de licencia es obligatoria.");

        PersonId = personId;
        DrivingLicenseCategoryId = drivingLicenseCategoryId;
    }

    public Guid PersonId { get; private set; }

    public Guid DrivingLicenseCategoryId { get; private set; }

    private static void ValidateRequired(Guid id, string message)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException(message);
        }
    }
}
