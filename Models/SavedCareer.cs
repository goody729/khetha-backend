namespace TertiaryInstitutions.Models;

/// <summary>
/// A career a learner has bookmarked from the static Data/Careers.All catalog. CareerId references
/// that static catalog by id, not a DB foreign key (careers are not persisted).
/// </summary>
public class SavedCareer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LearnerId { get; set; }
    public int CareerId { get; set; }
    public DateTime SavedAtUtc { get; set; } = DateTime.UtcNow;
}
