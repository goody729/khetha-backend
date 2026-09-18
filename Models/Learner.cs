namespace TertiaryInstitutions.Models;

/// <summary>
/// A registered learner's profile and credentials.
/// </summary>
public class Learner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>School grade, 8-12.</summary>
    public int Grade { get; set; }

    /// <summary>Preferred language, e.g. one of the 11 official South African languages.</summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>Academic/subject track, e.g. Science, Commerce, Humanities.</summary>
    public string Track { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<AssessmentSubmission> AssessmentSubmissions { get; set; } = new List<AssessmentSubmission>();
}
