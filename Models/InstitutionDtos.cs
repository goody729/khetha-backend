using System.ComponentModel.DataAnnotations;

namespace TertiaryInstitutions.Models;

public class InstitutionSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    /// <summary>The university's prospectus link, falling back to <see cref="Website"/> when not set.</summary>
    public string ProspectusUrl { get; set; } = string.Empty;

    /// <summary>Straight-line distance in km from the caller's resolved location, when available; null otherwise.</summary>
    public double? DistanceKm { get; set; }

    public static InstitutionSummary FromUniversity(University university, double? distanceKm) => new()
    {
        Id = university.Id,
        Name = university.Name,
        Abbreviation = university.Abbreviation,
        Province = university.Province,
        City = university.City,
        Type = university.Type,
        Website = university.Website,
        ProspectusUrl = university.ProspectusUrl ?? university.Website,
        DistanceKm = distanceKm
    };
}

public class MatchingCoursesRequest
{
    [Required]
    [MinLength(1)]
    public List<LearnerSubjectScore> Subjects { get; set; } = new();
}

public class MatchingCoursesResult
{
    public int UniversityId { get; set; }
    public string University { get; set; } = string.Empty;
    public List<CourseComparisonResult> MatchingCourses { get; set; } = new();
    public List<CourseComparisonResult> NonMatchingCourses { get; set; } = new();
}
