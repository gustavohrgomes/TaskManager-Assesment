namespace BallastLane.TaskManager.Infrastructure.Tests.Auth;

public sealed class Pbkdf2PasswordHasherTests
{
    private readonly BallastLane.TaskManager.Auth.Pbkdf2PasswordHasher _sut = new();

    [Fact]
    public void Hash_ReturnsNonEmptyString()
    {
        var hash = _sut.Hash("P@ssw0rd!");
        hash.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Hash_ProducesDifferentHashesForSamePassword()
    {
        var hash1 = _sut.Hash("P@ssw0rd!");
        var hash2 = _sut.Hash("P@ssw0rd!");
        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        var hash = _sut.Hash("Demo123!");
        _sut.Verify(hash, "Demo123!").ShouldBeTrue();
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hash = _sut.Hash("Demo123!");
        _sut.Verify(hash, "WrongPassword").ShouldBeFalse();
    }

    [Fact]
    public void Verify_EmptyPassword_ReturnsFalse()
    {
        var hash = _sut.Hash("Demo123!");
        _sut.Verify(hash, "").ShouldBeFalse();
    }
}
