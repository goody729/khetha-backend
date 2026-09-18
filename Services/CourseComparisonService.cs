using System.Text.RegularExpressions;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Compares a learner's NSC subject results against a course's published subject requirements.
///
/// Requirement text is stored as free-form strings sourced from each university's own prospectus, so this
/// is a best-effort text parser rather than an exact evaluator: it extracts NSC achievement levels (and
/// converts published percentages to their equivalent level band), splits "A or B" / "A and/or B" subject
/// alternatives, and picks up alternate subject+level pairs embedded in the level text (e.g. "3 or
/// Mathematical Literacy 4"). Results should be treated as a strong guide, not a final admission decision -
/// unusual phrasing (portfolios, auditions, aggregate-based selection) is called out in Notes rather than
/// evaluated.
/// </summary>
public static class CourseComparisonService
{
    // Matches an NSC level (1-7) as a whole word, optionally preceded by a short qualifying phrase
    // (e.g. "Mathematical Literacy 4", "Home Language 5"), and optionally followed by a parenthetical
    // percentage that just decorates the same level (e.g. "5 (60%+)") so it isn't parsed again below.
    private static readonly Regex LevelPattern = new(
        @"(?<phrase>[A-Za-z][A-Za-z\s]{0,40}?)?\s*\b(?<level>[1-7])\b(?:\s*\(\s*\d{1,3}\s*%?\s*[-+]?\s*(?:\d{1,3}\s*%)?\s*\))?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Matches a bare percentage (e.g. "65%") for requirements published purely as a percentage, optionally
    // preceded by a short qualifying phrase.
    private static readonly Regex PercentPattern = new(
        @"(?<phrase>[A-Za-z][A-Za-z\s]{0,40}?)?\s*\b(?<pct>\d{1,3})\s*%",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static CourseComparisonResult? Compare(int courseId, IReadOnlyList<LearnerSubjectScore> learnerSubjects)
    {
        var found = SouthAfricanUniversities.FindCourse(courseId);
        if (found is null)
        {
            return null;
        }

        var (university, course) = found.Value;

        var subjectResults = course.SubjectRequirements
            .Select(requirement => EvaluateRequirement(requirement, learnerSubjects))
            .ToList();

        return new CourseComparisonResult
        {
            CourseId = course.Id,
            CourseName = course.Name,
            University = university.Name,
            QualificationCode = course.QualificationCode,
            PublishedTotalAPS = course.TotalAPS,
            SubjectResults = subjectResults,
            MeetsAllSubjectRequirements = subjectResults.Count > 0 && subjectResults.All(r => r.Met),
            Notes = "Subject matching is best-effort text parsing over differently formatted university " +
                    "requirement tables and should be verified against the official prospectus, especially " +
                    "for portfolio-, audition- or aggregate-based selection. The published Total APS is on " +
                    "each university's own scale (a simple NSC level sum for UP/UJ/Wits/UKZN, or a Faculty/" +
                    "Weighted Points Score for UCT) and is not recalculated here, since APS/FPS formulas " +
                    "differ by university."
        };
    }

    /// <summary>
    /// Internal (not private) so <see cref="CareerMatchingScoreService"/> can reuse the same
    /// subject/level requirement evaluation instead of duplicating it.
    /// </summary>
    internal static SubjectRequirementResult EvaluateRequirement(SubjectRequirement requirement, IReadOnlyList<LearnerSubjectScore> learnerSubjects)
    {
        var candidates = BuildCandidates(requirement);

        if (candidates.Count == 0)
        {
            return new SubjectRequirementResult
            {
                RequiredSubject = requirement.Subject,
                RequiredLevelText = requirement.Level,
                Met = false,
                Explanation = "Could not automatically parse the required NSC level from the published text; check this requirement manually."
            };
        }

        string? matchedSubject = null;
        int? matchedLevel = null;
        int? matchedRequiredLevel = null;
        var met = false;

        foreach (var (candidateName, requiredLevel) in candidates)
        {
            var learnerSubject = learnerSubjects.FirstOrDefault(s => SubjectMatching.NamesMatch(s.Subject, candidateName));
            if (learnerSubject is null)
            {
                continue;
            }

            if (matchedLevel is null || learnerSubject.Level > matchedLevel)
            {
                matchedSubject = learnerSubject.Subject;
                matchedLevel = learnerSubject.Level;
                matchedRequiredLevel = requiredLevel;
            }

            if (learnerSubject.Level >= requiredLevel)
            {
                met = true;
                matchedSubject = learnerSubject.Subject;
                matchedLevel = learnerSubject.Level;
                matchedRequiredLevel = requiredLevel;
                break;
            }
        }

        var explanation = matchedSubject is null
            ? $"No matching subject was found among the learner's submitted results for '{requirement.Subject}'."
            : met
                ? $"'{matchedSubject}' at level {matchedLevel} meets the required level {matchedRequiredLevel}."
                : $"'{matchedSubject}' at level {matchedLevel} does not meet the required level {matchedRequiredLevel}.";

        return new SubjectRequirementResult
        {
            RequiredSubject = requirement.Subject,
            RequiredLevelText = requirement.Level,
            MatchedLearnerSubject = matchedSubject,
            MatchedLearnerLevel = matchedLevel,
            Met = met,
            Explanation = explanation
        };
    }

    private static List<(string Name, int Level)> BuildCandidates(SubjectRequirement requirement)
    {
        var candidates = new List<(string Name, int Level)>();

        var baseSubject = requirement.Subject.Split('(')[0].Trim();
        baseSubject = SubjectMatching.SplitAlternatives(baseSubject).FirstOrDefault()?.Trim() ?? baseSubject;

        var subjectTokens = SubjectMatching.SplitAlternatives(requirement.Subject);

        var consumedRanges = new List<(int Start, int End)>();

        foreach (Match match in LevelPattern.Matches(requirement.Level))
        {
            consumedRanges.Add((match.Index, match.Index + match.Length));
            var phrase = match.Groups["phrase"].Success ? match.Groups["phrase"].Value.Trim() : "";
            var level = int.Parse(match.Groups["level"].Value);
            AddCandidate(candidates, subjectTokens, baseSubject, phrase, level);
        }

        foreach (Match match in PercentPattern.Matches(requirement.Level))
        {
            if (consumedRanges.Any(r => match.Index < r.End && match.Index + match.Length > r.Start))
            {
                continue;
            }

            var phrase = match.Groups["phrase"].Success ? match.Groups["phrase"].Value.Trim() : "";
            var pct = int.Parse(match.Groups["pct"].Value);
            AddCandidate(candidates, subjectTokens, baseSubject, phrase, PercentToLevel(pct));
        }

        return candidates
            .GroupBy(c => (Name: c.Name.Trim().ToLowerInvariant(), c.Level))
            .Select(g => (Name: g.First().Name.Trim(), Level: g.Key.Level))
            .ToList();
    }

    private static void AddCandidate(List<(string Name, int Level)> candidates, List<string> subjectTokens, string baseSubject, string phrase, int level)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            foreach (var token in subjectTokens)
            {
                candidates.Add((token, level));
            }
        }
        else
        {
            candidates.Add((phrase, level));
            candidates.Add(($"{baseSubject} {phrase}".Trim(), level));
        }
    }

    private static int PercentToLevel(int pct)
    {
        if (pct >= 80) return 7;
        if (pct >= 70) return 6;
        if (pct >= 60) return 5;
        if (pct >= 50) return 4;
        if (pct >= 40) return 3;
        if (pct >= 30) return 2;
        return 1;
    }
}
