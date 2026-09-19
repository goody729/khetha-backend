namespace TertiaryInstitutions.Data;

/// <summary>One school term: the day schools open and the day they close for the holidays.</summary>
public record SchoolTerm(int Number, DateOnly Opens, DateOnly Closes);

/// <summary>
/// South African public-school year calendar (four terms). Dates are transcribed by hand from the
/// Department of Basic Education's published calendar and MUST be verified against the official
/// DBE calendar before release; add a new entry each year.
/// </summary>
public static class SchoolCalendar
{
    private static readonly IReadOnlyDictionary<int, IReadOnlyList<SchoolTerm>> Years =
        new Dictionary<int, IReadOnlyList<SchoolTerm>>
        {
            [2026] = new[]
            {
                new SchoolTerm(1, new DateOnly(2026, 1, 14), new DateOnly(2026, 3, 27)),
                new SchoolTerm(2, new DateOnly(2026, 4, 8), new DateOnly(2026, 6, 26)),
                new SchoolTerm(3, new DateOnly(2026, 7, 14), new DateOnly(2026, 9, 25)),
                new SchoolTerm(4, new DateOnly(2026, 10, 6), new DateOnly(2026, 12, 9)),
            }
        };

    public static IReadOnlyCollection<int> SupportedYears => Years.Keys.ToList();

    public static IReadOnlyList<SchoolTerm>? ForYear(int year) =>
        Years.TryGetValue(year, out var terms) ? terms : null;
}
