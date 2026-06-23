using SiteCorp.Domain.Common;
using SiteCorp.Domain.HumanResources.People;
using SiteCorp.Domain.HumanResources.Staffing;
using SiteCorp.Domain.HumanResources.ValueObjects;
using Xunit;

namespace SiteCorp.Domain.Tests.HumanResources;

public sealed class HiringDomainTests
{
    [Fact]
    public void OccupyVacancy_IncrementsFilledVacancies()
    {
        var templatePosition = new JobTemplatePosition(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new VacancyInfo(totalVacancies: 2, filledVacancies: 0, baseSalary: 45000, salaryCategory: "CAT-1"));

        templatePosition.OccupyVacancy();

        Assert.Equal(1, templatePosition.Vacancy.FilledVacancies);
        Assert.True(templatePosition.HasAvailability());
    }

    [Fact]
    public void OccupyVacancy_ThrowsWhenNoVacancyIsAvailable()
    {
        var templatePosition = new JobTemplatePosition(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new VacancyInfo(totalVacancies: 1, filledVacancies: 1, baseSalary: 45000, salaryCategory: "CAT-1"));

        Assert.Throws<DomainException>(templatePosition.OccupyVacancy);
    }

    [Fact]
    public void EmploymentHistory_End_ClosesActivePeriod()
    {
        var history = new EmploymentHistory(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 1, 1));

        history.End(new DateOnly(2026, 6, 1), "Fin de contrato");

        Assert.False(history.IsActive);
        Assert.Equal(new DateOnly(2026, 6, 1), history.Period.EndDate);
    }
}
