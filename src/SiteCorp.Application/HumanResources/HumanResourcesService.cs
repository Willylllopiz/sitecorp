using SiteCorp.Domain.Common;
using SiteCorp.Domain.HumanResources.Catalogs;
using SiteCorp.Domain.HumanResources.Organization;
using SiteCorp.Domain.HumanResources.People;
using SiteCorp.Domain.HumanResources.ValueObjects;
using DomainPosition = SiteCorp.Domain.HumanResources.Staffing.Position;
using JobTemplate = SiteCorp.Domain.HumanResources.Staffing.JobTemplate;
using JobTemplatePosition = SiteCorp.Domain.HumanResources.Staffing.JobTemplatePosition;
using StaffingArea = SiteCorp.Domain.HumanResources.Staffing.StaffingArea;

namespace SiteCorp.Application.HumanResources;

public sealed class HumanResourcesService(IHumanResourcesRepository repository)
{
    public async Task<SiteCorp.Shared.HumanResourcesSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var metrics = await GetDashboardMetricsAsync(cancellationToken);
        var employees = await GetEmployeesAsync(cancellationToken);
        var positions = await GetPositionsAsync(cancellationToken);
        var leaveRequests = await GetLeaveRequestsAsync(cancellationToken);

        return new SiteCorp.Shared.HumanResourcesSnapshot(metrics, employees, positions, leaveRequests, IsLive: true);
    }

    public async Task<SiteCorp.Shared.DashboardMetrics> GetDashboardMetricsAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetOrganizationEntitiesAsync(cancellationToken);
        var people = await repository.GetPeopleAsync(cancellationToken);
        var histories = await repository.GetEmploymentHistoriesAsync(cancellationToken);
        var templatePositions = await repository.GetJobTemplatePositionsAsync(cancellationToken);
        var positions = await repository.GetStaffingPositionsAsync(cancellationToken);

        var activeHistories = histories.Where(history => history.IsActive).ToList();
        var currentMonth = DateOnly.FromDateTime(DateTime.UtcNow);
        var activeByEntity = activeHistories
            .GroupBy(history => history.EntityId)
            .ToDictionary(group => group.Key, group => group.Count());
        var openByEntity = await BuildOpenPositionsByEntityAsync(templatePositions, cancellationToken);

        var departments = entities
            .Where(entity => entity.IsActive)
            .OrderBy(entity => entity.Name)
            .Select((entity, index) => new SiteCorp.Shared.DepartmentSummary(
                index + 1,
                entity.Name,
                entity.EntityType.ToString(),
                activeByEntity.GetValueOrDefault(entity.Id),
                openByEntity.GetValueOrDefault(entity.Id)))
            .ToList();

        return new SiteCorp.Shared.DashboardMetrics(
            TotalEmployees: people.Count,
            ActiveEmployees: activeHistories.Count,
            OpenPositions: templatePositions.Count(position => position.HasAvailability()),
            PendingLeaveRequests: 0,
            OnboardingThisMonth: activeHistories.Count(history =>
                history.Period.StartDate.Year == currentMonth.Year && history.Period.StartDate.Month == currentMonth.Month),
            AverageEngagement: 0,
            Departments: departments);
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.DepartmentSummary>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return (await GetDashboardMetricsAsync(cancellationToken)).Departments;
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var people = await repository.GetPeopleAsync(cancellationToken);
        var histories = await repository.GetEmploymentHistoriesAsync(cancellationToken);
        var entities = await repository.GetOrganizationEntitiesAsync(cancellationToken);
        var positions = await repository.GetStaffingPositionsAsync(cancellationToken);

        var peopleById = people.ToDictionary(person => person.Id);
        var entitiesById = entities.ToDictionary(entity => entity.Id);
        var positionsById = positions.ToDictionary(position => position.Id);

        return histories
            .Where(history => history.IsActive)
            .OrderBy(history => peopleById.GetValueOrDefault(history.PersonId)?.FullName.Value)
            .Select((history, index) =>
            {
                var person = peopleById.GetValueOrDefault(history.PersonId);
                var entity = entitiesById.GetValueOrDefault(history.EntityId);
                var position = positionsById.GetValueOrDefault(history.PositionId);

                return new SiteCorp.Shared.Employee(
                    index + 1,
                    person?.NationalId.Number ?? "SIN-ID",
                    person?.FullName.Value ?? "Candidato no encontrado",
                    entity?.Name ?? "Entidad no encontrada",
                    position?.Name ?? "Cargo no encontrado",
                    entity?.EntityType.ToString() ?? "N/A",
                    history.Period.StartDate,
                    SiteCorp.Shared.EmploymentStatus.Active,
                    0);
            })
            .ToList();
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.Position>> GetPositionsAsync(CancellationToken cancellationToken = default)
    {
        var templatePositions = await repository.GetJobTemplatePositionsAsync(cancellationToken);
        var positions = await repository.GetStaffingPositionsAsync(cancellationToken);
        var areas = await repository.GetStaffingAreasAsync(cancellationToken);
        var templates = await BuildTemplatesByIdAsync(templatePositions, cancellationToken);
        var entities = await repository.GetOrganizationEntitiesAsync(cancellationToken);

        var positionsById = positions.ToDictionary(position => position.Id);
        var areasById = areas.ToDictionary(area => area.Id);
        var entitiesById = entities.ToDictionary(entity => entity.Id);

        return templatePositions
            .OrderBy(position => areasById.GetValueOrDefault(position.AreaId)?.Priority ?? int.MaxValue)
            .ThenBy(position => areasById.GetValueOrDefault(position.AreaId)?.Name)
            .ThenBy(position => positionsById.GetValueOrDefault(position.PositionId)?.Name)
            .Select((templatePosition, index) =>
            {
                var position = positionsById.GetValueOrDefault(templatePosition.PositionId);
                var template = templates.GetValueOrDefault(templatePosition.JobTemplateId);
                var entity = template is null ? null : entitiesById.GetValueOrDefault(template.EntityId);

                return new SiteCorp.Shared.Position(
                    index + 1,
                    position?.Name ?? "Cargo no encontrado",
                    entity?.Name ?? "Entidad no encontrada",
                    templatePosition.Vacancy.SalaryCategory,
                    templatePosition.HasAvailability()
                        ? SiteCorp.Shared.PositionStatus.Open
                        : SiteCorp.Shared.PositionStatus.OnHold,
                    0,
                    template?.EffectiveDate ?? DateOnly.FromDateTime(DateTime.UtcNow));
            })
            .ToList();
    }

    public Task<IReadOnlyList<SiteCorp.Shared.LeaveRequest>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<SiteCorp.Shared.LeaveRequest>>([]);
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.OrganizationEntityResponse>> GetOrganizationEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetOrganizationEntitiesAsync(cancellationToken);
        return entities.Select(MapOrganizationEntity).ToList();
    }

    public async Task<SiteCorp.Shared.OrganizationEntityResponse> CreateBusinessGroupAsync(
        SiteCorp.Shared.CreateBusinessGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        var businessGroup = new BusinessGroup(request.Name, request.Description);
        await repository.AddBusinessGroupAsync(businessGroup, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapOrganizationEntity(businessGroup);
    }

    public async Task<SiteCorp.Shared.OrganizationEntityResponse> CreateCompanyAsync(
        SiteCorp.Shared.CreateCompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = await repository.GetBusinessGroupByIdAsync(request.BusinessGroupId, cancellationToken)
            ?? throw new DomainException("No existe el grupo empresarial indicado.");

        var company = new Company(
            request.BusinessGroupId,
            request.Name,
            request.Description,
            MapAddress(request.Address));

        await repository.AddCompanyAsync(company, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapOrganizationEntity(company);
    }

    public async Task<SiteCorp.Shared.OrganizationEntityResponse> CreateBusinessUnitAsync(
        SiteCorp.Shared.CreateBusinessUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = await repository.GetOrganizationCompanyByIdAsync(request.CompanyId, cancellationToken)
            ?? throw new DomainException("No existe la empresa indicada.");

        var businessUnit = new BusinessUnit(
            request.CompanyId,
            request.Name,
            request.Description,
            MapAddress(request.Address));

        await repository.AddBusinessUnitAsync(businessUnit, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapOrganizationEntity(businessUnit);
    }

    public async Task<SiteCorp.Shared.HumanResourcesCatalogsResponse> GetCatalogsAsync(CancellationToken cancellationToken = default)
    {
        return new SiteCorp.Shared.HumanResourcesCatalogsResponse(
            (await repository.GetGendersAsync(cancellationToken)).Select(MapCatalogItem).ToList(),
            (await repository.GetSkinColorsAsync(cancellationToken)).Select(MapCatalogItem).ToList(),
            (await repository.GetPoliticalAffiliationsAsync(cancellationToken)).Select(MapCatalogItem).ToList(),
            (await repository.GetMaritalStatusesAsync(cancellationToken)).Select(MapCatalogItem).ToList(),
            (await repository.GetEducationLevelsAsync(cancellationToken)).Select(MapEducationLevel).ToList(),
            (await repository.GetEmploymentTypesAsync(cancellationToken)).Select(MapCatalogItem).ToList(),
            (await repository.GetDrivingLicenseCategoriesAsync(cancellationToken)).Select(MapCatalogItem).ToList(),
            (await repository.GetRetireeRehireStatusesAsync(cancellationToken)).Select(MapCatalogItem).ToList());
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.PersonResponse>> GetPeopleAsync(CancellationToken cancellationToken = default)
    {
        var people = await repository.GetPeopleAsync(cancellationToken);
        return people.Select(MapPerson).ToList();
    }

    public async Task<SiteCorp.Shared.PersonResponse> CreatePersonAsync(
        SiteCorp.Shared.CreatePersonRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await repository.GetPersonByNationalIdAsync(request.NationalId, cancellationToken) is not null)
        {
            throw new DomainException("Ya existe un candidato con ese identificador nacional.");
        }

        await EnsurePersonCatalogsAsync(request, cancellationToken);
        EnsureRequiredPersonDocuments(request);
        var educationLevel = (await repository.GetEducationLevelsAsync(cancellationToken))
            .First(level => level.Id == request.EducationLevelId);
        var specialty = NormalizeSpecialty(request.Specialty, RequiresSpecialty(educationLevel.Code));

        var person = new Person(
            new FullName(request.FirstName, request.FirstLastName, request.SecondLastName),
            new NationalId(request.NationalId),
            request.BirthDate,
            MapAddress(request.Address),
            request.NumberOfChildren,
            request.DefenseSituation,
            request.PreEmploymentCheck,
            request.CompletedDegree,
            request.HasCriminalRecord,
            request.HasEmploymentContract,
            request.HasDisciplinaryMeasures,
            request.EducationLevelId,
            specialty,
            request.MaritalStatusId,
            request.GenderId,
            request.SkinColorId,
            request.PoliticalAffiliationId,
            request.EmploymentTypeId,
            request.HasDrivingLicense ? DistinctIds(request.DrivingLicenseCategoryIds) : [],
            request.RetireeRehireStatusId,
            MapPhysicalData(request.PhysicalData));

        AttachPersonDocuments(person, request);

        await repository.AddPersonAsync(person, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapPerson(person);
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.StaffingPositionResponse>> GetStaffingPositionsAsync(CancellationToken cancellationToken = default)
    {
        var positions = await repository.GetStaffingPositionsAsync(cancellationToken);
        return positions.Select(MapStaffingPosition).ToList();
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.StaffingAreaResponse>> GetStaffingAreasAsync(CancellationToken cancellationToken = default)
    {
        var areas = await repository.GetStaffingAreasAsync(cancellationToken);
        return areas.Select(MapStaffingArea).ToList();
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.JobTemplateResponse>> GetJobTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templates = await repository.GetJobTemplatesAsync(cancellationToken);
        return templates.Select(MapJobTemplate).ToList();
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.JobTemplatePositionResponse>> GetJobTemplatePositionsAsync(CancellationToken cancellationToken = default)
    {
        var templatePositions = await repository.GetJobTemplatePositionsAsync(cancellationToken);
        var positions = await repository.GetStaffingPositionsAsync(cancellationToken);
        var areas = await repository.GetStaffingAreasAsync(cancellationToken);
        var positionsById = positions.ToDictionary(position => position.Id);
        var areasById = areas.ToDictionary(area => area.Id);

        return templatePositions
            .OrderBy(templatePosition => areasById.GetValueOrDefault(templatePosition.AreaId)?.Priority ?? int.MaxValue)
            .ThenBy(templatePosition => areasById.GetValueOrDefault(templatePosition.AreaId)?.Name)
            .ThenBy(templatePosition => positionsById.GetValueOrDefault(templatePosition.PositionId)?.Name)
            .Select(templatePosition => MapJobTemplatePosition(
                templatePosition,
                areasById.GetValueOrDefault(templatePosition.AreaId),
                positionsById.GetValueOrDefault(templatePosition.PositionId)))
            .ToList();
    }

    public async Task<IReadOnlyList<SiteCorp.Shared.EmploymentResponse>> GetEmploymentsAsync(CancellationToken cancellationToken = default)
    {
        var histories = await repository.GetEmploymentHistoriesAsync(cancellationToken);
        var people = await repository.GetPeopleAsync(cancellationToken);
        var entities = await repository.GetOrganizationEntitiesAsync(cancellationToken);
        var positions = await repository.GetStaffingPositionsAsync(cancellationToken);

        var peopleById = people.ToDictionary(person => person.Id);
        var entitiesById = entities.ToDictionary(entity => entity.Id);
        var positionsById = positions.ToDictionary(position => position.Id);

        return histories
            .Select(history => MapEmployment(
                history,
                peopleById.GetValueOrDefault(history.PersonId),
                entitiesById.GetValueOrDefault(history.EntityId),
                positionsById.GetValueOrDefault(history.PositionId)))
            .ToList();
    }

    public async Task<SiteCorp.Shared.StaffingAreaResponse> CreateStaffingAreaAsync(
        SiteCorp.Shared.CreateStaffingAreaRequest request,
        CancellationToken cancellationToken = default)
    {
        var area = new StaffingArea(request.Name, request.Priority);
        var existingAreas = await repository.GetStaffingAreasAsync(cancellationToken);

        if (existingAreas.Any(existing => string.Equals(existing.Name, area.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException("Ya existe un area con ese nombre.");
        }

        await repository.AddStaffingAreaAsync(area, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapStaffingArea(area);
    }

    public async Task<SiteCorp.Shared.StaffingPositionResponse> CreateStaffingPositionAsync(
        SiteCorp.Shared.CreateStaffingPositionRequest request,
        CancellationToken cancellationToken = default)
    {
        var position = new DomainPosition(request.Code, request.Name, request.Description, request.Category);
        await repository.AddStaffingPositionAsync(position, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapStaffingPosition(position);
    }

    public async Task<SiteCorp.Shared.JobTemplateResponse> CreateJobTemplateAsync(
        SiteCorp.Shared.CreateJobTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = await repository.GetOrganizationEntityByIdAsync(request.EntityId, cancellationToken)
            ?? throw new DomainException("No existe la entidad indicada para la plantilla.");

        _ = await repository.CatalogItemExistsAsync<EducationLevel>(request.EducationLevelId, cancellationToken)
            ? true
            : throw new DomainException("No existe el nivel educacional indicado.");

        if (await repository.GetActiveJobTemplateByEntityIdAsync(request.EntityId, cancellationToken) is not null)
        {
            throw new DomainException("La entidad ya tiene una plantilla activa.");
        }

        var template = new JobTemplate(request.EntityId, request.EducationLevelId, request.EffectiveDate, request.ApprovedBy);
        await repository.AddJobTemplateAsync(template, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapJobTemplate(template);
    }

    public async Task<SiteCorp.Shared.JobTemplatePositionResponse> AddJobTemplatePositionAsync(
        Guid jobTemplateId,
        SiteCorp.Shared.AddJobTemplatePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = await repository.GetJobTemplateByIdAsync(jobTemplateId, cancellationToken)
            ?? throw new DomainException("No existe la plantilla indicada.");

        var area = await repository.GetStaffingAreaByIdAsync(request.AreaId, cancellationToken)
            ?? throw new DomainException("No existe el area indicada.");

        var position = await repository.GetStaffingPositionByIdAsync(request.PositionId, cancellationToken)
            ?? throw new DomainException("No existe el cargo indicado.");

        if (await repository.GetJobTemplatePositionAsync(jobTemplateId, request.PositionId, cancellationToken) is not null)
        {
            throw new DomainException("La plantilla ya contiene ese cargo.");
        }

        var templatePosition = new JobTemplatePosition(
            jobTemplateId,
            request.AreaId,
            request.PositionId,
            new VacancyInfo(request.TotalVacancies, request.FilledVacancies, request.BaseSalary, request.SalaryCategory));

        await repository.AddJobTemplatePositionAsync(templatePosition, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapJobTemplatePosition(templatePosition, area, position);
    }

    public async Task<SiteCorp.Shared.EmploymentResponse> HirePersonAsync(
        SiteCorp.Shared.HirePersonRequest request,
        CancellationToken cancellationToken = default)
    {
        var person = await repository.GetPersonByIdAsync(request.PersonId, cancellationToken)
            ?? throw new DomainException("No existe el candidato indicado.");

        var entity = await repository.GetOrganizationEntityByIdAsync(request.EntityId, cancellationToken)
            ?? throw new DomainException("No existe la entidad indicada.");

        var position = await repository.GetStaffingPositionByIdAsync(request.PositionId, cancellationToken)
            ?? throw new DomainException("No existe el cargo indicado.");

        var activeTemplate = await repository.GetActiveJobTemplateByEntityIdAsync(request.EntityId, cancellationToken)
            ?? throw new DomainException("La entidad no tiene plantilla activa.");

        var templatePosition = await repository.GetJobTemplatePositionAsync(activeTemplate.Id, request.PositionId, cancellationToken)
            ?? throw new DomainException("El cargo no pertenece a la plantilla activa de la entidad.");

        if (await repository.HasActiveEmploymentAsync(request.PersonId, request.EntityId, request.PositionId, cancellationToken))
        {
            throw new DomainException("El candidato ya tiene una relacion laboral activa con esa entidad y ese cargo.");
        }

        templatePosition.OccupyVacancy();

        var employment = new EmploymentHistory(
            request.PersonId,
            request.EntityId,
            request.PositionId,
            templatePosition.Id,
            request.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            request.Notes);

        await repository.AddEmploymentHistoryAsync(employment, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapEmployment(employment, person, entity, position);
    }

    private async Task<Dictionary<Guid, int>> BuildOpenPositionsByEntityAsync(
        IReadOnlyList<JobTemplatePosition> templatePositions,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<Guid, int>();

        foreach (var templatePosition in templatePositions.Where(position => position.HasAvailability()))
        {
            var template = await repository.GetJobTemplateByIdAsync(templatePosition.JobTemplateId, cancellationToken);

            if (template is null)
            {
                continue;
            }

            result[template.EntityId] = result.GetValueOrDefault(template.EntityId) + 1;
        }

        return result;
    }

    private async Task<Dictionary<Guid, JobTemplate>> BuildTemplatesByIdAsync(
        IReadOnlyList<JobTemplatePosition> templatePositions,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<Guid, JobTemplate>();

        foreach (var templateId in templatePositions.Select(position => position.JobTemplateId).Distinct())
        {
            var template = await repository.GetJobTemplateByIdAsync(templateId, cancellationToken);

            if (template is not null)
            {
                result[templateId] = template;
            }
        }

        return result;
    }

    private async Task EnsurePersonCatalogsAsync(SiteCorp.Shared.CreatePersonRequest request, CancellationToken cancellationToken)
    {
        await EnsureCatalogAsync<EducationLevel>(request.EducationLevelId, "No existe el nivel educacional indicado.", cancellationToken);
        await EnsureCatalogAsync<MaritalStatus>(request.MaritalStatusId, "No existe el estado civil indicado.", cancellationToken);
        await EnsureCatalogAsync<Gender>(request.GenderId, "No existe el genero indicado.", cancellationToken);
        await EnsureCatalogAsync<SkinColor>(request.SkinColorId, "No existe el color de piel indicado.", cancellationToken);
        await EnsureCatalogAsync<PoliticalAffiliation>(request.PoliticalAffiliationId, "No existe la afiliacion politica indicada.", cancellationToken);
        await EnsureCatalogAsync<EmploymentType>(request.EmploymentTypeId, "No existe el tipo de empleo indicado.", cancellationToken);
        await EnsureCatalogAsync<RetireeRehireStatus>(request.RetireeRehireStatusId, "No existe la condicion de jubilado/recontratado indicada.", cancellationToken);

        var drivingLicenseCategoryIds = DistinctIds(request.DrivingLicenseCategoryIds);
        if (request.HasDrivingLicense && drivingLicenseCategoryIds.Count == 0)
        {
            throw new DomainException("Debe seleccionar al menos una categoria de licencia de conduccion.");
        }

        foreach (var drivingLicenseCategoryId in drivingLicenseCategoryIds)
        {
            await EnsureCatalogAsync<DrivingLicenseCategory>(
                drivingLicenseCategoryId,
                "No existe la categoria de licencia indicada.",
                cancellationToken);
        }
    }

    private static void EnsureRequiredPersonDocuments(SiteCorp.Shared.CreatePersonRequest request)
    {
        EnsureRequiredDocument(
            request.PreEmploymentCheck,
            request.PreEmploymentCheckDocument,
            "Debe adjuntar el documento del chequeo pre-empleo.");

        EnsureRequiredDocument(
            request.HasEmploymentContract,
            request.EmploymentContractDocument,
            "Debe adjuntar el documento del contrato laboral.");

        EnsureRequiredDocument(
            request.HasCriminalRecord,
            request.CriminalRecordDocument,
            "Debe adjuntar el documento de antecedentes penales.");

        EnsureRequiredDocument(
            request.HasDisciplinaryMeasures,
            request.DisciplinaryMeasuresDocument,
            "Debe adjuntar el documento de medidas disciplinarias.");
    }

    private static void EnsureRequiredDocument(
        bool isRequired,
        SiteCorp.Shared.DocumentAttachmentDto? document,
        string message)
    {
        if (!isRequired)
        {
            return;
        }

        if (document is null ||
            string.IsNullOrWhiteSpace(document.FileName) ||
            string.IsNullOrWhiteSpace(document.ContentBase64))
        {
            throw new DomainException(message);
        }
    }

    private static void AttachPersonDocuments(Person person, SiteCorp.Shared.CreatePersonRequest request)
    {
        AttachPersonDocument(
            person,
            request.PreEmploymentCheck,
            request.PreEmploymentCheckDocument,
            PersonDocumentTypes.PreEmploymentCheck);

        AttachPersonDocument(
            person,
            request.HasEmploymentContract,
            request.EmploymentContractDocument,
            PersonDocumentTypes.EmploymentContract);

        AttachPersonDocument(
            person,
            request.HasCriminalRecord,
            request.CriminalRecordDocument,
            PersonDocumentTypes.CriminalRecord);

        AttachPersonDocument(
            person,
            request.HasDisciplinaryMeasures,
            request.DisciplinaryMeasuresDocument,
            PersonDocumentTypes.DisciplinaryMeasures);
    }

    private static void AttachPersonDocument(
        Person person,
        bool shouldAttach,
        SiteCorp.Shared.DocumentAttachmentDto? document,
        string documentType)
    {
        if (!shouldAttach || document is null)
        {
            return;
        }

        person.AttachDocument(
            documentType,
            document.FileName,
            document.ContentType,
            document.ContentBase64);
    }

    private async Task EnsureCatalogAsync<TCatalog>(Guid id, string message, CancellationToken cancellationToken)
        where TCatalog : CatalogItem
    {
        if (!await repository.CatalogItemExistsAsync<TCatalog>(id, cancellationToken))
        {
            throw new DomainException(message);
        }
    }

    private static string? NormalizeSpecialty(string? specialty, bool isRequired)
    {
        if (string.IsNullOrWhiteSpace(specialty))
        {
            if (isRequired)
            {
                throw new DomainException("La especialidad es obligatoria para el nivel educacional seleccionado.");
            }

            return null;
        }

        return isRequired ? specialty.Trim() : null;
    }

    private static bool RequiresSpecialty(string code)
    {
        var normalized = code
            .Trim()
            .ToUpperInvariant()
            .Replace("É", "E")
            .Replace("  ", " ");

        return normalized is "TECNICO MEDIO" or "SUPERIOR";
    }

    private static IReadOnlyList<Guid> DistinctIds(IEnumerable<Guid>? ids)
    {
        return ids?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList() ?? [];
    }

    private static Address MapAddress(SiteCorp.Shared.AddressDto address)
    {
        return new Address(address.Street, address.Number, address.City, address.Province, address.PostalCode);
    }

    private static PhysicalData MapPhysicalData(SiteCorp.Shared.PhysicalDataDto physicalData)
    {
        return new PhysicalData(
            physicalData.Height,
            physicalData.Weight,
            physicalData.PantsSize,
            physicalData.ShirtSize,
            physicalData.ShoeSize);
    }

    private static SiteCorp.Shared.OrganizationEntityResponse MapOrganizationEntity(OrganizationEntity entity)
    {
        return new SiteCorp.Shared.OrganizationEntityResponse(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.EntityType switch
            {
                OrganizationEntityType.BusinessGroup => SiteCorp.Shared.OrganizationEntityType.BusinessGroup,
                OrganizationEntityType.Company => SiteCorp.Shared.OrganizationEntityType.Company,
                OrganizationEntityType.BusinessUnit => SiteCorp.Shared.OrganizationEntityType.BusinessUnit,
                _ => throw new ArgumentOutOfRangeException(nameof(entity))
            },
            entity.IsActive,
            entity is Company company ? company.BusinessGroupId : null,
            entity is BusinessUnit businessUnit ? businessUnit.CompanyId : null);
    }

    private static SiteCorp.Shared.CatalogItemResponse MapCatalogItem(CatalogItem catalogItem)
    {
        return new SiteCorp.Shared.CatalogItemResponse(
            catalogItem.Id,
            catalogItem.Code,
            catalogItem.Description,
            catalogItem.IsActive);
    }

    private static SiteCorp.Shared.CatalogItemResponse MapEducationLevel(EducationLevel educationLevel)
    {
        return new SiteCorp.Shared.CatalogItemResponse(
            educationLevel.Id,
            educationLevel.Code,
            educationLevel.Description,
            educationLevel.IsActive,
            educationLevel.HierarchyLevel);
    }

    private static SiteCorp.Shared.PersonResponse MapPerson(Person person)
    {
        return new SiteCorp.Shared.PersonResponse(
            person.Id,
            person.FullName.Value,
            person.FullName.FirstName,
            person.FullName.FirstLastName,
            person.FullName.SecondLastName,
            person.Specialty,
            person.NationalId.Number,
            person.BirthDate,
            person.Address.FormatAddress(),
            person.DrivingLicenseCategories.Count > 0,
            person.DrivingLicenseCategories
                .Select(category => category.DrivingLicenseCategoryId)
                .ToList(),
            person.NumberOfChildren);
    }

    private static SiteCorp.Shared.StaffingPositionResponse MapStaffingPosition(DomainPosition position)
    {
        return new SiteCorp.Shared.StaffingPositionResponse(
            position.Id,
            position.Code,
            position.Name,
            position.Description,
            position.Category,
            position.IsActive);
    }

    private static SiteCorp.Shared.StaffingAreaResponse MapStaffingArea(StaffingArea area)
    {
        return new SiteCorp.Shared.StaffingAreaResponse(
            area.Id,
            area.Name,
            area.Priority,
            area.IsActive);
    }

    private static SiteCorp.Shared.JobTemplateResponse MapJobTemplate(JobTemplate template)
    {
        return new SiteCorp.Shared.JobTemplateResponse(
            template.Id,
            template.EntityId,
            template.EducationLevelId,
            template.EffectiveDate,
            template.ApprovedBy,
            template.IsActive);
    }

    private static SiteCorp.Shared.JobTemplatePositionResponse MapJobTemplatePosition(
        JobTemplatePosition templatePosition,
        StaffingArea? area,
        DomainPosition? position)
    {
        return new SiteCorp.Shared.JobTemplatePositionResponse(
            templatePosition.Id,
            templatePosition.JobTemplateId,
            templatePosition.AreaId,
            area?.Name ?? "Area no encontrada",
            area?.Priority ?? int.MaxValue,
            templatePosition.PositionId,
            position?.Name ?? "Cargo no encontrado",
            templatePosition.Vacancy.TotalVacancies,
            templatePosition.Vacancy.FilledVacancies,
            templatePosition.Vacancy.BaseSalary,
            templatePosition.Vacancy.SalaryCategory,
            templatePosition.HasAvailability());
    }

    private static SiteCorp.Shared.EmploymentResponse MapEmployment(
        EmploymentHistory history,
        Person? person,
        OrganizationEntity? entity,
        DomainPosition? position)
    {
        return new SiteCorp.Shared.EmploymentResponse(
            history.Id,
            history.PersonId,
            person?.FullName.Value ?? "Candidato no encontrado",
            history.EntityId,
            entity?.Name ?? "Entidad no encontrada",
            history.PositionId,
            position?.Name ?? "Cargo no encontrado",
            history.JobTemplatePositionId,
            history.Period.StartDate,
            history.Period.EndDate,
            history.IsActive);
    }
}
