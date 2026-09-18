namespace TertiaryInstitutions.Models;

/// <summary>
/// A persisted result of a learner's job-fit quiz submission (only saved for authenticated learners).
/// </summary>
public class AssessmentSubmission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LearnerId { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;

    public int RealisticScore { get; set; }
    public int InvestigativeScore { get; set; }
    public int ArtisticScore { get; set; }
    public int SocialScore { get; set; }
    public int EnterprisingScore { get; set; }
    public int ConventionalScore { get; set; }

    /// <summary>Top 3 RIASEC types by score, e.g. "SIA".</summary>
    public string ResultCode { get; set; } = string.Empty;
}
