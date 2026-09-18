namespace TertiaryInstitutions.Models;

public class SavedCareerDetail
{
    public Guid Id { get; set; }
    public DateTime SavedAtUtc { get; set; }
    public Career Career { get; set; } = new();
}

public class JourneyProgressResponse
{
    public DateTime? ExploreCompletedAtUtc { get; set; }
    public DateTime? AssessCompletedAtUtc { get; set; }
    public DateTime? ShortlistCompletedAtUtc { get; set; }
    public DateTime? ApplyCompletedAtUtc { get; set; }
    public DateTime? EnrollCompletedAtUtc { get; set; }

    /// <summary>The first incomplete step in fixed order, or null if all 5 steps are complete.</summary>
    public JourneyStep? CurrentStep { get; set; }

    public static JourneyProgressResponse FromProgress(JourneyProgress progress) => new()
    {
        ExploreCompletedAtUtc = progress.ExploreCompletedAtUtc,
        AssessCompletedAtUtc = progress.AssessCompletedAtUtc,
        ShortlistCompletedAtUtc = progress.ShortlistCompletedAtUtc,
        ApplyCompletedAtUtc = progress.ApplyCompletedAtUtc,
        EnrollCompletedAtUtc = progress.EnrollCompletedAtUtc,
        CurrentStep = ComputeCurrentStep(progress)
    };

    private static JourneyStep? ComputeCurrentStep(JourneyProgress progress)
    {
        if (progress.ExploreCompletedAtUtc is null) return JourneyStep.Explore;
        if (progress.AssessCompletedAtUtc is null) return JourneyStep.Assess;
        if (progress.ShortlistCompletedAtUtc is null) return JourneyStep.Shortlist;
        if (progress.ApplyCompletedAtUtc is null) return JourneyStep.Apply;
        if (progress.EnrollCompletedAtUtc is null) return JourneyStep.Enroll;
        return null;
    }
}
