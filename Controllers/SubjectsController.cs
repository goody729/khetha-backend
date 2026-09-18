using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubjectsController : ControllerBase
{
    /// <summary>
    /// Gets all official South African NSC (matric) high school subjects, optionally filtered by category
    /// or designated-subject status.
    /// </summary>
    /// <param name="category">Optional category to filter by (e.g. "Science", "Language", "Commerce").</param>
    /// <param name="designated">Optional filter for whether the subject is on the designated subject list.</param>
    /// <param name="elective">
    /// Optional filter for elective status. There is no dedicated elective-grouping data in this catalog, so
    /// this is interpreted as the inverse of <see cref="Subject.IsCompulsory"/>: <c>elective=true</c> returns
    /// subjects that are not compulsory.
    /// </param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Subject>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Subject>> GetAll(
        [FromQuery] string? category, [FromQuery] bool? designated, [FromQuery] bool? elective)
    {
        var subjects = SouthAfricanHighSchoolSubjects.All.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            subjects = subjects.Where(s =>
                string.Equals(s.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        if (designated.HasValue)
        {
            subjects = subjects.Where(s => s.IsDesignated == designated.Value);
        }

        if (elective.HasValue)
        {
            subjects = subjects.Where(s => s.IsCompulsory != elective.Value);
        }

        return Ok(subjects);
    }

    /// <summary>
    /// Gets a single subject by its id.
    /// </summary>
    /// <param name="id">The subject id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Subject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Subject> GetById(int id)
    {
        var subject = SouthAfricanHighSchoolSubjects.All.FirstOrDefault(s => s.Id == id);
        return subject is null ? NotFound() : Ok(subject);
    }
}
