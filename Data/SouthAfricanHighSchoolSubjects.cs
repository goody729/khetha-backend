using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Data;

/// <summary>
/// Official National Senior Certificate (NSC / "matric") subjects offered in South African high schools,
/// per the Department of Basic Education's CAPS subject list. "IsDesignated" marks a subject as one of the
/// higher-education "designated subject list" entries that universities count toward admission point scores
/// alongside English and Mathematics/Mathematical Literacy. Every candidate takes one Home Language and one
/// First Additional Language as compulsory subjects; a language taken beyond that pair counts as designated.
/// </summary>
public static class SouthAfricanHighSchoolSubjects
{
    private static readonly string[] OfficialLanguages =
    {
        "Afrikaans", "English", "isiNdebele", "isiXhosa", "isiZulu",
        "Sepedi", "Sesotho", "Setswana", "siSwati", "Tshivenda", "Xitsonga"
    };

    private static Subject S(string name, string category, bool designated, bool compulsory, string notes)
        => new() { Name = name, Category = category, IsDesignated = designated, IsCompulsory = compulsory, Notes = notes };

    public static readonly IReadOnlyList<Subject> All = BuildAll();

    private static List<Subject> BuildAll()
    {
        var subjects = new List<Subject>();

        foreach (var language in OfficialLanguages)
        {
            subjects.Add(S($"{language} Home Language", "Language", false, false,
                "One Home Language is compulsory for every NSC candidate. A language taken beyond a candidate's compulsory Home Language and First Additional Language pair counts as a designated elective."));
            subjects.Add(S($"{language} First Additional Language", "Language", false, false,
                "One First Additional Language is compulsory for every NSC candidate. A language taken beyond a candidate's compulsory Home Language and First Additional Language pair counts as a designated elective."));
        }

        var id = 1;
        foreach (var subject in new[]
        {
            S("Mathematics", "Mathematics", true, false, ""),
            S("Mathematical Literacy", "Mathematics", true, false, "Cannot be substituted for Mathematics in most quantitative programmes."),
            S("Technical Mathematics", "Mathematics", false, false, "Technical-school equivalent of Mathematics, commonly accepted in its place for technical/vocational-stream programmes at Universities of Technology."),
            S("Physical Sciences", "Science", true, false, ""),
            S("Life Sciences", "Science", true, false, ""),
            S("Technical Sciences", "Science", false, false, "Technical-school equivalent of Physical Sciences, commonly accepted in its place for technical/vocational-stream programmes at Universities of Technology."),
            S("Life Orientation", "Life Orientation", false, true, "Compulsory NSC subject; excluded from most university Admission Point Score (APS) calculations."),
            S("Accounting", "Commerce", true, false, ""),
            S("Business Studies", "Commerce", true, false, ""),
            S("Economics", "Commerce", true, false, ""),
            S("Maritime Economics", "Commerce", true, false, ""),
            S("Geography", "Humanities", true, false, ""),
            S("History", "Humanities", true, false, ""),
            S("Religion Studies", "Humanities", true, false, ""),
            S("Consumer Studies", "Services", true, false, ""),
            S("Tourism", "Services", true, false, ""),
            S("Hospitality Studies", "Services", true, false, ""),
            S("Agricultural Sciences", "Agriculture", true, false, ""),
            S("Agricultural Management Practices", "Agriculture", true, false, ""),
            S("Agricultural Technology", "Agriculture", true, false, ""),
            S("Equine Studies", "Agriculture", true, false, ""),
            S("Civil Technology", "Technology", true, false, ""),
            S("Electrical Technology", "Technology", true, false, ""),
            S("Mechanical Technology", "Technology", true, false, ""),
            S("Engineering Graphics and Design", "Technology", true, false, ""),
            S("Computer Applications Technology", "Technology", true, false, ""),
            S("Information Technology", "Technology", true, false, ""),
            S("Nautical Science", "Technology", true, false, ""),
            S("Dramatic Arts", "Arts", true, false, ""),
            S("Dance Studies", "Arts", true, false, ""),
            S("Design", "Arts", true, false, ""),
            S("Music", "Arts", true, false, ""),
            S("Visual Arts", "Arts", true, false, ""),
            S("Sport and Exercise Science", "Sport", true, false, ""),
        })
        {
            subjects.Add(subject);
        }

        foreach (var subject in subjects)
        {
            subject.Id = id++;
        }

        return subjects;
    }
}
