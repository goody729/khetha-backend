namespace TertiaryInstitutions.Models;

/// <summary>
/// One row per learner tracking completion timestamps for each of the 5 fixed journey steps
/// (see JourneyStep). LearnerId is both the primary key and a 1:1 foreign key to Learner.
/// </summary>
public class JourneyProgress
{
    public Guid LearnerId { get; set; }
    public DateTime? ExploreCompletedAtUtc { get; set; }
    public DateTime? AssessCompletedAtUtc { get; set; }
    public DateTime? ShortlistCompletedAtUtc { get; set; }
    public DateTime? ApplyCompletedAtUtc { get; set; }
    public DateTime? EnrollCompletedAtUtc { get; set; }
}
