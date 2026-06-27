using SiteCorp.Domain.Common;
using SiteCorp.Domain.HumanResources.ValueObjects;

namespace SiteCorp.Domain.HumanResources.People;

public sealed class Person
{
    private Person()
    {
        FullName = new FullName("Sin", "PrimerApellido", "SegundoApellido");
        NationalId = new NationalId("00000");
        Address = new Address("Sin direccion", null, "Sin ciudad", "Sin provincia", null);
        PhysicalData = new PhysicalData(0, 0, null, null, null);
        DefenseSituation = string.Empty;
        CompletedDegree = string.Empty;
        Specialty = string.Empty;
        DisciplinaryMeasures = string.Empty;
        DrivingLicenseCategories = new List<PersonDrivingLicenseCategory>();
    }

    public Person(
        FullName fullName,
        NationalId nationalId,
        DateOnly birthDate,
        Address address,
        int numberOfChildren,
        string? defenseSituation,
        bool preEmploymentCheck,
        string? completedDegree,
        bool hasCriminalRecord,
        bool hasEmploymentContract,
        string? disciplinaryMeasures,
        Guid educationLevelId,
        string? specialty,
        Guid maritalStatusId,
        Guid genderId,
        Guid skinColorId,
        Guid politicalAffiliationId,
        Guid employmentTypeId,
        IReadOnlyCollection<Guid> drivingLicenseCategoryIds,
        Guid retireeRehireStatusId,
        PhysicalData physicalData)
    {
        if (numberOfChildren < 0)
        {
            throw new DomainException("La cantidad de hijos no puede ser negativa.");
        }

        ValidateRequiredCatalog(educationLevelId, "El nivel educacional es obligatorio.");
        ValidateRequiredCatalog(maritalStatusId, "El estado civil es obligatorio.");
        ValidateRequiredCatalog(genderId, "El genero es obligatorio.");
        ValidateRequiredCatalog(skinColorId, "El color de piel es obligatorio.");
        ValidateRequiredCatalog(politicalAffiliationId, "La afiliacion politica es obligatoria.");
        ValidateRequiredCatalog(employmentTypeId, "El tipo de empleo es obligatorio.");
        ValidateRequiredCatalog(retireeRehireStatusId, "La condicion de jubilado/recontratado es obligatoria.");

        Id = Guid.NewGuid();
        FullName = fullName;
        NationalId = nationalId;
        BirthDate = birthDate;
        Address = address;
        NumberOfChildren = numberOfChildren;
        DefenseSituation = Normalize(defenseSituation);
        PreEmploymentCheck = preEmploymentCheck;
        CompletedDegree = Normalize(completedDegree);
        HasCriminalRecord = hasCriminalRecord;
        HasEmploymentContract = hasEmploymentContract;
        DisciplinaryMeasures = Normalize(disciplinaryMeasures);
        EducationLevelId = educationLevelId;
        Specialty = Normalize(specialty);
        MaritalStatusId = maritalStatusId;
        GenderId = genderId;
        SkinColorId = skinColorId;
        PoliticalAffiliationId = politicalAffiliationId;
        EmploymentTypeId = employmentTypeId;
        RetireeRehireStatusId = retireeRehireStatusId;
        PhysicalData = physicalData;
        DrivingLicenseCategories = drivingLicenseCategoryIds
            .Distinct()
            .Select(categoryId => new PersonDrivingLicenseCategory(Id, categoryId))
            .ToList();
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public FullName FullName { get; private set; }

    public NationalId NationalId { get; private set; }

    public DateOnly BirthDate { get; private set; }

    public Address Address { get; private set; }

    public int NumberOfChildren { get; private set; }

    public string? DefenseSituation { get; private set; }

    public bool PreEmploymentCheck { get; private set; }

    public string? CompletedDegree { get; private set; }

    public bool HasCriminalRecord { get; private set; }

    public bool HasEmploymentContract { get; private set; }

    public string? DisciplinaryMeasures { get; private set; }

    public Guid EducationLevelId { get; private set; }

    public string? Specialty { get; private set; }

    public Guid MaritalStatusId { get; private set; }

    public Guid GenderId { get; private set; }

    public Guid SkinColorId { get; private set; }

    public Guid PoliticalAffiliationId { get; private set; }

    public Guid EmploymentTypeId { get; private set; }

    public List<PersonDrivingLicenseCategory> DrivingLicenseCategories { get; private set; }

    public Guid RetireeRehireStatusId { get; private set; }

    public PhysicalData PhysicalData { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private static void ValidateRequiredCatalog(Guid id, string message)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException(message);
        }
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
