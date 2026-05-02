using BallastLane.TaskManager.Application.Common;
using BallastLane.TaskManager.Domain.Users;

namespace BallastLane.TaskManager.Application.Abstractions;

public interface IJwtTokenIssuer
{
    IssuedToken Issue(User user);
}
