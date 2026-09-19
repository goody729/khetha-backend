using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

/// <summary>
/// Term marks for learners still in school: enter report-card results, then see whether they are on
/// track for their career goals (optionally with AI coaching).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class TermResultsController(
    AppDbContext db, ProgressCoachService coach, ILogger<TermResultsController> logger) : ControllerBase
{
    /// <summary>
    /// Enters (or corrects) the signed-in learner's report-card marks for one term. Re-submitting a
    /// subject for the same year and term overwrites its mark, so it is safe to retry.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TermResultsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TermResultsResponse>> Submit([FromBody] SubmitTermResultsRequest request, CancellationToken ct)
    {
        var learnerId = GetLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var marks = request.Subjects
            .Select(s => (Subject: s.Subject.Trim(), s.Percentage))
            .ToList();
        if (marks.Any(m => m.Subject.Length == 0))
        {
            return BadRequest("Subject names cannot be blank.");
        }
        if (marks.GroupBy(m => m.Subject, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1))
        {
            return BadRequest("Each subject can only appear once per submission.");
        }

        var existing = await db.TermResults
            .Where(r => r.LearnerId == learnerId.Value && r.Year == request.Year && r.Term == request.Term)
            .ToListAsync(ct);

        foreach (var (subject, percentage) in marks)
        {
            var row = existing.FirstOrDefault(r => r.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));
            if (row is null)
            {
                db.TermResults.Add(new TermResult
                {
                    LearnerId = learnerId.Value, Year = request.Year, Term = request.Term,
                    Subject = subject, Percentage = percentage
                });
            }
            else
            {
                row.Percentage = percentage;
                row.RecordedAtUtc = DateTime.UtcNow;
            }
        }

        await db.SaveChangesAsync(ct);

        return Ok(await LoadTermAsync(learnerId.Value, request.Year, request.Term, ct));
    }

    /// <summary>
    /// Gets the signed-in learner's entered marks, grouped by year and term (most recent first).
    /// </summary>
    /// <param name="year">Optional: only this year.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TermResultsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<TermResultsResponse>>> List([FromQuery] int? year, CancellationToken ct)
    {
        var learnerId = GetLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var query = db.TermResults.Where(r => r.LearnerId == learnerId.Value);
        if (year is not null)
        {
            query = query.Where(r => r.Year == year.Value);
        }

        var rows = await query.OrderByDescending(r => r.Year).ThenByDescending(r => r.Term).ThenBy(r => r.Subject).ToListAsync(ct);

        return Ok(rows.GroupBy(r => (r.Year, r.Term)).Select(g => ToResponse(g.Key.Year, g.Key.Term, g)));
    }

    /// <summary>
    /// Checks whether the learner's latest marks are on track for their career goals: the careers
    /// they saved to their journey, or just <paramref name="careerId"/> when given. Includes each
    /// subject's term-on-term trend and, per career, which required subjects are met or how many
    /// NSC levels short they are.
    /// </summary>
    /// <param name="careerId">Optional career id from /api/careers; defaults to all saved careers.</param>
    [HttpGet("progress")]
    [ProducesResponseType(typeof(TermProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TermProgressResponse>> Progress([FromQuery] int? careerId, CancellationToken ct)
    {
        var learnerId = GetLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var (progress, error) = await BuildProgressAsync(learnerId.Value, careerId, ct);
        return error is not null ? BadRequest(error) : Ok(progress);
    }

    /// <summary>
    /// Same as <c>GET progress</c>, plus AI coaching that explains the results and suggests what to
    /// focus on. If the AI service is unavailable the progress is still returned and
    /// <c>coaching</c> is null.
    /// </summary>
    /// <param name="careerId">Optional career id from /api/careers; defaults to all saved careers.</param>
    [HttpPost("coach")]
    [ProducesResponseType(typeof(CoachResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CoachResponse>> Coach([FromQuery] int? careerId, CancellationToken ct)
    {
        var learnerId = GetLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var (progress, error) = await BuildProgressAsync(learnerId.Value, careerId, ct);
        if (error is not null)
        {
            return BadRequest(error);
        }

        if (progress.Subjects.Count == 0)
        {
            return Ok(new CoachResponse
            {
                Progress = progress,
                Notice = "Enter your term marks first (POST /api/termresults) so your coach has something to work with."
            });
        }

        try
        {
            var coaching = await coach.CoachAsync(progress, ct);
            return Ok(new CoachResponse
            {
                Progress = progress,
                Coaching = coaching,
                Notice = coaching is null ? "AI coaching is not available right now; your progress is shown without it." : null
            });
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            logger.LogWarning(ex, "AI coaching failed; returning progress without it.");
            return Ok(new CoachResponse
            {
                Progress = progress,
                Notice = "AI coaching is not available right now; your progress is shown without it."
            });
        }
    }

    /// <summary>
    /// Which terms' marks the learner should have entered by now but hasn't, and when the next
    /// report-card reminder is due. Lets the app show an in-app prompt alongside the push notification.
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(ReportCardStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReportCardStatusResponse>> Status(CancellationToken ct)
    {
        var learnerId = GetLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var today = DateOnly.FromDateTime(ReportCardReminderPlanner.NowSast());
        var due = ReportCardReminderPlanner.TermsDueBy(today);

        var entered = await db.TermResults
            .Where(r => r.LearnerId == learnerId.Value)
            .Select(r => new { r.Year, r.Term })
            .Distinct()
            .ToListAsync(ct);

        return Ok(new ReportCardStatusResponse
        {
            MissingTerms = due
                .Where(d => !entered.Any(e => e.Year == d.Year && e.Term == d.Term))
                .OrderBy(d => d.Year).ThenBy(d => d.Term)
                .Select(d => new MissingTerm { Year = d.Year, Term = d.Term })
                .ToList(),
            NextReminderOn = ReportCardReminderPlanner.NextReminderAfter(today)
        });
    }

    private async Task<(TermProgressResponse Progress, string? Error)> BuildProgressAsync(
        Guid learnerId, int? careerId, CancellationToken ct)
    {
        List<Career> goals;
        if (careerId is not null)
        {
            var career = Careers.All.FirstOrDefault(c => c.Id == careerId.Value);
            if (career is null)
            {
                return (new TermProgressResponse(), $"Career {careerId} was not found in /api/careers.");
            }
            goals = new List<Career> { career };
        }
        else
        {
            var savedIds = await db.SavedCareers
                .Where(s => s.LearnerId == learnerId)
                .Select(s => s.CareerId)
                .ToListAsync(ct);
            goals = Careers.All.Where(c => savedIds.Contains(c.Id)).ToList();
            if (goals.Count == 0)
            {
                return (new TermProgressResponse(),
                    "Pick a career goal first: save a career with POST /api/journey/careers/{careerId}, or pass ?careerId=.");
            }
        }

        var results = await db.TermResults.Where(r => r.LearnerId == learnerId).ToListAsync(ct);
        return (TermProgressService.Evaluate(results, goals), null);
    }

    private async Task<TermResultsResponse> LoadTermAsync(Guid learnerId, int year, int term, CancellationToken ct)
    {
        var rows = await db.TermResults
            .Where(r => r.LearnerId == learnerId && r.Year == year && r.Term == term)
            .OrderBy(r => r.Subject)
            .ToListAsync(ct);
        return ToResponse(year, term, rows);
    }

    private static TermResultsResponse ToResponse(int year, int term, IEnumerable<TermResult> rows) => new()
    {
        Year = year,
        Term = term,
        Subjects = rows.OrderBy(r => r.Subject).Select(r => new TermSubjectResult
        {
            Subject = r.Subject,
            Percentage = r.Percentage,
            Level = TermProgressService.PercentageToLevel(r.Percentage)
        }).ToList()
    };

    private Guid? GetLearnerId() =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
