namespace TertiaryInstitutions.Models;

public class CareerMatchRequest
{
    /// <summary>Optional. The learner's subjects and NSC levels, for subject-fit scoring.</summary>
    public List<LearnerSubjectScore>? Subjects { get; set; }
}

public class CareerMatchResult
{
    public int CareerId { get; set; }
    public string Title { get; set; } = string.Empty;

    /// <summary>0-1, or null if subject-fit was not scored (no subjects supplied).</summary>
    public double? SubjectFitScore { get; set; }

    /// <summary>0-1, or null if RIASEC-fit was not scored (anonymous, or no assessment history).</summary>
    public double? RiasecFitScore { get; set; }

    /// <summary>0-1 combined score, used to rank results.</summary>
    public double OverallScore { get; set; }

    public List<SubjectRequirementResult> SubjectResults { get; set; } = new();
    public string Explanation { get; set; } = string.Empty;
}

public class CareerMatchResponse
{
    public List<CareerMatchResult> Matches { get; set; } = new();
    public bool UsedSubjects { get; set; }
    public bool UsedRiasec { get; set; }
    public string Notes { get; set; } = string.Empty;
}
