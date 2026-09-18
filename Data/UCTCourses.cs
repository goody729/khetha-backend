using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Data;

/// <summary>
/// University of Cape Town undergraduate qualifications, subject requirements and Total APS,
/// sourced from the 2027 Undergraduate Prospectus (NSC admission tables).
/// UCT publishes admission bands per QUALIFICATION rather than per individual major (most majors
/// within a faculty share one entry table), and uses a Faculty Points Score (FPS, on a faculty-specific
/// scale) rather than a single national APS. TotalAPS below is the Band A "guaranteed admission" FPS/WPS
/// threshold for NSC (South African) applicants; Notes captures the Band B/C alternatives, NBT
/// requirements and the specialisations covered by each qualification.
/// </summary>
public static class UCTCourses
{
    private const string Eng = "English (Home Language or First Additional Language)";
    private const string Math = "Mathematics";
    private const string PhysSci = "Physical Sciences";

    private static Course C(string name, string duration, string faculty, int aps, string notes, params (string Subject, string Level)[] reqs)
    {
        return new Course
        {
            Name = name,
            Duration = duration,
            Faculty = faculty,
            TotalAPS = aps,
            Notes = notes,
            SubjectRequirements = reqs.Select(r => new SubjectRequirement { Subject = r.Subject, Level = r.Level }).ToList()
        };
    }

    public static readonly IReadOnlyList<Course> All = new List<Course>
    {
        // Faculty of Commerce
        C("Bachelor of Commerce / Bachelor of Business Science (all specialisations except Actuarial Science, Computer Science and Statistics & Data Science)",
            "3 years (BCom) / 4 years (BBusSc)", "Faculty of Commerce", 435,
            "FPS out of 600. Band A (guaranteed): FPS 435+. Band B (likely, WPS): 470+. Band C (SA redress categories, EDU programme only): FPS 430-434. NBT AL & QL Upper Intermediate or above required. Specialisations include Accounting, Actuarial-adjacent Finance, Economics (incl. PPE, Economics with Law/Finance/Statistics), Information Systems, Management Studies, Marketing, and Industrial & Organisational Psychology.",
            (Math, "60%"), ("English Home Language", "50%"), ("English First Additional Language", "60%")),
        C("Bachelor of Commerce / Bachelor of Business Science specialising in Computer Science or Statistics and Data Science",
            "3 years (BCom) / 4 years (BBusSc)", "Faculty of Commerce", 435,
            "FPS out of 600. Band A (guaranteed): FPS 435+. Band B (likely, WPS): 470+. Band C (EDU only): FPS 430-434. NBT AL & QL Upper Intermediate or above required.",
            (Math, "70%"), ("English Home Language", "50%"), ("English First Additional Language", "60%")),
        C("Bachelor of Commerce / Bachelor of Business Science specialising in Actuarial Science or Quantitative Finance",
            "3 years (BCom) / 4 years (BBusSc)", "Faculty of Commerce", 500,
            "FPS out of 600. Band A (guaranteed): FPS 500+. Band B (likely, WPS): 525+. Band C (EDU only): FPS 475-479. NBT AL & QL Upper Intermediate required (Proficient if English is a second language).",
            (Math, "80%"), ("English Home Language", "60%"), ("English First Additional Language", "80%")),

        // Faculty of Engineering & the Built Environment
        C("Bachelor of Science in Engineering in Civil Engineering", "4 years", "Faculty of Engineering & the Built Environment", 500,
            "FPS out of 800 (approx.). Band A (guaranteed): FPS 500+. Band B (likely, WPS): 480+. Band C (SA redress categories): FPS 420+. NBTs in Mathematics, AL & QL must be written but do not affect the admission decision. Mathematical Literacy/Technical Mathematics cannot substitute for Mathematics; Technical Science cannot substitute for Physical Sciences.",
            (Math, "80%"), (PhysSci, "70%")),
        C("Bachelor of Science in Engineering in Electrical Engineering, Electrical & Computer Engineering, or Mechatronics", "4 years", "Faculty of Engineering & the Built Environment", 500,
            "Band A (guaranteed): FPS 500+. Band B (likely, WPS): 480+. Band C (SA redress categories): FPS 420+. NBTs in Mathematics, AL & QL must be written but do not affect the admission decision.",
            (Math, "80%"), (PhysSci, "75%")),
        C("Bachelor of Science in Engineering in Chemical Engineering", "4 years", "Faculty of Engineering & the Built Environment", 500,
            "Band A (guaranteed): FPS 500+ with Physical Sciences 75%+. Band B (likely, WPS): 480+ with Physical Sciences 70%+. Band C (SA redress categories): FPS 420+ with Physical Sciences 70%+. NBTs in Mathematics, AL & QL must be written but do not affect the admission decision.",
            (Math, "80%"), (PhysSci, "75%")),
        C("Bachelor of Science in Engineering in Mechanical, or Mechanical & Mechatronic Engineering", "4 years", "Faculty of Engineering & the Built Environment", 500,
            "Band A (guaranteed): FPS 500+. Band B (likely, WPS): 480+. Band C (SA redress categories): FPS 420+. NBTs in Mathematics, AL & QL must be written but do not affect the admission decision.",
            (Math, "80%"), (PhysSci, "75%")),
        C("Bachelor of Science in Construction Studies", "3 years", "Faculty of Engineering & the Built Environment", 450,
            "Band A (guaranteed): FPS 450+ with Physical Sciences 60%+. Band B (likely, WPS): 420+ with Physical Sciences 55%+. Band C (SA redress categories): FPS 390+ with Physical Sciences 55%+. NBTs in Mathematics, AL & QL must be written but do not affect the admission decision.",
            (Math, "65%"), (PhysSci, "60%")),
        C("Bachelor of Science in Property Studies", "3 years", "Faculty of Engineering & the Built Environment", 450,
            "Band A (guaranteed): FPS 450+. Band B (likely, WPS): 420+. Band C (SA redress categories): FPS 390+. NBTs in Mathematics, AL & QL must be written but do not affect the admission decision.",
            (Math, "65%")),
        C("Bachelor of Architectural Studies (BAS)", "3 years", "Faculty of Engineering & the Built Environment", 450,
            "Band A (guaranteed): FPS 450+ with a portfolio score of 75%+. Band B (likely, WPS): 408+ with portfolio 68%+. Band C (SA redress categories): FPS 348+ with portfolio 50%+. Requires a written motivation and portfolio of creative work. NBTs in Mathematics, AL & QL must be written but do not affect the admission decision.",
            (Math, "50%"), ("English", "50%")),
        C("Bachelor of Science in Geomatics", "4 years", "Faculty of Engineering & the Built Environment", 450,
            "Band A (guaranteed): FPS 450+ with Physical Sciences 70%+. Band B (probable, WPS): 420+ with Mathematics 65%+ and Physical Sciences 60%+. Band C (SA redress categories): FPS 390+ with Mathematics 65%+ and Physical Sciences 60%+. NBTs in Mathematics, AL & QL must be written but do not affect the admission decision.",
            (Math, "75%"), (PhysSci, "70%")),

        // Faculty of Health Sciences
        C("Bachelor of Medicine and Bachelor of Surgery (MBChB)", "6 years", "Faculty of Health Sciences", 810,
            "FPS out of 900 (APS + NBT score out of 300). Sub-minimum APS of 450. Band A (guaranteed): FPS 810+ with NBT Proficient for AL, QL and Mathematics. Band B (likely, WPS): 807+ with NBT Intermediate or above. Band C (SA redress categories): FPS 644+ with NBT Intermediate or above. Also requires 70%+ for the next three best subjects excluding Life Orientation.",
            (Math, "70%"), (PhysSci, "70%"), ("English (Home or First Additional Language)", "65%")),
        C("Bachelor of Science in Physiotherapy", "4 years", "Faculty of Health Sciences", 730,
            "FPS out of 900. Sub-minimum APS of 360. Band A (guaranteed): FPS 730+ with NBT Intermediate or above for AL, QL and Mathematics. Band B (likely, WPS): 797+. Band C (SA redress categories): FPS 580 (redress 1), 610 (redress 2), or 680 (redress 3/4).",
            (Math, "60%"), ("Physical Sciences or Life Sciences", "65%"), ("English (Home or First Additional Language)", "65%")),
        C("Bachelor of Science in Occupational Therapy", "4 years", "Faculty of Health Sciences", 730,
            "FPS out of 900. Sub-minimum APS of 340. Band A (guaranteed): FPS 730+ with NBT Intermediate or above (Mathematical Literacy applicants above 75% are exempt from the NBT Mathematics test). Band B (likely, WPS): 782+. Band C (SA redress categories): FPS 565 (redress 1), 580 (redress 2), or 670 (redress 3/4).",
            ("Mathematics (or Mathematical Literacy 70%)", "60%"), ("Physical Sciences or Life Sciences", "65%"), ("English (Home or First Additional Language)", "65%")),
        C("Bachelor of Science in Audiology", "4 years", "Faculty of Health Sciences", 720,
            "FPS out of 900. Sub-minimum APS of 340. Band A (guaranteed): FPS 720+. Band B (likely, WPS): 710+. Band C (SA redress categories): FPS 550 (redress 1), 565 (redress 2), or 610 (redress 3/4).",
            ("Mathematics (or Mathematical Literacy 70%)", "60%"), ("Physical Sciences or Life Sciences", "65%"), ("English (Home or First Additional Language)", "65%")),
        C("Bachelor of Science in Speech-Language Pathology", "4 years", "Faculty of Health Sciences", 715,
            "FPS out of 900. Sub-minimum APS of 340. Band A (guaranteed): FPS 715+. Band B (likely, WPS): 670+. Band C (SA redress categories): FPS 510 (redress 1), 515 (redress 2), or 600 (redress 3/4).",
            ("Mathematics (or Mathematical Literacy 70%)", "60%"), ("Physical Sciences or Life Sciences", "65%"), ("English (Home or First Additional Language)", "65%")),
        C("Higher Certificate in Disability Practice", "1 year", "Faculty of Health Sciences", 0,
            "Not scored via FPS bands. Requires a National Senior Certificate (or equivalent) and Upper Intermediate to Proficient level in the NBT Academic and Quantitative Literacy components."),

        // Faculty of Humanities
        C("Bachelor of Arts (BA) / Bachelor of Social Science (BSocSc)", "3 years", "Faculty of Humanities", 450,
            "FPS out of roughly 600. Minimum to be considered: FPS 380 with NBT AL Intermediate. Band A (guaranteed): FPS 450+ with NBT AL Proficient. Band B (likely, WPS): 450+ with NBT AL Upper Intermediate or above. Band C (SA redress categories): FPS 380+ with NBT AL Intermediate or above. Covers most Arts and Social Science majors; Economics (60% Maths) and Psychology (50% Maths or NBT QL Proficient) carry additional quantitative requirements.",
            ("English Home Language", "50%"), ("English First Additional Language", "60%")),
        C("Bachelor of Social Science in Philosophy, Politics and Economics (PPE)", "3 years", "Faculty of Humanities", 450,
            "Band A (guaranteed, the only published band): FPS 450+ with NBT AL Proficient and QL Upper Intermediate or above.",
            (Math, "60%"), ("English Home Language", "50%"), ("English First Additional Language", "60%")),
        C("Bachelor of Social Work (BSW)", "4 years", "Faculty of Humanities", 450,
            "Band A (guaranteed): FPS 450+ with NBT AL Proficient. Band B (likely, WPS): 450+ with NBT AL Upper Intermediate or above. Band C (SA redress categories): FPS 380+ with NBT AL Intermediate or above. Applicants may be required to attend an admissions interview.",
            ("English Home Language", "50%"), ("English First Additional Language", "60%")),
        C("Bachelor of Arts in Fine Art (BA(FA))", "4 years", "Faculty of Humanities", 380,
            "FPS 380+ is a floor requirement with NBT AL Intermediate or above; the leading admission indicator is a satisfactory portfolio evaluation, and places are awarded on merit.",
            ("English Home Language", "50%"), ("English First Additional Language", "60%")),
        C("Bachelor of Arts in Theatre & Performance (BA(T&P))", "4 years", "Faculty of Humanities", 380,
            "FPS 380+ is a floor requirement with NBT AL Intermediate or above; the leading admission indicator is a satisfactory audition, and places are awarded on merit.",
            ("English Home Language", "50%"), ("English First Additional Language", "60%")),
        C("Bachelor of Music (BMus)", "4 years", "Faculty of Humanities", 380,
            "FPS 380+ is a floor requirement with NBT AL Intermediate or above, plus a satisfactory audition, interview and music theory test. NSC Music 60%+ (or Unisa Music Theory Grade V+ and Practical Grade VII+) preferred. The leading admission indicator is audition performance.",
            ("English Home Language", "50%"), ("English First Additional Language", "60%")),
        C("Diploma in Music Performance (DMP)", "3 years", "Faculty of Humanities", 0,
            "NSC endorsed for diploma study, NBT AL Intermediate or above, plus a satisfactory audition, interview and music theory test."),
        C("Diploma in Theatre & Performance (DTP)", "3 years", "Faculty of Humanities", 0,
            "NSC endorsed for diploma study, NBT AL Intermediate or above, plus a satisfactory audition."),

        // Faculty of Law
        C("Bachelor of Laws (LLB) - undergraduate route", "4 years", "Faculty of Law", 500,
            "FPS out of 600. Band A (guaranteed, SA applicants): FPS 500+ with NBT AL Proficient and QL Intermediate or above. Band B (likely, WPS): 500+. Band C (SA redress categories): FPS 470+ with NBT Proficient for AL and QL. International applicants: FPS 510+ (capped at 10 places). A combined Humanities/Commerce LLB (5 years) and a Graduate LLB (3 years, for holders of another undergraduate degree) are also offered, admitted via first-year GPA thresholds (65% Humanities / 63% Commerce for the combined route; 65% cumulative degree average for the graduate route) rather than school-leaving APS."),

        // Faculty of Science
        C("Bachelor of Science (BSc)", "3 years", "Faculty of Science", 660,
            "FPS out of 800 (best six NSC subjects including English, excluding Life Orientation, with Mathematics and Physical Sciences doubled). Minimum FPS to be considered is 550. Band A (guaranteed): FPS 660+. Band B (likely, WPS): 640+. Band C (SA redress categories): FPS 550+. Covers all Science majors (Applied Mathematics, Applied Statistics, Archaeology, Artificial Intelligence, Astrophysics, Biology, Biochemistry, Chemistry, Computer Science, Environmental & Geographical Science, Genetics, Geology, Human Anatomy & Physiology, Marine Biology, Mathematics, Mathematical Statistics, Ocean & Atmosphere Science, Physics, Quantitative Biology, Statistics & Data Science). NBTs in Mathematics, AL & QL must be written but do not affect the admission decision (used for placement into the Extended Degree Programme).",
            (Math, "70%"), (PhysSci, "60%")),
    };
}
