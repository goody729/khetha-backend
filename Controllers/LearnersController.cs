using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class LearnersController : ControllerBase
{
    private readonly AppDbContext _db;

    public LearnersController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets the signed-in learner's profile.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(LearnerProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LearnerProfileResponse>> GetMe()
    {
        var learner = await GetCurrentLearnerAsync();
        return learner is null ? Unauthorized() : Ok(LearnerProfileResponse.FromLearner(learner));
    }

    /// <summary>
    /// Updates the signed-in learner's profile (grade, language and track).
    /// </summary>
    /// <param name="request">The updated profile fields.</param>
    [HttpPut("me")]
    [ProducesResponseType(typeof(LearnerProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LearnerProfileResponse>> UpdateMe([FromBody] UpdateLearnerProfileRequest request)
    {
        var learner = await GetCurrentLearnerAsync();
        if (learner is null)
        {
            return Unauthorized();
        }

        learner.Name = request.Name;
        learner.Grade = request.Grade;
        learner.Language = request.Language;
        learner.Track = request.Track;
        learner.Latitude = request.Latitude;
        learner.Longitude = request.Longitude;
        await _db.SaveChangesAsync();

        return Ok(LearnerProfileResponse.FromLearner(learner));
    }

    /// <summary>
    /// Gets the signed-in learner's past job-fit assessment results, most recent first.
    /// </summary>
    [HttpGet("me/assessments")]
    [ProducesResponseType(typeof(IEnumerable<AssessmentSubmissionSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AssessmentSubmissionSummary>>> GetMyAssessments()
    {
        var learnerId = GetCurrentLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var submissions = await _db.AssessmentSubmissions
            .Where(a => a.LearnerId == learnerId.Value)
            .OrderByDescending(a => a.SubmittedAtUtc)
            .ToListAsync();

        return Ok(submissions.Select(AssessmentSubmissionSummary.FromSubmission));
    }

    private Guid? GetCurrentLearnerId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    private async Task<Learner?> GetCurrentLearnerAsync()
    {
        var learnerId = GetCurrentLearnerId();
        return learnerId is null ? null : await _db.Learners.FirstOrDefaultAsync(l => l.Id == learnerId.Value);
    }
}
