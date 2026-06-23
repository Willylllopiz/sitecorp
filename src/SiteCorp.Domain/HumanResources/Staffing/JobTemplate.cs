using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.Staffing;

public sealed class JobTemplate
{
    private JobTemplate()
    {
        ApprovedBy = string.Empty;
    }

    public JobTemplate(Guid entityId, Guid educationLevelId, DateOnly effectiveDate, string approvedBy)
    {
        if (entityId == Guid.Empty)
        {
            throw new DomainException("La plantilla debe pertenecer a una entidad organizacional.");
        }

        if (educationLevelId == Guid.Empty)
        {
            throw new DomainException("La plantilla debe definir un nivel educacional requerido.");
        }

        Id = Guid.NewGuid();
        EntityId = entityId;
        EducationLevelId = educationLevelId;
        EffectiveDate = effectiveDate;
        ApprovedBy = string.IsNullOrWhiteSpace(approvedBy)
            ? throw new DomainException("La autoridad aprobadora es obligatoria.")
            : approvedBy.Trim();
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid EntityId { get; private set; }

    public Guid EducationLevelId { get; private set; }

    public DateOnly EffectiveDate { get; private set; }

    public string ApprovedBy { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
