using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AssessmentController : ControllerBase
{
    private readonly AppDbContext _db;

    public AssessmentController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets the full job-fit quiz question bank. No sign-in required.
    /// </summary>
    [HttpGet("questions")]
    [ProducesResponseType(typeof(IEnumerable<HollandCodeQuestion>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<HollandCodeQuestion>> GetQuestions()
    {
        return Ok(HollandCodeQuestions.All);
    }

    /// <summary>
    /// Submits job-fit quiz answers and computes a Holland Code (RIASEC) result. Sign-in is optional:
    /// if the caller supplies a valid bearer token the result is saved to their profile and
    /// <c>Persisted</c> is true; otherwise the result is returned without being saved.
    /// </summary>
    /// <param name="request">The learner's answers (question id + 1-5 agreement rating).</param>
    [HttpPost("submit")]
    [ProducesResponseType(typeof(AssessmentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AssessmentResult>> Submit([FromBody] AssessmentSubmitRequest request)
    {
        if (request?.Answers is null || request.Answers.Count == 0)
        {
            return BadRequest("Provide at least one answer.");
        }

        var (valid, error) = RiasecScoringService.Validate(request.Answers);
        if (!valid)
        {
            return BadRequest(error);
        }

        var result = RiasecScoringService.Score(request.Answers);

        if (User.Identity?.IsAuthenticated == true &&
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var learnerId))
        {
            _db.AssessmentSubmissions.Add(new AssessmentSubmission
            {
                LearnerId = learnerId,
                RealisticScore = result.RealisticScore,
                InvestigativeScore = result.InvestigativeScore,
                ArtisticScore = result.ArtisticScore,
                SocialScore = result.SocialScore,
                EnterprisingScore = result.EnterprisingScore,
                ConventionalScore = result.ConventionalScore,
                ResultCode = result.ResultCode
            });
            await _db.SaveChangesAsync();
            result.Persisted = true;
        }

        return Ok(result);
    }
}
