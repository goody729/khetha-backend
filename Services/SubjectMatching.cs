using System.Text.RegularExpressions;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Shared fuzzy subject-name matching used by the course-comparison, APS-calculation and
/// careers-unlocked features when comparing a learner's subjects against free-text
/// university-published requirement text.
/// </summary>
public static class SubjectMatching
{
    private static readonly Regex SubjectSplitPattern = new(
        @"\s+and/or\s+|\s+or\s+|\s*/\s*|,",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Splits requirement text like "Mathematics or Mathematical Literacy" / "Physical Sciences, Life Sciences"
    /// into its individual subject-name alternatives.
    /// </summary>
    public static List<string> SplitAlternatives(string subjectText)
    {
        var tokens = SubjectSplitPattern.Split(subjectText)
            .Select(t => t.Replace("(", " ").Replace(")", " ").Trim())
            .Where(t => t.Length > 0)
            .ToList();

        if (tokens.Count == 0)
        {
            tokens.Add(subjectText.Trim());
        }

        return tokens;
    }

    public static bool NamesMatch(string a, string b)
    {
        var normalizedA = Normalize(a);
        var normalizedB = Normalize(b);
        if (normalizedA.Length == 0 || normalizedB.Length == 0)
        {
            return false;
        }

        return normalizedA.Equals(normalizedB, StringComparison.OrdinalIgnoreCase)
            || normalizedA.Contains(normalizedB, StringComparison.OrdinalIgnoreCase)
            || normalizedB.Contains(normalizedA, StringComparison.OrdinalIgnoreCase);
    }

    public static string Normalize(string s) =>
        Regex.Replace(s, @"[()]", " ").Replace("  ", " ").Trim();
}
