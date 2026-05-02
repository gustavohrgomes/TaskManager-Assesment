namespace BallastLane.TaskManager.Auth;

/// <summary>
/// Input for the user registration flow: the email and chosen plaintext password.
/// </summary>
/// <param name="Email">Email address the new user will log in with.</param>
/// <param name="Password">Plaintext password chosen by the user; will be hashed before storage.</param>
public sealed record RegisterUserCommand(string Email, string Password);
