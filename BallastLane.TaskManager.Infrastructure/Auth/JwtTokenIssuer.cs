using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Common;
using BallastLane.TaskManager.Users;

namespace BallastLane.TaskManager.Auth;

internal sealed class JwtTokenIssuer : IJwtTokenIssuer
{
    private readonly JwtSettings _settings;
    private readonly TimeProvider _timeProvider;
    private readonly JsonWebTokenHandler _handler = new();

    public JwtTokenIssuer(IOptions<JwtSettings> settings, TimeProvider timeProvider)
    {
        _settings = settings.Value;
        _timeProvider = timeProvider;
    }

    public IssuedToken Issue(User user)
    {
        var now = _timeProvider.GetUtcNow();
        var expires = now.AddMinutes(_settings.ExpiryMinutes);
        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_settings.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            ]),
            Expires = expires.UtcDateTime,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
        };

        var token = _handler.CreateToken(descriptor);
        return new IssuedToken(token, expires);
    }
}
