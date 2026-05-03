namespace BallastLane.TaskManager.Auth.Register;

/// <summary>
/// Output of a successful registration: the new user's identifier and normalized email.
/// </summary>
/// <param name="UserId">Identifier of the newly created user.</param>
/// <param name="Email">Normalized email address that was registered.</param>
public sealed record RegisterUserResult(Guid UserId, string Email);
