using Microsoft.AspNetCore.Identity;
using BallastLane.TaskManager.Abstractions;

namespace BallastLane.TaskManager.Auth;

internal sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _inner = new();

    public string Hash(string password) =>
        _inner.HashPassword(null!, password);

    public bool Verify(string hash, string password) =>
        _inner.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
}
