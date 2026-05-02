namespace BallastLane.TaskManager.Exceptions;

/// <summary>
/// Raised when an operation requires an authenticated caller and none was supplied or the credentials are invalid.
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException()
        : base("Authentication required.")
    {
    }

    public UnauthorizedException(string message) : base(message)
    {
    }
}
