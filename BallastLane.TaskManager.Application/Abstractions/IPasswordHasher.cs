namespace BallastLane.TaskManager.Abstractions;

/// <summary>
/// Hashes plaintext passwords for storage and verifies submitted passwords against stored hashes.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Produces a salted hash of the supplied plaintext password suitable for persistent storage.
    /// </summary>
    /// <param name="password">Caller-supplied plaintext password.</param>
    /// <returns>The encoded hash string, including any salt and algorithm metadata required to verify it later.</returns>
    string Hash(string password);

    /// <summary>
    /// Checks whether the supplied plaintext password matches the previously stored hash.
    /// </summary>
    /// <param name="hash">Stored hash produced by <see cref="Hash"/>.</param>
    /// <param name="password">Plaintext password submitted by the caller.</param>
    /// <returns><c>true</c> if the password matches; otherwise <c>false</c>.</returns>
    bool Verify(string hash, string password);
}
