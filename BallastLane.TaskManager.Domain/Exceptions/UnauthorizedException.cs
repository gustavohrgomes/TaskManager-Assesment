namespace BallastLane.TaskManager.Domain.Exceptions;

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
