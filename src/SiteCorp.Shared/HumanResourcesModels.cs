namespace SiteCorp.Shared;

public enum EmploymentStatus
{
    Active,
    Onboarding,
    OnLeave,
    Offboarding
}

public enum PositionStatus
{
    Open,
    Interviewing,
    Offer,
    OnHold
}

public enum LeaveRequestStatus
{
    Pending,
    Approved,
    Rejected
}

public sealed record DepartmentSummary(
    int Id,
    string Name,
    string ManagerName,
    int HeadCount,
    int OpenRoles);

public sealed record Employee(
    int Id,
    string EmployeeNumber,
    string FullName,
    string Department,
    string Position,
    string Location,
    DateOnly HireDate,
    EmploymentStatus Status,
    int EngagementScore);

public sealed record Position(
    int Id,
    string Title,
    string Department,
    string Location,
    PositionStatus Status,
    int Candidates,
    DateOnly TargetStartDate);

public sealed record LeaveRequest(
    int Id,
    int EmployeeId,
    string EmployeeName,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason,
    LeaveRequestStatus Status);

public sealed record LeaveRequestDraft(
    int EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason);

public sealed record DashboardMetrics(
    int TotalEmployees,
    int ActiveEmployees,
    int OpenPositions,
    int PendingLeaveRequests,
    int OnboardingThisMonth,
    double AverageEngagement,
    IReadOnlyList<DepartmentSummary> Departments);

public sealed record HumanResourcesSnapshot(
    DashboardMetrics Metrics,
    IReadOnlyList<Employee> Employees,
    IReadOnlyList<Position> Positions,
    IReadOnlyList<LeaveRequest> LeaveRequests,
    bool IsLive);

public enum OrganizationEntityType
{
    BusinessGroup,
    Company,
    BusinessUnit
}

public sealed record AddressDto(
    string Street,
    string? Number,
    string City,
    string Province,
    string? PostalCode);

public sealed record PhysicalDataDto(
    decimal Height,
    decimal Weight,
    string? PantsSize,
    string? ShirtSize,
    string? ShoeSize);

public sealed record CreateBusinessGroupRequest(
    string Name,
    string? Description);

public sealed record CreateCompanyRequest(
    Guid BusinessGroupId,
    string Name,
    string? Description,
    AddressDto Address);

public sealed record CreateBusinessUnitRequest(
    Guid CompanyId,
    string Name,
    string? Description,
    AddressDto Address);

public sealed record OrganizationEntityResponse(
    Guid Id,
    string Name,
    string? Description,
    OrganizationEntityType EntityType,
    bool IsActive,
    Guid? BusinessGroupId = null,
    Guid? CompanyId = null);

public sealed record CatalogItemResponse(
    Guid Id,
    string Code,
    string Description,
    bool IsActive,
    int? HierarchyLevel = null);

public sealed record HumanResourcesCatalogsResponse(
    IReadOnlyList<CatalogItemResponse> Genders,
    IReadOnlyList<CatalogItemResponse> SkinColors,
    IReadOnlyList<CatalogItemResponse> PoliticalAffiliations,
    IReadOnlyList<CatalogItemResponse> MaritalStatuses,
    IReadOnlyList<CatalogItemResponse> EducationLevels,
    IReadOnlyList<CatalogItemResponse> EmploymentTypes,
    IReadOnlyList<CatalogItemResponse> DrivingLicenseCategories,
    IReadOnlyList<CatalogItemResponse> RetireeRehireStatuses);

public sealed record DocumentAttachmentDto(
    string FileName,
    string ContentType,
    string ContentBase64);

public sealed record CreatePersonRequest(
    string FirstName,
    string FirstLastName,
    string SecondLastName,
    string NationalId,
    DateOnly BirthDate,
    AddressDto Address,
    int NumberOfChildren,
    string? DefenseSituation,
    bool PreEmploymentCheck,
    DocumentAttachmentDto? PreEmploymentCheckDocument,
    string? CompletedDegree,
    bool HasCriminalRecord,
    DocumentAttachmentDto? CriminalRecordDocument,
    bool HasEmploymentContract,
    DocumentAttachmentDto? EmploymentContractDocument,
    bool HasDisciplinaryMeasures,
    DocumentAttachmentDto? DisciplinaryMeasuresDocument,
    Guid EducationLevelId,
    string? Specialty,
    Guid MaritalStatusId,
    Guid GenderId,
    Guid SkinColorId,
    Guid PoliticalAffiliationId,
    Guid EmploymentTypeId,
    bool HasDrivingLicense,
    IReadOnlyList<Guid> DrivingLicenseCategoryIds,
    Guid RetireeRehireStatusId,
    PhysicalDataDto PhysicalData);

public sealed record PersonResponse(
    Guid Id,
    string FullName,
    string FirstName,
    string FirstLastName,
    string SecondLastName,
    string? Specialty,
    string NationalId,
    DateOnly BirthDate,
    string Address,
    bool HasDrivingLicense,
    IReadOnlyList<Guid> DrivingLicenseCategoryIds,
    int NumberOfChildren);

public sealed record CreateStaffingPositionRequest(
    string Code,
    string Name,
    string? Description,
    string Category);

public sealed record CreateStaffingAreaRequest(
    string Name,
    int Priority);

public sealed record StaffingAreaResponse(
    Guid Id,
    string Name,
    int Priority,
    bool IsActive);

public sealed record StaffingPositionResponse(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    string Category,
    bool IsActive);

public sealed record CreateJobTemplateRequest(
    Guid EntityId,
    Guid EducationLevelId,
    DateOnly EffectiveDate,
    string ApprovedBy);

public sealed record AddJobTemplatePositionRequest(
    Guid AreaId,
    Guid PositionId,
    int TotalVacancies,
    int FilledVacancies,
    decimal BaseSalary,
    string SalaryCategory);

public sealed record JobTemplateResponse(
    Guid Id,
    Guid EntityId,
    Guid EducationLevelId,
    DateOnly EffectiveDate,
    string ApprovedBy,
    bool IsActive);

public sealed record JobTemplatePositionResponse(
    Guid Id,
    Guid JobTemplateId,
    Guid AreaId,
    string AreaName,
    int AreaPriority,
    Guid PositionId,
    string PositionName,
    int TotalVacancies,
    int FilledVacancies,
    decimal BaseSalary,
    string SalaryCategory,
    bool HasAvailability);

public sealed record HirePersonRequest(
    Guid PersonId,
    Guid EntityId,
    Guid PositionId,
    DateOnly? StartDate,
    string? Notes);

public sealed record EmploymentResponse(
    Guid Id,
    Guid PersonId,
    string PersonName,
    Guid EntityId,
    string EntityName,
    Guid PositionId,
    string PositionName,
    Guid JobTemplatePositionId,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsActive);
