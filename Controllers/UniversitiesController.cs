using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UniversitiesController : ControllerBase
{
    /// <summary>
    /// Gets all public universities in South Africa, optionally filtered by province.
    /// </summary>
    /// <param name="province">Optional province name to filter by (e.g. "Gauteng").</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<University>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<University>> GetAll([FromQuery] string? province)
    {
        var universities = SouthAfricanUniversities.All.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(province))
        {
            universities = universities.Where(u =>
                string.Equals(u.Province, province, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(universities);
    }

    /// <summary>
    /// Gets a single university by its id.
    /// </summary>
    /// <param name="id">The university id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(University), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<University> GetById(int id)
    {
        var university = SouthAfricanUniversities.All.FirstOrDefault(u => u.Id == id);
        return university is null ? NotFound() : Ok(university);
    }
}
