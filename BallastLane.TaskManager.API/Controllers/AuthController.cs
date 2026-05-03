using BallastLane.TaskManager.API.Models;
using BallastLane.TaskManager.Auth;
using BallastLane.TaskManager.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.TaskManager.API.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(
    RegisterUserHandler registerHandler,
    LoginHandler loginHandler,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
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
            new RegisterUserCommand(request.Email, request.Password), ct);

        return StatusCode(StatusCodes.Status201Created,
            new RegisterResponse(result.UserId, result.Email));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
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
            new LoginCommand(request.Email, request.Password), ct);

        return Ok(new LoginResponse(result.Token, result.ExpiresAt));
    }
}
