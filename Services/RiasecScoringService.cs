using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Scores a learner's job-fit quiz answers into per-RIASEC-type totals and a top-3 Holland Code
/// (e.g. "SIA" for Social/Investigative/Artistic).
/// </summary>
public static class RiasecScoringService
{
    public static (bool Valid, string? Error) Validate(IReadOnlyList<AssessmentAnswer> answers)
    {
        var validQuestionIds = HollandCodeQuestions.All.Select(q => q.Id).ToHashSet();

        foreach (var answer in answers)
        {
            if (!validQuestionIds.Contains(answer.QuestionId))
            {
                return (false, $"Unknown question id {answer.QuestionId}.");
            }

            if (answer.Rating is < 1 or > 5)
            {
                return (false, "Ratings must be between 1 and 5.");
            }
        }

        return (true, null);
    }

    public static AssessmentResult Score(IReadOnlyList<AssessmentAnswer> answers)
    {
        var questionsById = HollandCodeQuestions.All.ToDictionary(q => q.Id);
        var scores = Enum.GetValues<RiasecType>().ToDictionary(t => t, _ => 0);

        foreach (var answer in answers)
        {
            var type = questionsById[answer.QuestionId].Type;
            scores[type] += answer.Rating;
        }

        var resultCode = string.Concat(
            scores.OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => (int)kvp.Key)
                .Take(3)
                .Select(kvp => kvp.Key.ToString()[0]));

        return new AssessmentResult
        {
            RealisticScore = scores[RiasecType.Realistic],
            InvestigativeScore = scores[RiasecType.Investigative],
            ArtisticScore = scores[RiasecType.Artistic],
            SocialScore = scores[RiasecType.Social],
            EnterprisingScore = scores[RiasecType.Enterprising],
            ConventionalScore = scores[RiasecType.Conventional],
            ResultCode = resultCode
        };
    }
}
