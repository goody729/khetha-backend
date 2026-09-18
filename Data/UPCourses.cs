using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Data;

/// <summary>
/// University of Pretoria undergraduate programmes, subject requirements and Total APS,
/// sourced from the Undergraduate Prospectus 2027 (Applicants with an NSC/IEB Certificate).
/// </summary>
public static class UPCourses
{
    private const string Eng = "English Home Language or English First Additional Language";
    private const string Math = "Mathematics";
    private const string PhysSci = "Physical Sciences";

    private static Course C(string name, string duration, string faculty, int aps, params (string Subject, string Level)[] requirements)
    {
        return new Course
        {
            Name = name,
            Duration = duration,
            Faculty = faculty,
            TotalAPS = aps,
            SubjectRequirements = requirements.Select(r => new SubjectRequirement { Subject = r.Subject, Level = r.Level }).ToList()
        };
    }

    public static readonly IReadOnlyList<Course> All = new List<Course>
    {
        // Faculty of Economic and Management Sciences
        C("Bachelor of Administration specialising in Public Administration and International Relations", "3 years", "Faculty of Economic and Management Sciences", 28,
            (Eng, "5"), (Math, "3 or Mathematical Literacy 4")),
        C("Bachelor of Commerce in Accounting Sciences", "3 years", "Faculty of Economic and Management Sciences", 34,
            (Eng, "5"), (Math, "6")),
        C("Bachelor of Commerce specialising in Investment Management", "3 years", "Faculty of Economic and Management Sciences", 34,
            (Eng, "5"), (Math, "6")),
        C("Bachelor of Commerce specialising in Financial Management Sciences", "3 years", "Faculty of Economic and Management Sciences", 32,
            (Eng, "5"), (Math, "5")),
        C("Bachelor of Commerce specialising in Econometrics", "3 years", "Faculty of Economic and Management Sciences", 32,
            (Eng, "5"), (Math, "6")),
        C("Bachelor of Commerce specialising in Economics", "3 years", "Faculty of Economic and Management Sciences", 32,
            (Eng, "5"), (Math, "5")),
        C("Bachelor of Commerce specialising in Law", "3 years", "Faculty of Economic and Management Sciences", 32,
            (Eng, "5"), (Math, "5")),
        C("Bachelor of Commerce specialising in Statistics and Data Science", "3 years", "Faculty of Economic and Management Sciences", 32,
            (Eng, "5"), (Math, "5")),
        C("Bachelor of Commerce specialising in Information Systems", "3 years", "Faculty of Economic and Management Sciences", 30,
            (Eng, "5"), (Math, "5")),
        C("Bachelor of Commerce specialising in Agribusiness Management", "3 years", "Faculty of Economic and Management Sciences", 30,
            (Eng, "5"), (Math, "5")),
        C("Bachelor of Commerce specialising in Business Management", "3 years", "Faculty of Economic and Management Sciences", 30,
            (Eng, "5"), (Math, "4")),
        C("Bachelor of Commerce specialising in Supply Chain Management", "3 years", "Faculty of Economic and Management Sciences", 30,
            (Eng, "5"), (Math, "4")),
        C("Bachelor of Commerce specialising in Marketing Management", "3 years", "Faculty of Economic and Management Sciences", 30,
            (Eng, "5"), (Math, "4")),
        C("Bachelor of Commerce specialising in Human Resource Management", "3 years", "Faculty of Economic and Management Sciences", 30,
            (Eng, "5"), (Math, "4")),
        C("Bachelor of Commerce", "3 years", "Faculty of Economic and Management Sciences", 30,
            (Eng, "5"), (Math, "4")),
        C("Bachelor of Commerce", "4 years", "Faculty of Economic and Management Sciences", 26,
            (Eng, "4"), (Math, "3")),

        // Faculty of Education
        C("Bachelor of Education in Early Childhood Care and Education", "4 years", "Faculty of Education", 28,
            (Eng, "4")),
        C("Bachelor of Education in Foundation Phase Teaching", "4 years", "Faculty of Education", 28,
            (Eng, "4")),
        C("Bachelor of Education in Intermediate Phase Teaching", "4 years", "Faculty of Education", 28,
            (Eng, "4")),
        C("Bachelor of Education in Senior Phase and Further Education and Training Teaching", "4 years", "Faculty of Education", 28,
            (Eng, "4"),
            (PhysSci, "5 (for elective modules in Physical Sciences or Life Sciences)"),
            (Math, "5 (for elective modules in Physical Sciences or Life Sciences)")),
        C("Higher Certificate in Sports Sciences", "1 year", "Faculty of Education", 20,
            (Eng, "4")),
        C("Higher Certificate in Sports Sciences (online)", "2 years", "Faculty of Education", 20,
            (Eng, "4")),

        // Faculty of Engineering, Built Environment and Information Technology - School of Engineering
        C("Bachelor of Engineering in Chemical Engineering", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "6")),
        C("Bachelor of Engineering in Civil Engineering", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "6")),
        C("Bachelor of Engineering in Computer Engineering", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "6")),
        C("Bachelor of Engineering in Electrical Engineering", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "6")),
        C("Bachelor of Engineering in Electronic Engineering", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "6")),
        C("Bachelor of Engineering in Industrial Engineering", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "6")),
        C("Bachelor of Engineering in Mechanical Engineering", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "6")),
        C("Bachelor of Engineering in Metallurgical Engineering", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "6")),
        C("Bachelor of Engineering in Mining Engineering", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "6")),
        C("Bachelor of Engineering (any discipline)", "5 years", "Faculty of Engineering, Built Environment and Information Technology (School of Engineering)", 33,
            (Eng, "65%"), (Math, "65%"), (PhysSci, "65%")),

        // School for the Built Environment
        C("Bachelor of Science in Architecture", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School for the Built Environment)", 30,
            (Eng, "5"), (Math, "4"), (PhysSci, "4")),
        C("Bachelor of Science in Construction Management", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School for the Built Environment)", 30,
            (Eng, "5"), (Math, "5 or Accounting 4")),
        C("Bachelor of Science in Real Estate", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School for the Built Environment)", 30,
            (Eng, "5"), (Math, "5 or Accounting 4")),
        C("Bachelor of Science in Quantity Surveying", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School for the Built Environment)", 30,
            (Eng, "5"), (Math, "5 or Accounting 4")),
        C("Bachelor of Town and Regional Planning", "4 years", "Faculty of Engineering, Built Environment and Information Technology (School for the Built Environment)", 30,
            (Eng, "5"), (Math, "4")),

        // School of Information Technology
        C("Bachelor of Information Science", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School of Information Technology)", 28,
            (Eng, "4")),
        C("Bachelor of Information Science specialising in Publishing", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School of Information Technology)", 28,
            (Eng, "5")),
        C("Bachelor of Information Science specialising in Multimedia", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School of Information Technology)", 30,
            (Eng, "4"), (Math, "5")),
        C("Bachelor of Information Technology in Information Systems", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School of Information Technology)", 30,
            (Eng, "5"), (Math, "5")),
        C("Bachelor of Science in Computer Science", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School of Information Technology)", 30,
            (Eng, "5"), (Math, "6")),
        C("Bachelor of Science in Information Technology in Information and Knowledge Systems", "3 years", "Faculty of Engineering, Built Environment and Information Technology (School of Information Technology)", 30,
            (Eng, "4"), (Math, "6")),

        // Faculty of Health Sciences - School of Dentistry
        C("Bachelor of Dental Surgery", "5 years", "Faculty of Health Sciences (School of Dentistry)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "5")),
        C("Bachelor of Oral Hygiene", "3 years", "Faculty of Health Sciences (School of Dentistry)", 25,
            (Eng, "4"), (Math, "4"), (PhysSci, "4")),

        // School of Healthcare Sciences
        C("Bachelor of Dietetics", "4 years", "Faculty of Health Sciences (School of Healthcare Sciences)", 28,
            (Eng, "4"), (Math, "4"), (PhysSci, "4")),
        C("Bachelor of Nursing Science", "4 years", "Faculty of Health Sciences (School of Healthcare Sciences)", 28,
            (Eng, "4"), (Math, "4"), ("Life Sciences (not Physical Sciences)", "4")),
        C("Bachelor of Occupational Therapy", "4 years", "Faculty of Health Sciences (School of Healthcare Sciences)", 30,
            (Eng, "4"), (Math, "4"), (PhysSci, "4")),
        C("Bachelor of Physiotherapy", "4 years", "Faculty of Health Sciences (School of Healthcare Sciences)", 30,
            (Eng, "4"), (Math, "4"), (PhysSci, "4")),
        C("Bachelor of Radiography in Diagnostics", "4 years", "Faculty of Health Sciences (School of Healthcare Sciences)", 30,
            (Eng, "4"), (Math, "4"), (PhysSci, "4")),

        // School of Medicine
        C("Bachelor of Clinical Medical Practice (BCMP)", "3 years", "Faculty of Health Sciences (School of Medicine)", 28,
            (Eng, "4"), (Math, "4"), ("Physical Sciences or Life Sciences", "4")),
        C("Bachelor of Medicine and Surgery (MBChB)", "6 years", "Faculty of Health Sciences (School of Medicine)", 35,
            (Eng, "5"), (Math, "6"), (PhysSci, "5")),
        C("Bachelor of Sports Science", "3 years", "Faculty of Health Sciences (School of Medicine)", 30,
            (Eng, "4"), (Math, "4"), ("Physical Sciences or Life Sciences", "4")),

        // Faculty of Humanities
        C("Bachelor of Arts in Speech-Language Pathology", "4 years", "Faculty of Humanities", 32,
            (Math, "4"), (Eng, "5")),
        C("Bachelor of Arts in Audiology", "4 years", "Faculty of Humanities", 32,
            (Math, "4"), (Eng, "5")),
        C("Bachelor of Arts in Information Design", "4 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Arts", "3 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Social Work", "4 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Arts specialising in Languages", "3 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Social Science specialising in Industrial Sociology and Labour Studies", "3 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Social Science in Heritage and Cultural Sciences", "3 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Music", "4 years", "Faculty of Humanities", 30,
            (Eng, "5"),
            ("Music", "4 (50-59%) or Grade VII Practical and Grade V Theory (Unisa/Royal Schools/Trinity) or comparable, plus practical audition and theoretical test passed with 60%")),
        C("Bachelor of Music", "5 years", "Faculty of Humanities", 26,
            (Eng, "4"),
            ("Music", "4 (50-59%) or Grade V Practical and Grade III Theory (Unisa/Royal Schools/Trinity) or comparable, plus practical audition and theoretical test passed with 50%")),
        C("Bachelor of Drama", "3 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Drama", "4 years", "Faculty of Humanities", 26,
            (Eng, "4")),
        C("Bachelor of Arts specialising in Philosophy, Politics and Economics", "3 years", "Faculty of Humanities", 32,
            (Math, "5"), (Eng, "5")),
        C("Bachelor of Political Science specialising in International Studies", "3 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Political Science specialising in Political Studies", "3 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Arts in Fine Arts", "4 years", "Faculty of Humanities", 30,
            (Eng, "5")),
        C("Bachelor of Arts in Fine Arts", "5 years", "Faculty of Humanities", 26,
            (Eng, "4")),
        C("Bachelor of Arts specialising in Visual Studies", "3 years", "Faculty of Humanities", 30,
            (Eng, "5")),

        // Faculty of Law
        C("Bachelor of Laws (LLB)", "4 years", "Faculty of Law", 35,
            (Eng, "6")),
        C("Bachelor of Commerce specialising in Law", "3 years", "Faculty of Law", 32,
            (Eng, "5"), (Math, "5")),
        C("Bachelor of Arts specialising in Law", "3 years", "Faculty of Law", 34,
            (Eng, "5")),

        // Faculty of Theology and Religion
        C("Bachelor of Theology", "3 years", "Faculty of Theology and Religion", 28,
            (Eng, "4")),
        C("Bachelor of Divinity", "4 years", "Faculty of Theology and Religion", 28,
            (Eng, "4")),
        C("Diploma in Theology", "3 years", "Faculty of Theology and Religion", 24,
            (Eng, "4")),

        // Faculty of Natural and Agricultural Sciences - Agricultural and Food Sciences
        C("Bachelor of Science in Agriculture in Agricultural Economics in Agribusiness Management", "4 years", "Faculty of Natural and Agricultural Sciences (Agricultural and Food Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Agriculture in Animal Science", "4 years", "Faculty of Natural and Agricultural Sciences (Agricultural and Food Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Agriculture in Applied Plant and Soil Sciences", "4 years", "Faculty of Natural and Agricultural Sciences (Agricultural and Food Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Agriculture in Plant Pathology", "4 years", "Faculty of Natural and Agricultural Sciences (Agricultural and Food Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Food Management (Culinary Science option)", "4 years", "Faculty of Natural and Agricultural Sciences (Agricultural and Food Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Food Management (Nutritional Science option)", "4 years", "Faculty of Natural and Agricultural Sciences (Agricultural and Food Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Food Science", "3 years", "Faculty of Natural and Agricultural Sciences (Agricultural and Food Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),

        // Biological Sciences
        C("Bachelor of Science in Biochemistry", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Biotechnology", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Ecology", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Entomology", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Genetics / Bachelor of Science in Human Genetics", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Human Physiology / Bachelor of Science in Human Physiology, Genetics and Psychology", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Medical Sciences", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Microbiology", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Plant Science", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Zoology", "3 years", "Faculty of Natural and Agricultural Sciences (Biological Sciences)", 32,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),

        // Consumer Science
        C("Bachelor of Consumer Science specialising in Clothing Retail Management", "4 years", "Faculty of Natural and Agricultural Sciences (Consumer Science)", 28,
            (Eng, "5"), (Math, "4")),
        C("Bachelor of Consumer Science specialising in Food Management", "4 years", "Faculty of Natural and Agricultural Sciences (Consumer Science)", 28,
            (Eng, "5"), (Math, "4")),

        // Mathematical Sciences
        C("Bachelor of Science in Actuarial and Financial Mathematics", "3 years", "Faculty of Natural and Agricultural Sciences (Mathematical Sciences)", 36,
            (Eng, "5"), (Math, "7")),
        C("Bachelor of Science in Mathematics / Bachelor of Science in Applied Mathematics", "3 years", "Faculty of Natural and Agricultural Sciences (Mathematical Sciences)", 34,
            (Eng, "5"), (Math, "6")),
        C("Bachelor of Science in Mathematical Statistics", "3 years", "Faculty of Natural and Agricultural Sciences (Mathematical Sciences)", 34,
            (Eng, "5"), (Math, "6")),

        // Physical Sciences
        C("Bachelor of Science in Chemistry", "3 years", "Faculty of Natural and Agricultural Sciences (Physical Sciences)", 34,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Environmental and Engineering Geology", "3 years", "Faculty of Natural and Agricultural Sciences (Physical Sciences)", 34,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Geography (Geography and Environmental Science option)", "3 years", "Faculty of Natural and Agricultural Sciences (Physical Sciences)", 34,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Geoinformatics", "3 years", "Faculty of Natural and Agricultural Sciences (Physical Sciences)", 34,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Geology", "3 years", "Faculty of Natural and Agricultural Sciences (Physical Sciences)", 34,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Meteorology", "3 years", "Faculty of Natural and Agricultural Sciences (Physical Sciences)", 34,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Science in Physics", "3 years", "Faculty of Natural and Agricultural Sciences (Physical Sciences)", 34,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),

        // Extended programmes (lower minimum requirements, additional academic support)
        C("Bachelor of Science in Mathematics (Extended programme)", "4 years", "Faculty of Natural and Agricultural Sciences (Extended Programme)", 32,
            (Eng, "58%"), (Math, "65%")),
        C("Bachelor of Science in Chemistry / Geoinformatics / Geology / Meteorology / Physics (Extended programme)", "4 years", "Faculty of Natural and Agricultural Sciences (Extended Programme)", 32,
            (Eng, "58%"), (Math, "58%"), (PhysSci, "58%")),
        C("Bachelor of Science in Agriculture in Applied Plant and Soil Sciences / Plant Pathology / Ecology / Human Physiology (Extended programme)", "5 years", "Faculty of Natural and Agricultural Sciences (Extended Programme)", 30,
            (Eng, "58%"), (Math, "58%"), (PhysSci, "58%")),

        // Faculty of Veterinary Science
        C("Bachelor of Veterinary Science", "6 years", "Faculty of Veterinary Science", 35,
            (Eng, "5"), (Math, "5"), (PhysSci, "5")),
        C("Bachelor of Veterinary Nursing", "3 years", "Faculty of Veterinary Science", 28,
            (Eng, "4"), (Math, "4"), ("Physical Sciences or Life Sciences", "4")),
    };
}
