namespace TertiaryInstitutions.Models;

public class University
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    /// <summary>
    /// Direct link to the university's prospectus, when known. Not populated in the current static
    /// catalog (left null) - callers should fall back to <see cref="Website"/> when this is null.
    /// </summary>
    public string? ProspectusUrl { get; set; }

    public List<Course> Courses { get; set; } = new();
}

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string QualificationCode { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string Campus { get; set; } = string.Empty;
    public List<SubjectRequirement> SubjectRequirements { get; set; } = new();
    public int TotalAPS { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class SubjectRequirement
{
    public string Subject { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}
