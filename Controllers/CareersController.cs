using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CareersController : ControllerBase
{
    /// <summary>
    /// Finds courses a learner's subject combination could unlock, grouped by faculty as an
    /// approximation of the career fields it opens up. Matching is by subject presence only - NSC
    /// achievement levels are not checked (see /api/compare for a level-aware evaluation of a single
    /// course).
    /// </summary>
    /// <param name="request">The learner's subject names (no levels required).</param>
    [HttpPost("unlocked")]
    [ProducesResponseType(typeof(CareersUnlockedResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CareersUnlockedResult> Unlocked([FromBody] CareersUnlockedRequest request)
    {
        if (request?.Subjects is null || request.Subjects.Count == 0)
        {
            return BadRequest("Provide at least one subject name.");
        }

        return Ok(CareerMatchingService.FindUnlockedCourses(request.Subjects));
    }
}
