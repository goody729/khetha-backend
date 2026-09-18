namespace TertiaryInstitutions.Models;

/// <summary>
/// The six Holland Code (RIASEC) personality/interest types used by career-fit assessments.
/// </summary>
public enum RiasecType
{
    Realistic,
    Investigative,
    Artistic,
    Social,
    Enterprising,
    Conventional
}

/// <summary>
/// A single job-fit quiz question, rated by the learner on a 1-5 agreement scale.
/// </summary>
public class HollandCodeQuestion
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public RiasecType Type { get; set; }
}
