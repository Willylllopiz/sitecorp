using Microsoft.EntityFrameworkCore;
using SiteCorp.Application.HumanResources;
using SiteCorp.Domain.HumanResources.Catalogs;
using SiteCorp.Domain.HumanResources.Organization;
using SiteCorp.Domain.HumanResources.People;
using SiteCorp.Domain.HumanResources.Staffing;
using OrgCompany = SiteCorp.Domain.HumanResources.Organization.Company;
using StaffingPosition = SiteCorp.Domain.HumanResources.Staffing.Position;

namespace SiteCorp.Infrastructure.Data;

public sealed class EfHumanResourcesRepository(SiteCorpDbContext dbContext) : IHumanResourcesRepository
{
    public async Task<IReadOnlyList<OrganizationEntity>> GetOrganizationEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.OrganizationEntities
            .AsNoTracking()
            .OrderBy(entity => entity.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<OrganizationEntity?> GetOrganizationEntityByIdAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        return dbContext.OrganizationEntities.FirstOrDefaultAsync(entity => entity.Id == entityId, cancellationToken);
    }

    public Task<BusinessGroup?> GetBusinessGroupByIdAsync(Guid businessGroupId, CancellationToken cancellationToken = default)
    {
        return dbContext.BusinessGroups.FirstOrDefaultAsync(group => group.Id == businessGroupId, cancellationToken);
    }

    public Task<OrgCompany?> GetOrganizationCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return dbContext.OrganizationCompanies.FirstOrDefaultAsync(company => company.Id == companyId, cancellationToken);
    }

    public async Task AddBusinessGroupAsync(BusinessGroup businessGroup, CancellationToken cancellationToken = default)
    {
        await dbContext.BusinessGroups.AddAsync(businessGroup, cancellationToken);
    }

    public async Task AddCompanyAsync(OrgCompany company, CancellationToken cancellationToken = default)
    {
        await dbContext.OrganizationCompanies.AddAsync(company, cancellationToken);
    }

    public async Task AddBusinessUnitAsync(BusinessUnit businessUnit, CancellationToken cancellationToken = default)
    {
        await dbContext.BusinessUnits.AddAsync(businessUnit, cancellationToken);
    }

    public async Task<IReadOnlyList<Gender>> GetGendersAsync(CancellationToken cancellationToken = default)
    {
        return await GetCatalogsAsync(dbContext.Genders, cancellationToken);
    }

    public async Task<IReadOnlyList<SkinColor>> GetSkinColorsAsync(CancellationToken cancellationToken = default)
    {
        return await GetCatalogsAsync(dbContext.SkinColors, cancellationToken);
    }

    public async Task<IReadOnlyList<PoliticalAffiliation>> GetPoliticalAffiliationsAsync(CancellationToken cancellationToken = default)
    {
        return await GetCatalogsAsync(dbContext.PoliticalAffiliations, cancellationToken);
    }

    public async Task<IReadOnlyList<MaritalStatus>> GetMaritalStatusesAsync(CancellationToken cancellationToken = default)
    {
        return await GetCatalogsAsync(dbContext.MaritalStatuses, cancellationToken);
    }

    public async Task<IReadOnlyList<EducationLevel>> GetEducationLevelsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.EducationLevels
            .AsNoTracking()
            .OrderBy(item => item.HierarchyLevel)
            .ThenBy(item => item.Description)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmploymentType>> GetEmploymentTypesAsync(CancellationToken cancellationToken = default)
    {
        return await GetCatalogsAsync(dbContext.EmploymentTypes, cancellationToken);
    }

    public async Task<IReadOnlyList<DrivingLicenseCategory>> GetDrivingLicenseCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await GetCatalogsAsync(dbContext.DrivingLicenseCategories, cancellationToken);
    }

    public async Task<IReadOnlyList<RetireeRehireStatus>> GetRetireeRehireStatusesAsync(CancellationToken cancellationToken = default)
    {
        return await GetCatalogsAsync(dbContext.RetireeRehireStatuses, cancellationToken);
    }

    public Task<bool> CatalogItemExistsAsync<TCatalog>(Guid id, CancellationToken cancellationToken = default)
        where TCatalog : CatalogItem
    {
        return dbContext.Set<TCatalog>().AnyAsync(item => item.Id == id && item.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<Person>> GetPeopleAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Persons
            .AsNoTracking()
            .Include(person => person.DrivingLicenseCategories)
            .OrderBy(person => person.FullName.FirstLastName)
            .ThenBy(person => person.FullName.SecondLastName)
            .ThenBy(person => person.FullName.FirstName)
            .ToListAsync(cancellationToken);
    }

    public Task<Person?> GetPersonByIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return dbContext.Persons.FirstOrDefaultAsync(person => person.Id == personId, cancellationToken);
    }

    public Task<Person?> GetPersonByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
    {
        var normalized = nationalId.Trim();
        return dbContext.Persons.FirstOrDefaultAsync(person => person.NationalId.Number == normalized, cancellationToken);
    }

    public async Task AddPersonAsync(Person person, CancellationToken cancellationToken = default)
    {
        await dbContext.Persons.AddAsync(person, cancellationToken);
    }

    public async Task<IReadOnlyList<StaffingPosition>> GetStaffingPositionsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.StaffingPositions
            .AsNoTracking()
            .OrderBy(position => position.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<StaffingPosition?> GetStaffingPositionByIdAsync(Guid positionId, CancellationToken cancellationToken = default)
    {
        return dbContext.StaffingPositions.FirstOrDefaultAsync(position => position.Id == positionId, cancellationToken);
    }

    public async Task AddStaffingPositionAsync(StaffingPosition position, CancellationToken cancellationToken = default)
    {
        await dbContext.StaffingPositions.AddAsync(position, cancellationToken);
    }

    public Task<JobTemplate?> GetActiveJobTemplateByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        return dbContext.JobTemplates.FirstOrDefaultAsync(
            template => template.EntityId == entityId && template.IsActive,
            cancellationToken);
    }

    public async Task<IReadOnlyList<JobTemplate>> GetJobTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.JobTemplates
            .AsNoTracking()
            .OrderByDescending(template => template.EffectiveDate)
            .ToListAsync(cancellationToken);
    }

    public Task<JobTemplate?> GetJobTemplateByIdAsync(Guid jobTemplateId, CancellationToken cancellationToken = default)
    {
        return dbContext.JobTemplates.FirstOrDefaultAsync(template => template.Id == jobTemplateId, cancellationToken);
    }

    public async Task AddJobTemplateAsync(JobTemplate jobTemplate, CancellationToken cancellationToken = default)
    {
        await dbContext.JobTemplates.AddAsync(jobTemplate, cancellationToken);
    }

    public async Task<IReadOnlyList<JobTemplatePosition>> GetJobTemplatePositionsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.JobTemplatePositions
            .AsNoTracking()
            .OrderBy(position => position.JobTemplateId)
            .ToListAsync(cancellationToken);
    }

    public Task<JobTemplatePosition?> GetJobTemplatePositionAsync(Guid jobTemplateId, Guid positionId, CancellationToken cancellationToken = default)
    {
        return dbContext.JobTemplatePositions.FirstOrDefaultAsync(
            position => position.JobTemplateId == jobTemplateId && position.PositionId == positionId,
            cancellationToken);
    }

    public async Task AddJobTemplatePositionAsync(JobTemplatePosition jobTemplatePosition, CancellationToken cancellationToken = default)
    {
        await dbContext.JobTemplatePositions.AddAsync(jobTemplatePosition, cancellationToken);
    }

    public async Task<IReadOnlyList<EmploymentHistory>> GetEmploymentHistoriesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.EmploymentHistories
            .AsNoTracking()
            .OrderByDescending(history => history.Period.StartDate)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasActiveEmploymentAsync(Guid personId, Guid entityId, Guid positionId, CancellationToken cancellationToken = default)
    {
        return dbContext.EmploymentHistories.AnyAsync(
            history => history.PersonId == personId
                && history.EntityId == entityId
                && history.PositionId == positionId
                && history.IsActive,
            cancellationToken);
    }

    public async Task AddEmploymentHistoryAsync(EmploymentHistory employmentHistory, CancellationToken cancellationToken = default)
    {
        await dbContext.EmploymentHistories.AddAsync(employmentHistory, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<IReadOnlyList<TCatalog>> GetCatalogsAsync<TCatalog>(
        IQueryable<TCatalog> query,
        CancellationToken cancellationToken)
        where TCatalog : CatalogItem
    {
        return await query
            .AsNoTracking()
            .OrderBy(item => item.Description)
            .ToListAsync(cancellationToken);
    }
}
