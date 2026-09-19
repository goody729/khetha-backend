using System.Text;
using TertiaryInstitutions.Data;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Builds an iCalendar (.ics) feed of a school year - term open/close days plus the report-card
/// reminder dates - that Google/Apple/Outlook calendars can import or subscribe to.
/// </summary>
public static class CalendarIcsBuilder
{
    public static string Build(int year, IReadOnlyList<SchoolTerm> terms)
    {
        var sb = new StringBuilder();
        Line(sb, "BEGIN:VCALENDAR");
        Line(sb, "VERSION:2.0");
        Line(sb, "PRODID:-//Khetha//School Calendar//EN");
        Line(sb, "CALSCALE:GREGORIAN");
        Line(sb, "METHOD:PUBLISH");
        Line(sb, $"X-WR-CALNAME:Khetha school calendar {year}");
        Line(sb, "X-WR-TIMEZONE:Africa/Johannesburg");

        var stamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");

        foreach (var term in terms)
        {
            AllDayEvent(sb, stamp, $"term-{year}-{term.Number}-open", term.Opens, $"Term {term.Number} starts");
            AllDayEvent(sb, stamp, $"term-{year}-{term.Number}-close", term.Closes, $"Term {term.Number} ends - school holidays begin");
        }

        foreach (var reminder in ReportCardReminderPlanner.ForYear(year))
        {
            AllDayEvent(sb, stamp, $"reminder-{reminder.Date:yyyyMMdd}", reminder.Date,
                $"{reminder.Title} (Term {reminder.ForTerm} marks)", reminder.Message, alarm: true);
        }

        Line(sb, "END:VCALENDAR");
        return sb.ToString();
    }

    private static void AllDayEvent(StringBuilder sb, string stamp, string uid, DateOnly date,
        string summary, string? description = null, bool alarm = false)
    {
        Line(sb, "BEGIN:VEVENT");
        Line(sb, $"UID:{uid}@khetha");
        Line(sb, $"DTSTAMP:{stamp}");
        Line(sb, $"DTSTART;VALUE=DATE:{date:yyyyMMdd}");
        Line(sb, $"DTEND;VALUE=DATE:{date.AddDays(1):yyyyMMdd}");
        Line(sb, $"SUMMARY:{Escape(summary)}");
        if (description is not null)
        {
            Line(sb, $"DESCRIPTION:{Escape(description)}");
        }
        if (alarm)
        {
            Line(sb, "BEGIN:VALARM");
            Line(sb, "ACTION:DISPLAY");
            Line(sb, $"DESCRIPTION:{Escape(summary)}");
            Line(sb, "TRIGGER:PT8H");
            Line(sb, "END:VALARM");
        }
        Line(sb, "END:VEVENT");
    }

    private static void Line(StringBuilder sb, string text) => sb.Append(text).Append("\r\n");

    private static string Escape(string text) =>
        text.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,").Replace("\n", "\\n");
}
