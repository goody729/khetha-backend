using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Works out when learners should be reminded to enter their report-card marks. Schools close for
/// the holidays after each term; a learner's marks for the term that just ended are due when schools
/// reopen, and a reminder repeats every month after reopening until the marks are entered.
/// Term 1 asks for the previous year's Term 4 marks.
/// </summary>
public static class ReportCardReminderPlanner
{
    /// <summary>South Africa is UTC+2 all year (no daylight saving).</summary>
    public static DateTime NowSast() => DateTime.UtcNow.AddHours(2);

    /// <summary>All reminder dates in a year: the reopening day, then every month until the term closes.</summary>
    public static List<CalendarReminder> ForYear(int year)
    {
        var reminders = new List<CalendarReminder>();
        var terms = SchoolCalendar.ForYear(year);
        if (terms is null)
        {
            return reminders;
        }

        foreach (var term in terms)
        {
            var (forYear, forTerm) = term.Number == 1 ? (year - 1, 4) : (year, term.Number - 1);

            for (var months = 0; ; months++)
            {
                var date = term.Opens.AddMonths(months);
                if (date > term.Closes)
                {
                    break;
                }

                reminders.Add(new CalendarReminder
                {
                    Date = date,
                    ForYear = forYear,
                    ForTerm = forTerm,
                    Title = "Update your report card",
                    Message = $"Enter your Term {forTerm} marks so Khetha can check you're on track for your career goals."
                });
            }
        }

        return reminders;
    }

    public static List<CalendarReminder> DueOn(DateOnly date) => ForYear(date.Year).Where(r => r.Date == date).ToList();

    /// <summary>Report-card terms whose marks are already due (their reopening day has passed).</summary>
    public static List<(int Year, int Term)> TermsDueBy(DateOnly today) =>
        ForYear(today.Year).Where(r => r.Date <= today).Select(r => (r.ForYear, r.ForTerm)).Distinct().ToList();

    public static DateOnly? NextReminderAfter(DateOnly today) =>
        ForYear(today.Year).Concat(ForYear(today.Year + 1))
            .Where(r => r.Date > today)
            .Select(r => (DateOnly?)r.Date)
            .OrderBy(d => d)
            .FirstOrDefault();
}
