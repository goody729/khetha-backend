using System.Text.RegularExpressions;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Turns a learner's term marks into NSC levels and checks whether they are on track for the
/// subject requirements of their career goals. Deterministic - the AI coach only explains this.
/// </summary>
public static class TermProgressService
{
    private static readonly Regex FirstLevel = new(@"\b([1-7])\b", RegexOptions.Compiled);

    /// <summary>NSC achievement level: 7 = 80-100%, 6 = 70-79, 5 = 60-69, 4 = 50-59, 3 = 40-49, 2 = 30-39, 1 = 0-29.</summary>
    public static int PercentageToLevel(double percentage) => percentage switch
    {
        >= 80 => 7,
        >= 70 => 6,
        >= 60 => 5,
        >= 50 => 4,
        >= 40 => 3,
        >= 30 => 2,
        _ => 1
    };

    public static TermProgressResponse Evaluate(IReadOnlyList<TermResult> results, IReadOnlyList<Career> goals)
    {
        var bySubject = results
            .GroupBy(r => r.Subject, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderByDescending(r => r.Year).ThenByDescending(r => r.Term).ToList())
            .ToList();

        var latest = results.OrderByDescending(r => r.Year).ThenByDescending(r => r.Term).FirstOrDefault();

        var trends = bySubject.Select(history =>
        {
            var current = history[0];
            var previous = history.Count > 1 ? history[1] : null;
            var change = previous is null ? (double?)null : Math.Round(current.Percentage - previous.Percentage, 1);
            return new SubjectTrend
            {
                Subject = current.Subject,
                LatestPercentage = current.Percentage,
                LatestLevel = PercentageToLevel(current.Percentage),
                PreviousPercentage = previous?.Percentage,
                Change = change,
                Trend = change switch
                {
                    null => "New",
                    >= 2 => "Improving",
                    <= -2 => "Declining",
                    _ => "Steady"
                }
            };
        }).OrderBy(t => t.Subject).ToList();

        var levels = trends
            .Select(t => new LearnerSubjectScore { Subject = t.Subject, Level = t.LatestLevel })
            .ToList();

        return new TermProgressResponse
        {
            LatestYear = latest?.Year,
            LatestTerm = latest?.Term,
            Subjects = trends,
            Careers = goals.Select(career => EvaluateCareer(career, levels, trends)).ToList(),
            Notes = "Term marks are an early indicator only. Final NSC results, and each institution's own " +
                    "admission requirements, decide admission - always check the official prospectus."
        };
    }

    private static CareerProgress EvaluateCareer(Career career, List<LearnerSubjectScore> levels, List<SubjectTrend> trends)
    {
        var requirements = career.RequiredSubjects.Select(req =>
        {
            var result = CourseComparisonService.EvaluateRequirement(req, levels);
            var trend = result.MatchedLearnerSubject is null
                ? null
                : trends.FirstOrDefault(t => t.Subject.Equals(result.MatchedLearnerSubject, StringComparison.OrdinalIgnoreCase));

            int? levelsShort = null;
            if (!result.Met && result.MatchedLearnerLevel is not null
                && FirstLevel.Match(result.RequiredLevelText) is { Success: true } m)
            {
                levelsShort = Math.Max(0, int.Parse(m.Value) - result.MatchedLearnerLevel.Value);
            }

            return new RequirementProgress
            {
                RequiredSubject = result.RequiredSubject,
                RequiredLevelText = result.RequiredLevelText,
                CurrentSubject = result.MatchedLearnerSubject,
                CurrentPercentage = trend?.LatestPercentage,
                CurrentLevel = result.MatchedLearnerLevel,
                Met = result.Met,
                LevelsShort = levelsShort,
                Message = result.Explanation
            };
        }).ToList();

        string status;
        if (levels.Count == 0)
        {
            status = "NoMarks";
        }
        else if (requirements.Count == 0)
        {
            status = "NoRequirements";
        }
        else if (requirements.All(r => r.Met))
        {
            status = "OnTrack";
        }
        else if (requirements.Where(r => !r.Met).All(r => r.LevelsShort is <= 1))
        {
            status = "Close";
        }
        else
        {
            status = "NeedsAttention";
        }

        return new CareerProgress { CareerId = career.Id, Title = career.Title, Status = status, Requirements = requirements };
    }
}
