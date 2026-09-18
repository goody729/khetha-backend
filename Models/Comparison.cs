namespace TertiaryInstitutions.Models;

public class LearnerSubjectScore
{
    public string Subject { get; set; } = string.Empty;

    /// <summary>NSC achievement level, from 1 (0-29%) to 7 (80-100%).</summary>
    public int Level { get; set; }
}

public class SubjectRequirementResult
{
    public string RequiredSubject { get; set; } = string.Empty;
    public string RequiredLevelText { get; set; } = string.Empty;
    public string? MatchedLearnerSubject { get; set; }
    public int? MatchedLearnerLevel { get; set; }
    public bool Met { get; set; }
    public string Explanation { get; set; } = string.Empty;
}

public class CourseComparisonResult
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public string QualificationCode { get; set; } = string.Empty;
    public int PublishedTotalAPS { get; set; }
    public List<SubjectRequirementResult> SubjectResults { get; set; } = new();
    public bool MeetsAllSubjectRequirements { get; set; }
    public string Notes { get; set; } = string.Empty;
}
