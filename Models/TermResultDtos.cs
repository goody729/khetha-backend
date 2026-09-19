using System.ComponentModel.DataAnnotations;

namespace TertiaryInstitutions.Models;

public class SubmitTermResultsRequest
{
    /// <summary>Calendar year of the term, e.g. 2026.</summary>
    [Range(2000, 2100)]
    public int Year { get; set; }

    /// <summary>School term, 1-4.</summary>
    [Range(1, 4)]
    public int Term { get; set; }

    /// <summary>Report-card marks. Re-submitting a subject for the same term overwrites its mark.</summary>
    [Required, MinLength(1), MaxLength(15)]
    public List<TermSubjectMark> Subjects { get; set; } = new();
}

public class TermSubjectMark
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Subject { get; set; } = string.Empty;

    /// <summary>Report-card mark as a percentage, 0-100.</summary>
    [Range(0, 100)]
    public double Percentage { get; set; }
}

public class TermResultsResponse
{
    public int Year { get; set; }
    public int Term { get; set; }
    public List<TermSubjectResult> Subjects { get; set; } = new();
}

public class TermSubjectResult
{
    public string Subject { get; set; } = string.Empty;
    public double Percentage { get; set; }

    /// <summary>NSC achievement level, 1-7, derived from the percentage.</summary>
    public int Level { get; set; }
}

public class TermProgressResponse
{
    /// <summary>The most recent year/term the learner has entered marks for; null if none.</summary>
    public int? LatestYear { get; set; }
    public int? LatestTerm { get; set; }

    public List<SubjectTrend> Subjects { get; set; } = new();
    public List<CareerProgress> Careers { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public class SubjectTrend
{
    public string Subject { get; set; } = string.Empty;
    public double LatestPercentage { get; set; }
    public int LatestLevel { get; set; }
    public double? PreviousPercentage { get; set; }

    /// <summary>Latest minus previous term's percentage; null when there is no earlier mark.</summary>
    public double? Change { get; set; }

    /// <summary>"Improving", "Declining", "Steady" or "New".</summary>
    public string Trend { get; set; } = string.Empty;
}

public class CareerProgress
{
    public int CareerId { get; set; }
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// "OnTrack", "Close" (unmet subjects are within one level), "NeedsAttention", "NoMarks" (learner has
    /// entered no marks yet) or "NoRequirements" (the career lists no subject requirements).
    /// </summary>
    public string Status { get; set; } = string.Empty;

    public List<RequirementProgress> Requirements { get; set; } = new();
}

public class RequirementProgress
{
    public string RequiredSubject { get; set; } = string.Empty;
    public string RequiredLevelText { get; set; } = string.Empty;
    public string? CurrentSubject { get; set; }
    public double? CurrentPercentage { get; set; }
    public int? CurrentLevel { get; set; }
    public bool Met { get; set; }

    /// <summary>How many NSC levels below the requirement the learner currently is; null if unknown or met.</summary>
    public int? LevelsShort { get; set; }

    public string Message { get; set; } = string.Empty;
}

public class CoachResponse
{
    /// <summary>The grounded, deterministic progress the coaching is based on.</summary>
    public TermProgressResponse Progress { get; set; } = new();

    /// <summary>AI coaching; null when the AI service is unavailable (progress is still returned).</summary>
    public CoachReply? Coaching { get; set; }

    public string? Notice { get; set; }
}

/// <summary>Structured AI coaching on a learner's term marks.</summary>
/// <param name="Summary">2-3 sentence overall read on whether they are on track.</param>
/// <param name="OnTrack">True only when the marks support their career goal(s).</param>
/// <param name="FocusAreas">Specific subjects to work on, with practical advice.</param>
/// <param name="Encouragement">One short motivating line.</param>
public record CoachReply(
    string Summary,
    bool? OnTrack = null,
    IReadOnlyList<CoachFocusArea>? FocusAreas = null,
    string? Encouragement = null);

public record CoachFocusArea(string Subject, string Advice);

public class RegisterDeviceRequest
{
    [Required, StringLength(512, MinimumLength = 10)]
    public string Token { get; set; } = string.Empty;

    /// <summary>"android", "ios" or "web".</summary>
    [Required, RegularExpression("^(android|ios|web)$", ErrorMessage = "Platform must be android, ios or web.")]
    public string Platform { get; set; } = string.Empty;
}

public class ReportCardStatusResponse
{
    /// <summary>Terms the learner should have entered marks for by now but hasn't.</summary>
    public List<MissingTerm> MissingTerms { get; set; } = new();

    /// <summary>The next scheduled report-card reminder date (SAST), if any.</summary>
    public DateOnly? NextReminderOn { get; set; }
}

public class MissingTerm
{
    public int Year { get; set; }
    public int Term { get; set; }
}

public class CalendarResponse
{
    public int Year { get; set; }
    public List<CalendarTerm> Terms { get; set; } = new();
    public List<CalendarHoliday> Holidays { get; set; } = new();
    public List<CalendarReminder> ReportCardReminders { get; set; } = new();
}

public class CalendarTerm
{
    public int Term { get; set; }
    public DateOnly Opens { get; set; }
    public DateOnly Closes { get; set; }
}

public class CalendarHoliday
{
    public int AfterTerm { get; set; }
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
}

public class CalendarReminder
{
    public DateOnly Date { get; set; }

    /// <summary>The year/term whose marks the reminder asks the learner to enter.</summary>
    public int ForYear { get; set; }
    public int ForTerm { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
