namespace BallastLane.TaskManager.Abstractions;

/// <summary>
/// Exposes the currently authenticated caller's identity to application handlers, decoupling them
/// from HTTP/<c>HttpContext</c> specifics.
/// </summary>
public interface IUserContext
{
    /// <summary>Identifier of the authenticated user making the current request.</summary>
    Guid UserId { get; }
}
