using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using BallastLane.TaskManager.Auth;
using BallastLane.TaskManager.Users;

namespace BallastLane.TaskManager.Infrastructure.Tests.Auth;

public sealed class JwtTokenIssuerTests
{
    private static readonly JwtSettings TestSettings = new()
    {
        Key = "TestSigningKey-MustBeAtLeast32BytesLong!!",
        Issuer = "https://test-issuer",
        Audience = "test-audience",
        ExpiryMinutes = 15,
    };

    private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2026, 5, 3, 12, 0, 0, TimeSpan.Zero));
    private readonly JwtTokenIssuer _sut;

    public JwtTokenIssuerTests()
    {
        _sut = new JwtTokenIssuer(Options.Create(TestSettings), _timeProvider);
    }

    private static User CreateTestUser() =>
        new(
            id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            email: EmailAddress.From("test@example.com"),
            passwordHash: "hashed",
            createdAt: DateTimeOffset.UtcNow);

    [Fact]
    public void Issue_ReturnsNonEmptyToken()
    {
        var result = _sut.Issue(CreateTestUser());
        result.Token.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Issue_ExpiresAt_MatchesConfiguredMinutes()
    {
        var result = _sut.Issue(CreateTestUser());
        var expectedExpiry = _timeProvider.GetUtcNow().AddMinutes(TestSettings.ExpiryMinutes);
        result.ExpiresAt.ShouldBe(expectedExpiry);
    }

    [Fact]
    public async Task Issue_TokenContainsCorrectSubClaim()
    {
        var user = CreateTestUser();
        var result = _sut.Issue(user);

        var handler = new JsonWebTokenHandler();
        var validation = await handler.ValidateTokenAsync(result.Token, BuildValidationParameters());

        validation.IsValid.ShouldBeTrue();
        validation.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            .ShouldBe(user.Id.ToString());
    }

    [Fact]
    public async Task Issue_TokenContainsEmailClaim()
    {
        var user = CreateTestUser();
        var result = _sut.Issue(user);

        var handler = new JsonWebTokenHandler();
        var validation = await handler.ValidateTokenAsync(result.Token, BuildValidationParameters());

        validation.IsValid.ShouldBeTrue();
        validation.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            .ShouldBe(user.Email.Value);
    }

    [Fact]
    public async Task Issue_TokenValidatesWithCorrectParameters()
    {
        var result = _sut.Issue(CreateTestUser());

        var handler = new JsonWebTokenHandler();
        var validation = await handler.ValidateTokenAsync(result.Token, BuildValidationParameters());

        validation.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task Issue_ExpiredToken_FailsValidation()
    {
        var result = _sut.Issue(CreateTestUser());

        _timeProvider.Advance(TimeSpan.FromMinutes(TestSettings.ExpiryMinutes + 5));

        var handler = new JsonWebTokenHandler();
        var validationParams = BuildValidationParameters();
        validationParams.ValidateLifetime = true;
        validationParams.LifetimeValidator = (notBefore, expires, token, parameters) =>
            expires > _timeProvider.GetUtcNow().UtcDateTime;

        var validation = await handler.ValidateTokenAsync(result.Token, validationParams);

        validation.IsValid.ShouldBeFalse();
    }

    [Fact]
    public async Task Issue_WrongSigningKey_FailsValidation()
    {
        var result = _sut.Issue(CreateTestUser());

        var handler = new JsonWebTokenHandler();
        var wrongParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("CompletelyWrongKey-MustBeAtLeast32Bytes!!")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
        };

        var validation = await handler.ValidateTokenAsync(result.Token, wrongParams);
        validation.IsValid.ShouldBeFalse();
    }

    private static TokenValidationParameters BuildValidationParameters() => new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(TestSettings.Key)),
        ValidateIssuer = true,
        ValidIssuer = TestSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = TestSettings.Audience,
        ValidateLifetime = false,
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
        ClockSkew = TimeSpan.FromSeconds(30),
    };
}
