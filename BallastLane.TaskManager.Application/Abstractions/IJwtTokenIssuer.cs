using BallastLane.TaskManager.Common;
using BallastLane.TaskManager.Users;

namespace BallastLane.TaskManager.Abstractions;

/// <summary>
/// Issues signed JWT bearer tokens for authenticated users. Implementations live in the infrastructure layer
/// and own the signing key, audience, and expiry policy.
/// </summary>
public interface IJwtTokenIssuer
{
    /// <summary>
    /// Produces a freshly signed JWT for the supplied user.
    /// </summary>
    /// <param name="user">Authenticated user the token will represent.</param>
    /// <returns>The token string together with its absolute expiration timestamp.</returns>
    IssuedToken Issue(User user);
}
