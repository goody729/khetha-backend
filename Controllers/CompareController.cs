using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CompareController : ControllerBase
{
    /// <summary>
    /// Compares a learner's NSC subject results (achievement levels 1-7) against a course's published
    /// subject requirements, returning a per-subject pass/fail breakdown. The course id is the one
    /// returned by the universities/courses endpoints (e.g. GET /api/universities/{id}).
    /// </summary>
    /// <param name="courseId">The course's id.</param>
    /// <param name="subjects">The learner's subjects and NSC achievement levels (1-7 each).</param>
    [HttpPost("{courseId:int}")]
    [ProducesResponseType(typeof(CourseComparisonResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CourseComparisonResult> Compare(int courseId, [FromBody] List<LearnerSubjectScore> subjects)
    {
        if (subjects is null || subjects.Count == 0)
        {
            return BadRequest("Provide at least one subject with an NSC achievement level (1-7).");
        }

        if (subjects.Any(s => s.Level is < 1 or > 7))
        {
            return BadRequest("NSC achievement levels must be between 1 and 7.");
        }

        var result = CourseComparisonService.Compare(courseId, subjects);
        return result is null ? NotFound($"No course found with id {courseId}.") : Ok(result);
    }
}
