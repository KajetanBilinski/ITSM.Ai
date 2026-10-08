using ItsmAi.Domain.Entities;
using ItsmAi.Domain.Exceptions;

namespace ItsmAi.Tests.Domain;

public class RequesterTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateRequester()
    {
        var requester = new Requester(
            "Jan Kowalski",
            "JAN.KOWALSKI@COMPANY.COM");

        Assert.NotEqual(Guid.Empty, requester.Id);
        Assert.Equal("Jan Kowalski", requester.Name);
        Assert.Equal(
            "jan.kowalski@company.com",
            requester.Email);
    }

    [Fact]
    public void Create_WithEmptyEmail_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(
            () => new Requester(
                "Jan Kowalski",
                " "));
    }
}