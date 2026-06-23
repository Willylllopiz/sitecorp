using SiteCorp.Domain.HumanResources.Catalogs;
using SiteCorp.Domain.HumanResources.Organization;
using SiteCorp.Domain.HumanResources.People;
using SiteCorp.Domain.HumanResources.Staffing;

namespace SiteCorp.Application.HumanResources;

public interface IHumanResourcesRepository
{
    Task<IReadOnlyList<OrganizationEntity>> GetOrganizationEntitiesAsync(CancellationToken cancellationToken = default);

    Task<OrganizationEntity?> GetOrganizationEntityByIdAsync(Guid entityId, CancellationToken cancellationToken = default);

    Task<BusinessGroup?> GetBusinessGroupByIdAsync(Guid businessGroupId, CancellationToken cancellationToken = default);

    Task<Company?> GetOrganizationCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default);

    Task AddBusinessGroupAsync(BusinessGroup businessGroup, CancellationToken cancellationToken = default);

    Task AddCompanyAsync(Company company, CancellationToken cancellationToken = default);

    Task AddBusinessUnitAsync(BusinessUnit businessUnit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Gender>> GetGendersAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SkinColor>> GetSkinColorsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PoliticalAffiliation>> GetPoliticalAffiliationsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MaritalStatus>> GetMaritalStatusesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EducationLevel>> GetEducationLevelsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmploymentType>> GetEmploymentTypesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DrivingLicenseCategory>> GetDrivingLicenseCategoriesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RetireeRehireStatus>> GetRetireeRehireStatusesAsync(CancellationToken cancellationToken = default);

    Task<bool> CatalogItemExistsAsync<TCatalog>(Guid id, CancellationToken cancellationToken = default)
        where TCatalog : CatalogItem;

    Task<IReadOnlyList<Person>> GetPeopleAsync(CancellationToken cancellationToken = default);

    Task<Person?> GetPersonByIdAsync(Guid personId, CancellationToken cancellationToken = default);

    Task<Person?> GetPersonByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);

    Task AddPersonAsync(Person person, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Position>> GetStaffingPositionsAsync(CancellationToken cancellationToken = default);

    Task<Position?> GetStaffingPositionByIdAsync(Guid positionId, CancellationToken cancellationToken = default);

    Task AddStaffingPositionAsync(Position position, CancellationToken cancellationToken = default);

    Task<JobTemplate?> GetActiveJobTemplateByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JobTemplate>> GetJobTemplatesAsync(CancellationToken cancellationToken = default);

    Task<JobTemplate?> GetJobTemplateByIdAsync(Guid jobTemplateId, CancellationToken cancellationToken = default);

    Task AddJobTemplateAsync(JobTemplate jobTemplate, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JobTemplatePosition>> GetJobTemplatePositionsAsync(CancellationToken cancellationToken = default);

    Task<JobTemplatePosition?> GetJobTemplatePositionAsync(Guid jobTemplateId, Guid positionId, CancellationToken cancellationToken = default);

    Task AddJobTemplatePositionAsync(JobTemplatePosition jobTemplatePosition, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmploymentHistory>> GetEmploymentHistoriesAsync(CancellationToken cancellationToken = default);

    Task<bool> HasActiveEmploymentAsync(Guid personId, Guid entityId, Guid positionId, CancellationToken cancellationToken = default);

    Task AddEmploymentHistoryAsync(EmploymentHistory employmentHistory, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
