namespace TertiaryInstitutions.Models;

/// <summary>
/// A learner's mark (percentage) for one subject in one school term, taken from their report card.
/// </summary>
public class TermResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LearnerId { get; set; }

    /// <summary>Calendar year the term falls in, e.g. 2026.</summary>
    public int Year { get; set; }

    /// <summary>School term, 1-4.</summary>
    public int Term { get; set; }

    public string Subject { get; set; } = string.Empty;

    /// <summary>Report-card mark as a percentage, 0-100.</summary>
    public double Percentage { get; set; }

    public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// A learner's device registered to receive push notifications.
/// </summary>
public class DeviceToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LearnerId { get; set; }

    /// <summary>The push provider's device token (e.g. an FCM registration token).</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>"android", "ios" or "web".</summary>
    public string Platform { get; set; } = string.Empty;

    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Records that a report-card reminder was sent, so a restart of the worker never double-sends.
/// </summary>
public class ReminderLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LearnerId { get; set; }

    /// <summary>The year/term whose marks the reminder asked for.</summary>
    public int ForYear { get; set; }
    public int ForTerm { get; set; }

    /// <summary>The (South African) date the reminder was sent.</summary>
    public DateOnly SentOn { get; set; }
}
