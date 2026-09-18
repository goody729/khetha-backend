using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CareersController : ControllerBase
{
    private readonly AppDbContext _db;

    public CareersController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets the career directory, optionally filtered by RIASEC (Holland Code) type and/or a keyword
    /// matched against the title and summary.
    /// </summary>
    /// <param name="riasecType">Optional RIASEC type to filter by (careers tagged with this type).</param>
    /// <param name="keyword">Optional case-insensitive keyword to match against title/summary.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Career>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Career>> GetAll([FromQuery] RiasecType? riasecType, [FromQuery] string? keyword)
    {
        var careers = Careers.All.AsEnumerable();

        if (riasecType.HasValue)
        {
            careers = careers.Where(c => c.RiasecTags.Contains(riasecType.Value));
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            careers = careers.Where(c =>
                c.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                c.Summary.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(careers);
    }

    /// <summary>
    /// Gets a single career by its id.
    /// </summary>
    /// <param name="id">The career id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Career), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Career> GetById(int id)
    {
        var career = Careers.All.FirstOrDefault(c => c.Id == id);
        return career is null ? NotFound() : Ok(career);
    }

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

    /// <summary>
    /// Ranks the career directory against a learner's subjects and/or (when signed in) their most
    /// recent job-fit quiz result. At least one of subjects-in-body or a signed-in caller with
    /// assessment history is required.
    /// </summary>
    /// <param name="request">Optionally, the learner's subjects and NSC achievement levels.</param>
    [HttpPost("match")]
    [ProducesResponseType(typeof(CareerMatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CareerMatchResponse>> Match([FromBody] CareerMatchRequest? request)
    {
        AssessmentSubmission? latestAssessment = null;
        if (User.Identity?.IsAuthenticated == true &&
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var learnerId))
        {
            latestAssessment = await _db.AssessmentSubmissions
                .Where(a => a.LearnerId == learnerId)
                .OrderByDescending(a => a.SubmittedAtUtc)
                .FirstOrDefaultAsync();
        }

        var hasSubjects = request?.Subjects is { Count: > 0 };
        if (!hasSubjects && latestAssessment is null)
        {
            return BadRequest("Provide subjects, or sign in with assessment history, to get a match score.");
        }

        return Ok(CareerMatchingScoreService.MatchAll(request?.Subjects, latestAssessment));
    }
}
