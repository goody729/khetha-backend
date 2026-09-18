using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Data;

public static class SouthAfricanUniversities
{
    public static readonly IReadOnlyList<University> All = BuildAll();

    /// <summary>
    /// Finds a course by its globally unique id, regardless of which university it belongs to.
    /// </summary>
    public static (University University, Course Course)? FindCourse(int courseId)
    {
        foreach (var university in All)
        {
            var course = university.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course is not null)
            {
                return (university, course);
            }
        }

        return null;
    }

    private static List<University> BuildAll()
    {
        var universities = new List<University>
    {
        new() { Id = 1, Name = "University of Cape Town", Abbreviation = "UCT", Province = "Western Cape", City = "Cape Town", Type = "Traditional", Website = "https://www.uct.ac.za", Courses = UCTCourses.All.ToList() },
        new() { Id = 2, Name = "University of the Witwatersrand", Abbreviation = "Wits", Province = "Gauteng", City = "Johannesburg", Type = "Traditional", Website = "https://www.wits.ac.za", Courses = WitsCourses.All.ToList() },
        new() { Id = 3, Name = "Stellenbosch University", Abbreviation = "SU", Province = "Western Cape", City = "Stellenbosch", Type = "Traditional", Website = "https://www.sun.ac.za" },
        new() { Id = 4, Name = "University of Pretoria", Abbreviation = "UP", Province = "Gauteng", City = "Pretoria", Type = "Traditional", Website = "https://www.up.ac.za", Courses = UPCourses.All.ToList() },
        new() { Id = 5, Name = "University of KwaZulu-Natal", Abbreviation = "UKZN", Province = "KwaZulu-Natal", City = "Durban", Type = "Traditional", Website = "https://www.ukzn.ac.za", Courses = UKZNCourses.All.ToList() },
        new() { Id = 6, Name = "University of Johannesburg", Abbreviation = "UJ", Province = "Gauteng", City = "Johannesburg", Type = "Comprehensive", Website = "https://www.uj.ac.za", Courses = UJCourses.All.ToList() },
        new() { Id = 7, Name = "University of South Africa", Abbreviation = "UNISA", Province = "Gauteng", City = "Pretoria", Type = "Distance", Website = "https://www.unisa.ac.za" },
        new() { Id = 8, Name = "Rhodes University", Abbreviation = "RU", Province = "Eastern Cape", City = "Makhanda", Type = "Traditional", Website = "https://www.ru.ac.za" },
        new() { Id = 9, Name = "North-West University", Abbreviation = "NWU", Province = "North West", City = "Potchefstroom", Type = "Comprehensive", Website = "https://www.nwu.ac.za" },
        new() { Id = 10, Name = "University of the Free State", Abbreviation = "UFS", Province = "Free State", City = "Bloemfontein", Type = "Traditional", Website = "https://www.ufs.ac.za" },
        new() { Id = 11, Name = "University of the Western Cape", Abbreviation = "UWC", Province = "Western Cape", City = "Cape Town", Type = "Traditional", Website = "https://www.uwc.ac.za" },
        new() { Id = 12, Name = "Nelson Mandela University", Abbreviation = "NMU", Province = "Eastern Cape", City = "Gqeberha", Type = "Comprehensive", Website = "https://www.mandela.ac.za" },
        new() { Id = 13, Name = "University of Limpopo", Abbreviation = "UL", Province = "Limpopo", City = "Polokwane", Type = "Traditional", Website = "https://www.ul.ac.za" },
        new() { Id = 14, Name = "University of Venda", Abbreviation = "Univen", Province = "Limpopo", City = "Thohoyandou", Type = "Comprehensive", Website = "https://www.univen.ac.za" },
        new() { Id = 15, Name = "University of Zululand", Abbreviation = "UNIZULU", Province = "KwaZulu-Natal", City = "KwaDlangezwa", Type = "Comprehensive", Website = "https://www.unizulu.ac.za" },
        new() { Id = 16, Name = "Walter Sisulu University", Abbreviation = "WSU", Province = "Eastern Cape", City = "Mthatha", Type = "Comprehensive", Website = "https://www.wsu.ac.za" },
        new() { Id = 17, Name = "Central University of Technology", Abbreviation = "CUT", Province = "Free State", City = "Bloemfontein", Type = "University of Technology", Website = "https://www.cut.ac.za" },
        new() { Id = 18, Name = "Cape Peninsula University of Technology", Abbreviation = "CPUT", Province = "Western Cape", City = "Cape Town", Type = "University of Technology", Website = "https://www.cput.ac.za" },
        new() { Id = 19, Name = "Durban University of Technology", Abbreviation = "DUT", Province = "KwaZulu-Natal", City = "Durban", Type = "University of Technology", Website = "https://www.dut.ac.za" },
        new() { Id = 20, Name = "Mangosuthu University of Technology", Abbreviation = "MUT", Province = "KwaZulu-Natal", City = "Durban", Type = "University of Technology", Website = "https://www.mut.ac.za" },
        new() { Id = 21, Name = "Tshwane University of Technology", Abbreviation = "TUT", Province = "Gauteng", City = "Pretoria", Type = "University of Technology", Website = "https://www.tut.ac.za" },
        new() { Id = 22, Name = "Vaal University of Technology", Abbreviation = "VUT", Province = "Gauteng", City = "Vanderbijlpark", Type = "University of Technology", Website = "https://www.vut.ac.za" },
        new() { Id = 23, Name = "University of Mpumalanga", Abbreviation = "UMP", Province = "Mpumalanga", City = "Mbombela", Type = "Comprehensive", Website = "https://www.ump.ac.za" },
        new() { Id = 24, Name = "Sol Plaatje University", Abbreviation = "SPU", Province = "Northern Cape", City = "Kimberley", Type = "Traditional", Website = "https://www.spu.ac.za" },
        new() { Id = 25, Name = "Sefako Makgatho Health Sciences University", Abbreviation = "SMU", Province = "Gauteng", City = "Pretoria", Type = "Traditional", Website = "https://www.smu.ac.za" },
        new() { Id = 26, Name = "University of Fort Hare", Abbreviation = "UFH", Province = "Eastern Cape", City = "Alice", Type = "Traditional", Website = "https://www.ufh.ac.za" },
    };

        var nextCourseId = 1;
        foreach (var university in universities)
        {
            foreach (var course in university.Courses)
            {
                course.Id = nextCourseId++;
            }

            if (SouthAfricanCityCoordinates.ByCity.TryGetValue(university.City, out var coordinates))
            {
                university.Latitude = coordinates.Lat;
                university.Longitude = coordinates.Lng;
            }
        }

        return universities;
    }
}
