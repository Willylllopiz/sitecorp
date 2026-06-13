using SiteCorp.Domain.Common;
using SiteCorp.Domain.HumanResources;
using Xunit;

namespace SiteCorp.Domain.Tests.HumanResources;

public sealed class LeaveRequestTests
{
    [Fact]
    public void Create_WhenEndDateIsBeforeStartDate_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            LeaveRequest.Create(
                employeeId: 1,
                startDate: new DateOnly(2026, 6, 20),
                endDate: new DateOnly(2026, 6, 19),
                reason: "Vacaciones"));

        Assert.Contains("fecha final", exception.Message);
    }

    [Fact]
    public void Create_WhenDataIsValid_StartsPending()
    {
        var request = LeaveRequest.Create(
            employeeId: 1,
            startDate: new DateOnly(2026, 6, 20),
            endDate: new DateOnly(2026, 6, 22),
            reason: "Vacaciones");

        Assert.Equal(LeaveRequestStatus.Pending, request.Status);
    }

    [Fact]
    public void Approve_WhenRequestIsPending_ChangesStatusToApproved()
    {
        var request = LeaveRequest.Create(
            employeeId: 1,
            startDate: new DateOnly(2026, 6, 20),
            endDate: new DateOnly(2026, 6, 22),
            reason: "Vacaciones");

        request.Approve();

        Assert.Equal(LeaveRequestStatus.Approved, request.Status);
    }

    [Fact]
    public void Reject_WhenRequestWasAlreadyApproved_ThrowsDomainException()
    {
        var request = LeaveRequest.Create(
            employeeId: 1,
            startDate: new DateOnly(2026, 6, 20),
            endDate: new DateOnly(2026, 6, 22),
            reason: "Vacaciones");

        request.Approve();

        Assert.Throws<DomainException>(request.Reject);
    }
}
