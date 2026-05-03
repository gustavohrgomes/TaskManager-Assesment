using System.Security.Claims;
using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Exceptions;

namespace BallastLane.TaskManager.API.Auth;

public sealed class ClaimsPrincipalUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId
    {
        get
        {
            var sub = httpContextAccessor.HttpContext!.User.FindFirstValue("sub");

            if (sub is null || !Guid.TryParse(sub, out var userId))
                throw new UnauthorizedException("Invalid or missing user identity.");

            return userId;
        }
    }
}
