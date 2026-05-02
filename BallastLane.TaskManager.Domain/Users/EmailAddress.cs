using BallastLane.TaskManager.Exceptions;

namespace BallastLane.TaskManager.Users;

/// <summary>
/// Value object representing a syntactically valid, normalized (trimmed and lowercased) email address.
/// Construction is gated through <see cref="From"/> so an instance is always well-formed.
/// </summary>
public sealed record EmailAddress
{
    private static readonly System.Text.RegularExpressions.Regex Pattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>The normalized email address string.</summary>
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    /// <summary>
    /// Validates and normalizes the supplied raw email string and returns an <see cref="EmailAddress"/> instance.
    /// </summary>
    /// <param name="raw">Caller-supplied email value, potentially with surrounding whitespace or mixed case.</param>
    /// <returns>A new <see cref="EmailAddress"/> with the value trimmed and lowercased.</returns>
    public static EmailAddress From(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainValidationException("Email", "Email is required.");

        var normalized = raw.Trim().ToLowerInvariant();
        if (!Pattern.IsMatch(normalized))
            throw new DomainValidationException("Email", "Email format is invalid.");

        return new EmailAddress(normalized);
    }

    /// <summary>Returns the normalized email string.</summary>
    public override string ToString() => Value;
}
