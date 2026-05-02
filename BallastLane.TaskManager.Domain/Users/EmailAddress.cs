using BallastLane.TaskManager.Domain.Exceptions;

namespace BallastLane.TaskManager.Domain.Users;

public sealed record EmailAddress
{
    private static readonly System.Text.RegularExpressions.Regex Pattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", System.Text.RegularExpressions.RegexOptions.Compiled);

    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress From(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainValidationException("Email", "Email is required.");

        var normalized = raw.Trim().ToLowerInvariant();
        if (!Pattern.IsMatch(normalized))
            throw new DomainValidationException("Email", "Email format is invalid.");

        return new EmailAddress(normalized);
    }

    public override string ToString() => Value;
}
