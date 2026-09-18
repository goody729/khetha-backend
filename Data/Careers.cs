using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Data;

/// <summary>
/// Starter catalog of common South African careers spanning a range of RIASEC types and fields, for
/// the /api/careers directory and match-scoring endpoints.
///
/// OfoCode values are FABRICATED PLACEHOLDERS for structural/demo purposes only - they are not
/// verified against the real South African Organising Framework for Occupations (OFO) and must
/// not be treated as authoritative or used in production without verification.
/// </summary>
public static class Careers
{
    public static readonly IReadOnlyList<Career> All = BuildAll();

    private static List<Career> BuildAll() => new()
    {
        C(1, "Software Engineer", "OFO-DEMO-2513", "Designs, builds and maintains software systems.",
            new() { "Write and review code", "Design system architecture", "Debug and test software" },
            new() { S("Mathematics", "5"), S("Information Technology or Physical Sciences", "4") },
            new() { "BSc Computer Science", "BEng Software Engineering", "Higher Certificate in IT + industry certification" },
            new() { RiasecType.Investigative, RiasecType.Realistic, RiasecType.Conventional }),

        C(2, "Professional Nurse", "OFO-DEMO-2221", "Provides clinical nursing care in hospitals and clinics.",
            new() { "Assess and monitor patients", "Administer treatment", "Educate patients on care" },
            new() { S("Life Sciences", "5"), S("Mathematics or Mathematical Literacy", "4") },
            new() { "BCur Nursing Science", "Diploma in Nursing" },
            new() { RiasecType.Social, RiasecType.Investigative, RiasecType.Realistic }),

        C(3, "Civil Engineer", "OFO-DEMO-2142", "Designs and oversees construction of infrastructure such as roads, bridges and buildings.",
            new() { "Design structures and systems", "Manage construction projects", "Ensure safety and compliance" },
            new() { S("Mathematics", "6"), S("Physical Sciences", "5") },
            new() { "BEng Civil Engineering", "BSc Engineering: Civil" },
            new() { RiasecType.Realistic, RiasecType.Investigative }),

        C(4, "Chartered Accountant", "OFO-DEMO-2411", "Prepares, audits and advises on financial records and reporting.",
            new() { "Prepare financial statements", "Conduct audits", "Advise on tax and compliance" },
            new() { S("Mathematics", "6"), S("Accounting", "5") },
            new() { "BCom Accounting + CTA + articles (SAICA)" },
            new() { RiasecType.Conventional, RiasecType.Enterprising, RiasecType.Investigative }),

        C(5, "Medical Doctor", "OFO-DEMO-2211", "Diagnoses and treats illness and injury in patients.",
            new() { "Examine and diagnose patients", "Prescribe treatment", "Coordinate patient care" },
            new() { S("Life Sciences", "6"), S("Physical Sciences", "6"), S("Mathematics", "6") },
            new() { "MBChB" },
            new() { RiasecType.Investigative, RiasecType.Social }),

        C(6, "Attorney", "OFO-DEMO-2611", "Advises and represents clients on legal matters.",
            new() { "Research and interpret law", "Draft legal documents", "Represent clients" },
            new() { S("English Home Language or English First Additional Language", "5") },
            new() { "LLB" },
            new() { RiasecType.Enterprising, RiasecType.Investigative, RiasecType.Social }),

        C(7, "Foundation Phase Teacher", "OFO-DEMO-2341", "Teaches and supports young learners in the early grades.",
            new() { "Plan and deliver lessons", "Assess learner progress", "Support learner development" },
            new() { S("English Home Language or English First Additional Language", "4") },
            new() { "BEd Foundation Phase" },
            new() { RiasecType.Social, RiasecType.Artistic }),

        C(8, "Electrician", "OFO-DEMO-6131", "Installs and maintains electrical systems and equipment.",
            new() { "Install wiring and fittings", "Diagnose electrical faults", "Test for compliance and safety" },
            new() { S("Mathematics or Mathematical Literacy", "4"), S("Physical Sciences or Electrical Technology", "4") },
            new() { "TVET N-diploma: Electrical Engineering + apprenticeship", "Trade test" },
            new() { RiasecType.Realistic, RiasecType.Conventional }),

        C(9, "Graphic Designer", "OFO-DEMO-2166", "Creates visual concepts for print, digital and brand media.",
            new() { "Design layouts and visuals", "Develop brand identities", "Collaborate with clients" },
            new() { S("Visual Arts or Information Technology", "4") },
            new() { "Diploma in Graphic Design", "BA Visual Communication" },
            new() { RiasecType.Artistic, RiasecType.Enterprising }),

        C(10, "Agricultural Scientist", "OFO-DEMO-2132", "Researches and improves farming methods, crops and livestock.",
            new() { "Conduct field and lab research", "Analyse soil, crop and livestock data", "Advise on farming practices" },
            new() { S("Life Sciences", "5"), S("Mathematics", "5") },
            new() { "BSc Agriculture" },
            new() { RiasecType.Investigative, RiasecType.Realistic }),

        C(11, "Marketing Manager", "OFO-DEMO-1221", "Plans and leads marketing strategy for products or brands.",
            new() { "Develop marketing campaigns", "Analyse market trends", "Manage brand strategy" },
            new() { S("English Home Language or English First Additional Language", "4"), S("Mathematics or Mathematical Literacy", "4") },
            new() { "BCom Marketing Management", "BA Communication + experience" },
            new() { RiasecType.Enterprising, RiasecType.Artistic, RiasecType.Social }),

        C(12, "Quantity Surveyor", "OFO-DEMO-2143", "Manages construction costs and contracts from planning to completion.",
            new() { "Estimate project costs", "Manage tenders and contracts", "Track project budgets" },
            new() { S("Mathematics", "6"), S("Physical Sciences", "4") },
            new() { "BSc Quantity Surveying" },
            new() { RiasecType.Conventional, RiasecType.Investigative }),

        C(13, "Pharmacist", "OFO-DEMO-2262", "Dispenses medication and advises on safe medicine use.",
            new() { "Dispense prescriptions", "Advise patients on medication", "Monitor drug interactions" },
            new() { S("Physical Sciences", "6"), S("Life Sciences", "5"), S("Mathematics", "5") },
            new() { "BPharm" },
            new() { RiasecType.Investigative, RiasecType.Conventional }),

        C(14, "Social Worker", "OFO-DEMO-2635", "Supports individuals and families facing social, emotional or economic challenges.",
            new() { "Assess client needs", "Develop support plans", "Connect clients with services" },
            new() { S("Life Orientation or Social Sciences", "4") },
            new() { "BSW (Bachelor of Social Work)" },
            new() { RiasecType.Social, RiasecType.Investigative }),

        C(15, "Chef", "OFO-DEMO-3434", "Plans menus and prepares food in professional kitchens.",
            new() { "Prepare and plate dishes", "Plan menus", "Manage kitchen operations" },
            new() { S("Consumer Studies or Hospitality Studies", "4") },
            new() { "Diploma in Culinary Arts", "Chef's apprenticeship" },
            new() { RiasecType.Realistic, RiasecType.Artistic }),

        C(16, "Journalist", "OFO-DEMO-2642", "Researches, writes and reports news and feature stories.",
            new() { "Research and verify stories", "Write and edit copy", "Conduct interviews" },
            new() { S("English Home Language or English First Additional Language", "5") },
            new() { "BA Journalism" },
            new() { RiasecType.Artistic, RiasecType.Social, RiasecType.Enterprising }),

        C(17, "Mechanical Engineer", "OFO-DEMO-2144", "Designs, builds and maintains mechanical systems and machinery.",
            new() { "Design mechanical components", "Test and prototype systems", "Oversee manufacturing processes" },
            new() { S("Mathematics", "6"), S("Physical Sciences", "6") },
            new() { "BEng Mechanical Engineering" },
            new() { RiasecType.Realistic, RiasecType.Investigative }),

        C(18, "Actuary", "OFO-DEMO-2120", "Uses statistics and financial theory to assess and manage risk.",
            new() { "Model financial risk", "Price insurance products", "Advise on long-term financial planning" },
            new() { S("Mathematics", "7") },
            new() { "BSc Actuarial Science + professional exams (ASSA)" },
            new() { RiasecType.Investigative, RiasecType.Conventional }),

        C(19, "Architect", "OFO-DEMO-2161", "Designs buildings and oversees their construction.",
            new() { "Design buildings and spaces", "Prepare drawings and models", "Liaise with clients and engineers" },
            new() { S("Mathematics", "5"), S("Visual Arts or Physical Sciences", "4") },
            new() { "BAS + MArch (Professional Architecture)" },
            new() { RiasecType.Artistic, RiasecType.Investigative, RiasecType.Realistic }),

        C(20, "Police Officer", "OFO-DEMO-3355", "Maintains public safety and enforces the law.",
            new() { "Patrol and respond to incidents", "Investigate offences", "Support community safety" },
            new() { S("English Home Language or English First Additional Language", "4") },
            new() { "SAPS Basic Police Development Learning Programme" },
            new() { RiasecType.Realistic, RiasecType.Social, RiasecType.Enterprising }),

        C(21, "Human Resources Manager", "OFO-DEMO-1212", "Manages recruitment, employee relations and workplace policy.",
            new() { "Manage recruitment and onboarding", "Handle employee relations", "Develop HR policy" },
            new() { S("English Home Language or English First Additional Language", "4") },
            new() { "BCom Human Resource Management" },
            new() { RiasecType.Social, RiasecType.Enterprising, RiasecType.Conventional }),

        C(22, "Veterinarian", "OFO-DEMO-2250", "Diagnoses and treats illness and injury in animals.",
            new() { "Examine and treat animals", "Perform surgery", "Advise owners on animal care" },
            new() { S("Life Sciences", "6"), S("Physical Sciences", "5"), S("Mathematics", "5") },
            new() { "BVSc (Veterinary Science)" },
            new() { RiasecType.Investigative, RiasecType.Social, RiasecType.Realistic }),

        C(23, "Air Traffic Controller", "OFO-DEMO-3154", "Directs aircraft movements to keep air travel safe and efficient.",
            new() { "Monitor aircraft positions", "Issue flight instructions", "Coordinate with pilots and airports" },
            new() { S("Mathematics", "5"), S("Physical Sciences or English Home Language", "4") },
            new() { "ATNS Air Traffic Control training programme" },
            new() { RiasecType.Conventional, RiasecType.Investigative, RiasecType.Realistic }),

        C(24, "Small Business Owner / Entrepreneur", "OFO-DEMO-1439", "Starts, runs and grows an independent business.",
            new() { "Develop business plans", "Manage operations and finances", "Build and lead a team" },
            new() { S("Business Studies or Mathematics", "4") },
            new() { "BCom Entrepreneurship", "Any qualification + industry experience" },
            new() { RiasecType.Enterprising, RiasecType.Realistic }),
    };

    private static Career C(
        int id, string title, string ofoCode, string summary,
        List<string> responsibilities, List<SubjectRequirement> requiredSubjects,
        List<string> pathways, List<RiasecType> riasecTags) => new()
    {
        Id = id,
        Title = title,
        OfoCode = ofoCode,
        Summary = summary,
        Responsibilities = responsibilities,
        RequiredSubjects = requiredSubjects,
        Pathways = pathways,
        RiasecTags = riasecTags
    };

    private static SubjectRequirement S(string subject, string level) => new() { Subject = subject, Level = level };
}
