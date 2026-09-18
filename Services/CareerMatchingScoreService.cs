using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Ranks the career directory (Data/Careers.All) against a learner's subjects and/or their most
/// recent job-fit quiz (RIASEC) result. This is a different concern from CareerMatchingService,
/// which finds courses a subject combo unlocks by presence-only matching against course data.
/// </summary>
public static class CareerMatchingScoreService
{
    // Weight given to the learner's 1st/2nd/3rd strongest RIASEC type (from their quiz ResultCode),
    // normalized by the sum of these weights (1.9) to produce a 0-1 RiasecFitScore.
    private static readonly double[] RiasecRankWeights = { 1.0, 0.6, 0.3 };
    private const double RiasecMaxWeight = 1.9;

    private static readonly Dictionary<char, RiasecType> RiasecByInitial =
        Enum.GetValues<RiasecType>().ToDictionary(t => t.ToString()[0], t => t);

    public static CareerMatchResponse MatchAll(IReadOnlyList<LearnerSubjectScore>? subjects, AssessmentSubmission? latestAssessment)
    {
        var hasSubjects = subjects is { Count: > 0 };
        var riasecWeights = latestAssessment is null ? null : BuildRiasecWeights(latestAssessment.ResultCode);

        var results = Careers.All
            .Select(career => Score(career, hasSubjects ? subjects : null, riasecWeights))
            .OrderByDescending(r => r.OverallScore)
            .ToList();

        return new CareerMatchResponse
        {
            Matches = results,
            UsedSubjects = hasSubjects,
            UsedRiasec = riasecWeights is not null,
            Notes = "SubjectFitScore is the share of a career's listed required subjects the supplied " +
                    "subjects satisfy. RiasecFitScore is a weighted overlap between the career's RIASEC " +
                    "tags and the learner's most recent job-fit quiz result. OverallScore blends both " +
                    "signals when available (60% subject fit / 40% RIASEC fit), or uses whichever single " +
                    "signal is available."
        };
    }

    private static CareerMatchResult Score(
        Career career, IReadOnlyList<LearnerSubjectScore>? subjects, IReadOnlyDictionary<RiasecType, double>? riasecWeights)
    {
        double? subjectFit = null;
        var subjectResults = new List<SubjectRequirementResult>();
        if (subjects is not null && career.RequiredSubjects.Count > 0)
        {
            subjectResults = career.RequiredSubjects
                .Select(requirement => CourseComparisonService.EvaluateRequirement(requirement, subjects))
                .ToList();
            subjectFit = (double)subjectResults.Count(r => r.Met) / subjectResults.Count;
        }

        double? riasecFit = null;
        if (riasecWeights is not null)
        {
            var matchedWeight = career.RiasecTags.Sum(tag => riasecWeights.TryGetValue(tag, out var weight) ? weight : 0.0);
            riasecFit = Math.Min(1.0, matchedWeight / RiasecMaxWeight);
        }

        var overall = subjectFit is not null && riasecFit is not null
            ? 0.6 * subjectFit.Value + 0.4 * riasecFit.Value
            : subjectFit ?? riasecFit ?? 0.0;

        var explanationParts = new List<string>();
        if (subjectFit is not null)
        {
            explanationParts.Add($"Meets {subjectResults.Count(r => r.Met)}/{subjectResults.Count} required subjects.");
        }
        if (riasecFit is not null)
        {
            explanationParts.Add($"RIASEC interest overlap: {riasecFit:P0}.");
        }

        return new CareerMatchResult
        {
            CareerId = career.Id,
            Title = career.Title,
            SubjectFitScore = subjectFit,
            RiasecFitScore = riasecFit,
            OverallScore = overall,
            SubjectResults = subjectResults,
            Explanation = explanationParts.Count > 0 ? string.Join(" ", explanationParts) : "No scoring signal available."
        };
    }

    private static Dictionary<RiasecType, double> BuildRiasecWeights(string resultCode)
    {
        var weights = new Dictionary<RiasecType, double>();
        for (var i = 0; i < resultCode.Length && i < RiasecRankWeights.Length; i++)
        {
            if (RiasecByInitial.TryGetValue(resultCode[i], out var type))
            {
                weights[type] = RiasecRankWeights[i];
            }
        }
        return weights;
    }
}
