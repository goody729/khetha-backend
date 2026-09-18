namespace TertiaryInstitutions.Models;

/// <summary>Incoming question from the learner.</summary>
/// <param name="Message">The learner's question.</param>
public record ChatRequest(string Message);

/// <summary>A single career or study option returned by the AI.</summary>
/// <param name="Title">Short name of the pathway.</param>
/// <param name="Description">One-sentence description.</param>
public record Pathway(string Title, string Description);

/// <summary>Structured reply from Ask Khetha AI.</summary>
/// <param name="Text">Main answer (2–4 sentences).</param>
/// <param name="Pathways">Optional concrete career/study options.</param>
/// <param name="FollowUp">Optional suggested next question.</param>
public record ChatReply(
    string Text,
    IReadOnlyList<Pathway>? Pathways = null,
    string? FollowUp = null
);
