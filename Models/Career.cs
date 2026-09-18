namespace TertiaryInstitutions.Models;

/// <summary>
/// A career directory entry: what it involves, what NSC subjects it typically requires, how to get
/// there, and which Holland Code (RIASEC) interest types it fits.
/// </summary>
public class Career
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Placeholder/demo OFO (Organising Framework for Occupations) code for structural purposes only.
    /// These are NOT verified official OFO codes and must not be used in production without
    /// verification against the real South African OFO.
    /// </summary>
    public string? OfoCode { get; set; }

    public string Summary { get; set; } = string.Empty;
    public List<string> Responsibilities { get; set; } = new();

    /// <summary>Reuses the <see cref="SubjectRequirement"/> shape from University.cs (subject + free-text level).</summary>
    public List<SubjectRequirement> RequiredSubjects { get; set; } = new();

    public List<string> Pathways { get; set; } = new();
    public List<RiasecType> RiasecTags { get; set; } = new();
}
