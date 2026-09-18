using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Finds courses a learner's subject combination could unlock, purely by subject presence (no NSC
/// level thresholds - see CourseComparisonService for a level-aware evaluation against a single
/// course), grouped by faculty as an approximation of "career fields unlocked" since this catalog
/// has no separate career taxonomy.
/// </summary>
public static class CareerMatchingService
{
    public static CareersUnlockedResult FindUnlockedCourses(IReadOnlyList<string> learnerSubjects)
    {
        var unlockedByFaculty = new Dictionary<string, List<UnlockedCourse>>(StringComparer.OrdinalIgnoreCase);

        foreach (var university in SouthAfricanUniversities.All)
        {
            foreach (var course in university.Courses)
            {
                if (course.SubjectRequirements.Count == 0 || !IsUnlocked(course, learnerSubjects))
                {
                    continue;
                }

                if (!unlockedByFaculty.TryGetValue(course.Faculty, out var list))
                {
                    list = new List<UnlockedCourse>();
                    unlockedByFaculty[course.Faculty] = list;
                }

                list.Add(new UnlockedCourse
                {
                    CourseId = course.Id,
                    CourseName = course.Name,
                    University = university.Name,
                    QualificationCode = course.QualificationCode
                });
            }
        }

        return new CareersUnlockedResult
        {
            FacultiesUnlocked = unlockedByFaculty
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => new FacultyCareerGroup { Faculty = kvp.Key, Courses = kvp.Value })
                .ToList(),
            Notes = "Courses are matched by subject presence only (NSC achievement levels are not checked here " +
                    "- see /api/compare for a level-aware evaluation of a single course). Faculties are used as " +
                    "an approximation of \"career fields\" since this catalog has no separate career taxonomy."
        };
    }

    private static bool IsUnlocked(Course course, IReadOnlyList<string> learnerSubjects)
    {
        return course.SubjectRequirements.All(requirement =>
            SubjectMatching.SplitAlternatives(requirement.Subject)
                .Any(alternative => learnerSubjects.Any(s => SubjectMatching.NamesMatch(s, alternative))));
    }
}
