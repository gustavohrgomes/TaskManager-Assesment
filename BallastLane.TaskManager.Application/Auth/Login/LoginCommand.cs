namespace BallastLane.TaskManager.Auth.Login;

/// <summary>
/// Input for the login flow: an email/password pair to be verified against stored credentials.
/// </summary>
/// <param name="Email">Email address the caller is logging in with.</param>
/// <param name="Password">Plaintext password to verify against the stored hash.</param>
public sealed record LoginCommand(string Email, string Password);
