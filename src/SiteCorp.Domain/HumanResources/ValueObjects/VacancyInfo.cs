using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.ValueObjects;

public sealed class VacancyInfo
{
    private VacancyInfo()
    {
        SalaryCategory = string.Empty;
    }

    public VacancyInfo(int totalVacancies, int filledVacancies, decimal baseSalary, string salaryCategory)
    {
        if (totalVacancies <= 0)
        {
            throw new DomainException("La cantidad total de plazas debe ser mayor que cero.");
        }

        if (filledVacancies < 0 || filledVacancies > totalVacancies)
        {
            throw new DomainException("La cantidad de plazas ocupadas no es valida.");
        }

        if (baseSalary < 0)
        {
            throw new DomainException("El salario base no puede ser negativo.");
        }

        TotalVacancies = totalVacancies;
        FilledVacancies = filledVacancies;
        BaseSalary = baseSalary;
        SalaryCategory = string.IsNullOrWhiteSpace(salaryCategory)
            ? throw new DomainException("La categoria salarial es obligatoria.")
            : salaryCategory.Trim();
    }

    public int TotalVacancies { get; private set; }

    public int FilledVacancies { get; private set; }

    public decimal BaseSalary { get; private set; }

    public string SalaryCategory { get; private set; }

    public bool HasAvailability() => FilledVacancies < TotalVacancies;

    public decimal OccupationPercentage() => TotalVacancies == 0
        ? 0
        : decimal.Round((decimal)FilledVacancies / TotalVacancies * 100, 2);

    public VacancyInfo Occupy()
    {
        if (!HasAvailability())
        {
            throw new DomainException("No hay plazas disponibles para esta posicion.");
        }

        return new VacancyInfo(TotalVacancies, FilledVacancies + 1, BaseSalary, SalaryCategory);
    }

    public VacancyInfo Release()
    {
        if (FilledVacancies == 0)
        {
            throw new DomainException("No hay plazas ocupadas para liberar.");
        }

        return new VacancyInfo(TotalVacancies, FilledVacancies - 1, BaseSalary, SalaryCategory);
    }
}
