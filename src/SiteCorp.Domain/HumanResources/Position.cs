using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources;

public sealed class Position
{
    private Position()
    {
        Title = string.Empty;
        Location = string.Empty;
    }

    public Position(
        string title,
        int departmentId,
        string location,
        PositionStatus status,
        int candidates,
        DateOnly targetStartDate)
    {
        if (departmentId <= 0)
        {
            throw new DomainException("La vacante debe pertenecer a un departamento valido.");
        }

        Title = RequireText(title, "El titulo de la vacante es obligatorio.");
        DepartmentId = departmentId;
        Location = RequireText(location, "La ubicacion de la vacante es obligatoria.");
        Status = status;
        ChangeCandidateCount(candidates);
        TargetStartDate = targetStartDate;
    }

    public int Id { get; private set; }

    public string Title { get; private set; }

    public int DepartmentId { get; private set; }

    public string Location { get; private set; }

    public PositionStatus Status { get; private set; }

    public int Candidates { get; private set; }

    public DateOnly TargetStartDate { get; private set; }

    public bool CountsAsOpen => Status is PositionStatus.Open or PositionStatus.Interviewing or PositionStatus.Offer;

    public void Pause() => Status = PositionStatus.OnHold;

    public void StartInterviews() => Status = PositionStatus.Interviewing;

    public void MoveToOffer() => Status = PositionStatus.Offer;

    public void ChangeCandidateCount(int candidates)
    {
        if (candidates < 0)
        {
            throw new DomainException("La cantidad de candidatos no puede ser negativa.");
        }

        Candidates = candidates;
    }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }
}

