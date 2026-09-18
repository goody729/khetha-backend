using System.ComponentModel.DataAnnotations;

namespace TertiaryInstitutions.Models;

public class AssessmentAnswer
{
    [Required]
    public int QuestionId { get; set; }

    /// <summary>Agreement rating, 1 (strongly disagree) to 5 (strongly agree).</summary>
    [Range(1, 5)]
    public int Rating { get; set; }
}

public class AssessmentSubmitRequest
{
    [Required]
    [MinLength(1)]
    public List<AssessmentAnswer> Answers { get; set; } = new();
}

public class AssessmentResult
{
    public int RealisticScore { get; set; }
    public int InvestigativeScore { get; set; }
    public int ArtisticScore { get; set; }
    public int SocialScore { get; set; }
    public int EnterprisingScore { get; set; }
    public int ConventionalScore { get; set; }

    /// <summary>Top 3 RIASEC types by score, e.g. "SIA".</summary>
    public string ResultCode { get; set; } = string.Empty;

    /// <summary>True if the caller was authenticated and the result was saved to their profile.</summary>
    public bool Persisted { get; set; }
}

public class AssessmentSubmissionSummary
{
    public Guid Id { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
    public int RealisticScore { get; set; }
    public int InvestigativeScore { get; set; }
    public int ArtisticScore { get; set; }
    public int SocialScore { get; set; }
    public int EnterprisingScore { get; set; }
    public int ConventionalScore { get; set; }
    public string ResultCode { get; set; } = string.Empty;

    public static AssessmentSubmissionSummary FromSubmission(AssessmentSubmission submission) => new()
    {
        Id = submission.Id,
        SubmittedAtUtc = submission.SubmittedAtUtc,
        RealisticScore = submission.RealisticScore,
        InvestigativeScore = submission.InvestigativeScore,
        ArtisticScore = submission.ArtisticScore,
        SocialScore = submission.SocialScore,
        EnterprisingScore = submission.EnterprisingScore,
        ConventionalScore = submission.ConventionalScore,
        ResultCode = submission.ResultCode
    };
}
