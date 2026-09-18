using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ApsController : ControllerBase
{
    /// <summary>
    /// Calculates a generic APS (Admission Point Score) approximation from a learner's NSC subject
    /// results: the sum of achievement levels (1-7) for their best 6 subjects, excluding Life
    /// Orientation. Individual universities may use a different scale - see /api/compare for a
    /// course-specific evaluation against a university's own published requirements.
    /// </summary>
    /// <param name="subjects">The learner's subjects and NSC achievement levels (1-7 each).</param>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ApsCalculationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ApsCalculationResult> Calculate([FromBody] List<LearnerSubjectScore> subjects)
    {
        if (subjects is null || subjects.Count == 0)
        {
            return BadRequest("Provide at least one subject with an NSC achievement level (1-7).");
        }

        if (subjects.Any(s => s.Level is < 1 or > 7))
        {
            return BadRequest("NSC achievement levels must be between 1 and 7.");
        }

        return Ok(ApsCalculationService.Calculate(subjects));
    }
}
