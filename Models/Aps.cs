namespace TertiaryInstitutions.Models;

public class ApsCalculationResult
{
    public int TotalAps { get; set; }
    public List<LearnerSubjectScore> SubjectsCounted { get; set; } = new();
    public List<LearnerSubjectScore> SubjectsExcluded { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}
