using System.ComponentModel.DataAnnotations;

namespace TertiaryInstitutions.Models;

public class CareersUnlockedRequest
{
    [Required]
    [MinLength(1)]
    public List<string> Subjects { get; set; } = new();
}

public class UnlockedCourse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public string QualificationCode { get; set; } = string.Empty;
}

public class FacultyCareerGroup
{
    public string Faculty { get; set; } = string.Empty;
    public List<UnlockedCourse> Courses { get; set; } = new();
}

public class CareersUnlockedResult
{
    public List<FacultyCareerGroup> FacultiesUnlocked { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}
