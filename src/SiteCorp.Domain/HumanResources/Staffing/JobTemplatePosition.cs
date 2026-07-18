using SiteCorp.Domain.Common;
using SiteCorp.Domain.HumanResources.ValueObjects;

namespace SiteCorp.Domain.HumanResources.Staffing;

public sealed class JobTemplatePosition
{
    private JobTemplatePosition()
    {
        Vacancy = new VacancyInfo(1, 0, 0, "GENERAL");
        RowVersion = [];
    }

    public JobTemplatePosition(Guid jobTemplateId, Guid areaId, Guid positionId, VacancyInfo vacancy)
    {
        if (jobTemplateId == Guid.Empty)
        {
            throw new DomainException("La posicion debe pertenecer a una plantilla.");
        }

        if (areaId == Guid.Empty)
        {
            throw new DomainException("La posicion de plantilla debe pertenecer a un area.");
        }

        if (positionId == Guid.Empty)
        {
            throw new DomainException("La posicion de plantilla debe referenciar un cargo.");
        }

        Id = Guid.NewGuid();
        JobTemplateId = jobTemplateId;
        AreaId = areaId;
        PositionId = positionId;
        Vacancy = vacancy;
        RowVersion = [];
    }

    public Guid Id { get; private set; }

    public Guid JobTemplateId { get; private set; }

    public Guid AreaId { get; private set; }

    public Guid PositionId { get; private set; }

    public VacancyInfo Vacancy { get; private set; }

    public byte[] RowVersion { get; private set; }

    public bool HasAvailability() => Vacancy.HasAvailability();

    public void OccupyVacancy()
    {
        Vacancy = Vacancy.Occupy();
    }

    public void ReleaseVacancy()
    {
        Vacancy = Vacancy.Release();
    }
}
