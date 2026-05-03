using BallastLane.TaskManager.Auth.Login;
using BallastLane.TaskManager.Auth.Register;
using BallastLane.TaskManager.Exceptions;
using BallastLane.TaskManager.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.TaskManager.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(
    RegisterUserHandler registerHandler,
    LoginHandler loginHandler) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request, [FromServices] IValidator<RegisterRequest> registerValidator, CancellationToken cancellationToken)
    {
        var validation = registerValidator.Validate(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
                .ToList();
            throw new DomainValidationException(errors);
        }

        var result = await registerHandler.Handle(
            new RegisterUserCommand(request.Email, request.Password), cancellationToken);

        return StatusCode(StatusCodes.Status201Created,
            new RegisterResponse(result.UserId, result.Email));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, [FromServices] IValidator<LoginRequest> loginValidator, CancellationToken cancellationToken)
    {
        var validation = loginValidator.Validate(request);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
                .ToList();
            throw new DomainValidationException(errors);
        }

        var result = await loginHandler.Handle(
            new LoginCommand(request.Email, request.Password), cancellationToken);

        return Ok(new LoginResponse(result.Token, result.ExpiresAt));
    }
}
