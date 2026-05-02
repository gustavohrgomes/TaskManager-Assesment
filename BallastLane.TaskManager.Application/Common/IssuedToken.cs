namespace BallastLane.TaskManager.Common;

/// <summary>
/// Result of a successful token issuance: the encoded JWT and the absolute UTC instant at which it expires.
/// </summary>
/// <param name="Token">Encoded JWT string to return to the caller.</param>
/// <param name="ExpiresAt">Absolute UTC expiration timestamp of <paramref name="Token"/>.</param>
public sealed record IssuedToken(string Token, DateTimeOffset ExpiresAt);
