using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Computes a generic approximation of the widely-used South African "APS" scale: the sum of NSC
/// achievement levels (1-7) across a learner's best 6 subjects, excluding Life Orientation. This is
/// a generic approximation only - each university's own published Total APS (see Course.TotalAPS)
/// may use a different, non-standard scale (e.g. UCT's Faculty/Weighted Points Score) and is not
/// recalculated here.
/// </summary>
public static class ApsCalculationService
{
    private const string ExcludedSubjectName = "Life Orientation";
    private const int SubjectsCounted = 6;

    public static ApsCalculationResult Calculate(IReadOnlyList<LearnerSubjectScore> subjects)
    {
        var lifeOrientation = subjects.Where(s => SubjectMatching.NamesMatch(s.Subject, ExcludedSubjectName)).ToList();
        var eligible = subjects.Except(lifeOrientation).OrderByDescending(s => s.Level).ToList();

        var counted = eligible.Take(SubjectsCounted).ToList();
        var excluded = lifeOrientation.Concat(eligible.Skip(SubjectsCounted)).ToList();

        return new ApsCalculationResult
        {
            TotalAps = counted.Sum(s => s.Level),
            SubjectsCounted = counted,
            SubjectsExcluded = excluded,
            Notes = "This is a generic approximation of the standard South African APS scale (sum of NSC " +
                    "levels for the best 6 subjects, excluding Life Orientation). It is not the same as any " +
                    "individual university's published Total APS, which may use a different, non-standard " +
                    "scale - see /api/compare for a course-specific evaluation."
        };
    }
}
