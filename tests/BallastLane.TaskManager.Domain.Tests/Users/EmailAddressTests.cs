using BallastLane.TaskManager.Exceptions;
using BallastLane.TaskManager.Users;

namespace BallastLane.TaskManager.Domain.Tests.Users;

public class EmailAddressTests
{
    [Fact]
    public void From_LowercasesAndTrimsInput()
    {
        var email = EmailAddress.From("  Foo@BAR.com ");
        email.Value.ShouldBe("foo@bar.com");
    }

    [Fact]
    public void From_RejectsEmpty()
    {
        var ex = Should.Throw<DomainValidationException>(() => EmailAddress.From(""));
        ex.Errors.ShouldContain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void From_RejectsWhitespace()
    {
        Should.Throw<DomainValidationException>(() => EmailAddress.From("   "));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("@bar.com")]
    [InlineData("foo@")]
    [InlineData("foo@bar")]
    [InlineData("foo @bar.com")]
    public void From_RejectsInvalidFormats(string raw)
    {
        Should.Throw<DomainValidationException>(() => EmailAddress.From(raw));
    }
}
