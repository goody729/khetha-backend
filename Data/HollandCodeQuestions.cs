using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Data;

/// <summary>
/// Job-fit quiz questions for the Holland Code (RIASEC) career-interest assessment. Each question is
/// rated by the learner on a 1 (strongly disagree) to 5 (strongly agree) scale and is tagged with the
/// single RIASEC type it measures. 6 questions per type (36 total) is enough to produce a meaningful
/// top-3 result code while keeping the quiz short.
/// </summary>
public static class HollandCodeQuestions
{
    private static HollandCodeQuestion Q(int id, string text, RiasecType type)
        => new() { Id = id, Text = text, Type = type };

    public static readonly IReadOnlyList<HollandCodeQuestion> All = BuildAll();

    private static List<HollandCodeQuestion> BuildAll() => new()
    {
        // Realistic
        Q(1, "I like building or repairing things with my hands.", RiasecType.Realistic),
        Q(2, "I enjoy working with tools, machines, or equipment.", RiasecType.Realistic),
        Q(3, "I'd rather be outdoors doing physical work than sitting at a desk.", RiasecType.Realistic),
        Q(4, "I like taking things apart to see how they work.", RiasecType.Realistic),
        Q(5, "I enjoy sports, farming, or other hands-on physical activities.", RiasecType.Realistic),
        Q(6, "I prefer practical, concrete tasks over abstract discussions.", RiasecType.Realistic),

        // Investigative
        Q(7, "I enjoy analyzing data to look for patterns.", RiasecType.Investigative),
        Q(8, "I like solving complex problems and puzzles.", RiasecType.Investigative),
        Q(9, "I'm curious about how and why things work the way they do.", RiasecType.Investigative),
        Q(10, "I enjoy conducting experiments or research.", RiasecType.Investigative),
        Q(11, "I like reading about science, technology, or new discoveries.", RiasecType.Investigative),
        Q(12, "I prefer to figure things out logically before acting.", RiasecType.Investigative),

        // Artistic
        Q(13, "I enjoy creative writing, art, music, or drama.", RiasecType.Artistic),
        Q(14, "I like coming up with original ideas.", RiasecType.Artistic),
        Q(15, "I prefer work that lets me express myself.", RiasecType.Artistic),
        Q(16, "I enjoy designing things (visuals, spaces, media, fashion).", RiasecType.Artistic),
        Q(17, "I dislike strict rules and prefer flexible, open-ended tasks.", RiasecType.Artistic),
        Q(18, "I often notice and appreciate beauty or aesthetics in everyday things.", RiasecType.Artistic),

        // Social
        Q(19, "I enjoy helping, teaching, or caring for other people.", RiasecType.Social),
        Q(20, "I like working in teams and getting to know people.", RiasecType.Social),
        Q(21, "I'm good at listening to and understanding others' feelings.", RiasecType.Social),
        Q(22, "I enjoy volunteering or community-focused activities.", RiasecType.Social),
        Q(23, "I find it rewarding to support someone through a problem.", RiasecType.Social),
        Q(24, "I prefer talking to and working with people over working alone.", RiasecType.Social),

        // Enterprising
        Q(25, "I enjoy persuading or influencing other people.", RiasecType.Enterprising),
        Q(26, "I like taking the lead and organizing others.", RiasecType.Enterprising),
        Q(27, "I'm comfortable taking risks to achieve a goal.", RiasecType.Enterprising),
        Q(28, "I enjoy competition and working toward ambitious targets.", RiasecType.Enterprising),
        Q(29, "I like the idea of starting or running my own business.", RiasecType.Enterprising),
        Q(30, "I enjoy public speaking or selling ideas to others.", RiasecType.Enterprising),

        // Conventional
        Q(31, "I like following clear procedures and organized systems.", RiasecType.Conventional),
        Q(32, "I enjoy working with numbers, records, or detailed data.", RiasecType.Conventional),
        Q(33, "I prefer structured tasks with clear instructions.", RiasecType.Conventional),
        Q(34, "I'm careful and accurate when checking details.", RiasecType.Conventional),
        Q(35, "I like keeping things organized, filed, and up to date.", RiasecType.Conventional),
        Q(36, "I feel comfortable working within set rules and routines.", RiasecType.Conventional),
    };
}
