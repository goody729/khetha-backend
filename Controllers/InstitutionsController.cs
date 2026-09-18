using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

/// <summary>
/// A richer view over the same university catalog as /api/universities: adds distance-from-caller,
/// prospectus-link fallback, and bulk APS-based course matching. /api/universities remains available
/// as a simple, unchanged browse surface over the same underlying data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InstitutionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public InstitutionsController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all institutions, optionally filtered by province, sorted by distance ascending when a
    /// location is available (from <paramref name="lat"/>/<paramref name="lng"/>, or failing that the
    /// signed-in learner's saved profile location).
    /// </summary>
    /// <param name="province">Optional province name to filter by.</param>
    /// <param name="lat">Optional latitude to measure distance from (takes precedence over the saved profile location).</param>
    /// <param name="lng">Optional longitude to measure distance from.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InstitutionSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InstitutionSummary>>> GetAll(
        [FromQuery] string? province, [FromQuery] double? lat, [FromQuery] double? lng)
    {
        var universities = SouthAfricanUniversities.All.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(province))
        {
            universities = universities.Where(u =>
                string.Equals(u.Province, province, StringComparison.OrdinalIgnoreCase));
        }

        var location = await ResolveLocationAsync(lat, lng);

        var summaries = universities
            .Select(u => InstitutionSummary.FromUniversity(u, DistanceTo(u, location)))
            .ToList();

        if (location is not null)
        {
            summaries = summaries.OrderBy(s => s.DistanceKm).ToList();
        }

        return Ok(summaries);
    }

    /// <summary>
    /// Gets a single institution by its id.
    /// </summary>
    /// <param name="id">The institution (university) id.</param>
    /// <param name="lat">Optional latitude to measure distance from (takes precedence over the saved profile location).</param>
    /// <param name="lng">Optional longitude to measure distance from.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InstitutionSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstitutionSummary>> GetById(int id, [FromQuery] double? lat, [FromQuery] double? lng)
    {
        var university = SouthAfricanUniversities.All.FirstOrDefault(u => u.Id == id);
        if (university is null)
        {
            return NotFound();
        }

        var location = await ResolveLocationAsync(lat, lng);
        return Ok(InstitutionSummary.FromUniversity(university, DistanceTo(university, location)));
    }

    /// <summary>
    /// Finds which of a single institution's courses a learner's subjects/levels qualify them for,
    /// by applying the existing course-comparison logic (see /api/compare) to every course at that
    /// institution.
    /// </summary>
    /// <param name="id">The institution (university) id.</param>
    /// <param name="request">The learner's subjects and NSC achievement levels.</param>
    [HttpPost("{id:int}/matching-courses")]
    [ProducesResponseType(typeof(MatchingCoursesResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<MatchingCoursesResult> MatchingCourses(int id, [FromBody] MatchingCoursesRequest request)
    {
        var university = SouthAfricanUniversities.All.FirstOrDefault(u => u.Id == id);
        if (university is null)
        {
            return NotFound();
        }

        if (request?.Subjects is null || request.Subjects.Count == 0)
        {
            return BadRequest("Provide at least one subject with an NSC achievement level (1-7).");
        }

        return Ok(BuildMatchingCoursesResult(university, request.Subjects));
    }

    /// <summary>
    /// Finds which courses across every institution a learner's subjects/levels qualify them for.
    /// </summary>
    /// <param name="request">The learner's subjects and NSC achievement levels.</param>
    [HttpPost("matching-courses")]
    [ProducesResponseType(typeof(IEnumerable<MatchingCoursesResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<MatchingCoursesResult>> MatchingCoursesAll([FromBody] MatchingCoursesRequest request)
    {
        if (request?.Subjects is null || request.Subjects.Count == 0)
        {
            return BadRequest("Provide at least one subject with an NSC achievement level (1-7).");
        }

        var results = SouthAfricanUniversities.All
            .Select(u => BuildMatchingCoursesResult(u, request.Subjects))
            .ToList();

        return Ok(results);
    }

    private static MatchingCoursesResult BuildMatchingCoursesResult(University university, List<LearnerSubjectScore> subjects)
    {
        var comparisons = university.Courses
            .Select(course => CourseComparisonService.Compare(course.Id, subjects))
            .Where(result => result is not null)
            .Select(result => result!)
            .ToList();

        return new MatchingCoursesResult
        {
            UniversityId = university.Id,
            University = university.Name,
            MatchingCourses = comparisons.Where(c => c.MeetsAllSubjectRequirements).ToList(),
            NonMatchingCourses = comparisons.Where(c => !c.MeetsAllSubjectRequirements).ToList()
        };
    }

    private static double? DistanceTo(University university, (double Lat, double Lng)? location)
    {
        if (location is null || university.Latitude is null || university.Longitude is null)
        {
            return null;
        }

        return GeoDistance.CalculateKm(location.Value.Lat, location.Value.Lng, university.Latitude.Value, university.Longitude.Value);
    }

    private async Task<(double Lat, double Lng)?> ResolveLocationAsync(double? lat, double? lng)
    {
        if (lat is not null && lng is not null)
        {
            return (lat.Value, lng.Value);
        }

        if (User.Identity?.IsAuthenticated == true &&
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var learnerId))
        {
            var learner = await _db.Learners.FirstOrDefaultAsync(l => l.Id == learnerId);
            if (learner?.Latitude is not null && learner.Longitude is not null)
            {
                return (learner.Latitude.Value, learner.Longitude.Value);
            }
        }

        return null;
    }
}
