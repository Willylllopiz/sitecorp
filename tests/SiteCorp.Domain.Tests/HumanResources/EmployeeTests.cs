using SiteCorp.Domain.Common;
using SiteCorp.Domain.HumanResources;
using Xunit;

namespace SiteCorp.Domain.Tests.HumanResources;

public sealed class EmployeeTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ChangeEngagementScore_WhenValueIsOutsideRange_ThrowsDomainException(int score)
    {
        var employee = CreateEmployee();

        Assert.Throws<DomainException>(() => employee.ChangeEngagementScore(score));
    }

    [Fact]
    public void StartLeave_ChangesStatusToOnLeave()
    {
        var employee = CreateEmployee();

        employee.StartLeave();

        Assert.Equal(EmploymentStatus.OnLeave, employee.Status);
    }

    private static Employee CreateEmployee()
    {
        return new Employee(
            employeeNumber: "SC-2001",
            fullName: "Ana Ramirez",
            departmentId: 1,
            position: "HR Analyst",
            location: "Habana",
            hireDate: new DateOnly(2025, 4, 10),
            status: EmploymentStatus.Active,
            engagementScore: 85);
    }
}
