using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

/// <summary>
/// The South African school year (four terms) and the report-card reminder dates, as JSON for the
/// app and as an .ics feed that can be added to the learner's own calendar.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    /// <summary>
    /// Gets the school year: term open/close dates, the holidays between terms, and the dates the
    /// app reminds learners to update their report card.
    /// </summary>
    /// <param name="year">Calendar year, e.g. 2026.</param>
    [HttpGet("{year:int}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CalendarResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CalendarResponse> GetYear(int year)
    {
        var terms = SchoolCalendar.ForYear(year);
        if (terms is null)
        {
            return NotFound($"No school calendar is loaded for {year}. Available: {string.Join(", ", SchoolCalendar.SupportedYears)}.");
        }

        return Ok(new CalendarResponse
        {
            Year = year,
            Terms = terms.Select(t => new CalendarTerm { Term = t.Number, Opens = t.Opens, Closes = t.Closes }).ToList(),
            Holidays = terms.Zip(terms.Skip(1), (a, b) => new CalendarHoliday
            {
                AfterTerm = a.Number,
                From = a.Closes.AddDays(1),
                To = b.Opens.AddDays(-1)
            }).ToList(),
            ReportCardReminders = ReportCardReminderPlanner.ForYear(year)
        });
    }

    /// <summary>
    /// Downloads the school year as an iCalendar (.ics) file - term start/end days and report-card
    /// reminders (with an 08:00 alarm) - for import into Google, Apple or Outlook calendars.
    /// </summary>
    /// <param name="year">Calendar year, e.g. 2026.</param>
    [HttpGet("{year:int}/ics")]
    [Produces("text/calendar")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetIcs(int year)
    {
        var terms = SchoolCalendar.ForYear(year);
        if (terms is null)
        {
            return NotFound($"No school calendar is loaded for {year}. Available: {string.Join(", ", SchoolCalendar.SupportedYears)}.");
        }

        return File(System.Text.Encoding.UTF8.GetBytes(CalendarIcsBuilder.Build(year, terms)),
            "text/calendar; charset=utf-8", $"khetha-school-calendar-{year}.ics");
    }
}
