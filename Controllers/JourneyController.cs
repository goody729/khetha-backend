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
public class JourneyController : ControllerBase
{
    private readonly AppDbContext _db;

    public JourneyController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Saves a career to the signed-in learner's shortlist. Idempotent - saving an already-saved
    /// career returns the existing saved record rather than an error.
    /// </summary>
    /// <param name="careerId">The career id, from the /api/careers directory.</param>
    [HttpPost("careers/{careerId:int}")]
    [ProducesResponseType(typeof(SavedCareerDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SavedCareerDetail>> SaveCareer(int careerId)
    {
        var career = Careers.All.FirstOrDefault(c => c.Id == careerId);
        if (career is null)
        {
            return NotFound();
        }

        var learnerId = GetCurrentLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var existing = await _db.SavedCareers
            .FirstOrDefaultAsync(s => s.LearnerId == learnerId.Value && s.CareerId == careerId);

        if (existing is null)
        {
            existing = new SavedCareer { LearnerId = learnerId.Value, CareerId = careerId };
            _db.SavedCareers.Add(existing);
            await _db.SaveChangesAsync();
        }

        return Ok(new SavedCareerDetail { Id = existing.Id, SavedAtUtc = existing.SavedAtUtc, Career = career });
    }

    /// <summary>
    /// Removes a career from the signed-in learner's shortlist. Idempotent - returns 204 whether or
    /// not it was saved.
    /// </summary>
    /// <param name="careerId">The career id to unsave.</param>
    [HttpDelete("careers/{careerId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnsaveCareer(int careerId)
    {
        var learnerId = GetCurrentLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var existing = await _db.SavedCareers
            .FirstOrDefaultAsync(s => s.LearnerId == learnerId.Value && s.CareerId == careerId);

        if (existing is not null)
        {
            _db.SavedCareers.Remove(existing);
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    /// <summary>
    /// Gets the signed-in learner's saved careers, most recently saved first.
    /// </summary>
    [HttpGet("careers")]
    [ProducesResponseType(typeof(IEnumerable<SavedCareerDetail>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<SavedCareerDetail>>> GetSavedCareers()
    {
        var learnerId = GetCurrentLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var saved = await _db.SavedCareers
            .Where(s => s.LearnerId == learnerId.Value)
            .OrderByDescending(s => s.SavedAtUtc)
            .ToListAsync();

        var careersById = Careers.All.ToDictionary(c => c.Id);

        return Ok(saved
            .Where(s => careersById.ContainsKey(s.CareerId))
            .Select(s => new SavedCareerDetail { Id = s.Id, SavedAtUtc = s.SavedAtUtc, Career = careersById[s.CareerId] }));
    }

    /// <summary>
    /// Gets the signed-in learner's roadmap progress across the 5 fixed journey steps, plus the
    /// computed current (first incomplete) step. Auto-creates an all-incomplete record on first access.
    /// </summary>
    [HttpGet("progress")]
    [ProducesResponseType(typeof(JourneyProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<JourneyProgressResponse>> GetProgress()
    {
        var learnerId = GetCurrentLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var progress = await GetOrCreateProgressAsync(learnerId.Value);
        return Ok(JourneyProgressResponse.FromProgress(progress));
    }

    /// <summary>
    /// Marks a journey step as complete for the signed-in learner. Idempotent - completing an
    /// already-completed step leaves its original completion timestamp unchanged. Completing a later
    /// step does not auto-complete earlier ones.
    /// </summary>
    /// <param name="step">The step to mark complete.</param>
    [HttpPost("progress/{step}/complete")]
    [ProducesResponseType(typeof(JourneyProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<JourneyProgressResponse>> CompleteStep(JourneyStep step)
    {
        var learnerId = GetCurrentLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var progress = await GetOrCreateProgressAsync(learnerId.Value);

        var now = DateTime.UtcNow;
        switch (step)
        {
            case JourneyStep.Explore:
                progress.ExploreCompletedAtUtc ??= now;
                break;
            case JourneyStep.Assess:
                progress.AssessCompletedAtUtc ??= now;
                break;
            case JourneyStep.Shortlist:
                progress.ShortlistCompletedAtUtc ??= now;
                break;
            case JourneyStep.Apply:
                progress.ApplyCompletedAtUtc ??= now;
                break;
            case JourneyStep.Enroll:
                progress.EnrollCompletedAtUtc ??= now;
                break;
        }

        await _db.SaveChangesAsync();
        return Ok(JourneyProgressResponse.FromProgress(progress));
    }

    private async Task<JourneyProgress> GetOrCreateProgressAsync(Guid learnerId)
    {
        var progress = await _db.JourneyProgresses.FirstOrDefaultAsync(j => j.LearnerId == learnerId);
        if (progress is null)
        {
            progress = new JourneyProgress { LearnerId = learnerId };
            _db.JourneyProgresses.Add(progress);
            await _db.SaveChangesAsync();
        }

        return progress;
    }

    private Guid? GetCurrentLearnerId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
