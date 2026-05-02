using BallastLane.TaskManager.Exceptions;
using BallastLane.TaskManager.Users;

namespace BallastLane.TaskManager.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Register_AssignsNewIdAndStoresEmailLowercase()
    {
        var email = EmailAddress.From("ALICE@example.com");
        var now = DateTimeOffset.UtcNow;

        var user = User.Register(email, passwordHash: "hash", now: now);

        user.Id.ShouldNotBe(Guid.Empty);
        user.Email.Value.ShouldBe("alice@example.com");
        user.PasswordHash.ShouldBe("hash");
        user.CreatedAt.ShouldBe(now);
    }

    [Fact]
    public void Register_RejectsEmptyPasswordHash()
    {
        var email = EmailAddress.From("a@b.com");
        Should.Throw<DomainValidationException>(() =>
            User.Register(email, passwordHash: "", now: DateTimeOffset.UtcNow));
    }
}
