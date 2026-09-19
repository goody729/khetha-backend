using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// AI coach for term marks. Claude is given the learner's already-computed, deterministic progress
/// (marks, NSC levels, trends, career requirement gaps) and turns it into encouraging, practical
/// advice. It never sees the learner's name or email.
/// </summary>
public sealed class ProgressCoachService(HttpClient http, IOptions<AnthropicOptions> opts)
{
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonNode _toolDefinition = JsonNode.Parse("""
        {
          "name": "coach_learner",
          "description": "Give the learner coaching on their term marks using this exact structure.",
          "input_schema": {
            "type": "object",
            "properties": {
              "summary": {
                "type": "string",
                "description": "2-3 sentences: are they on track for their career goal(s), and why."
              },
              "onTrack": {
                "type": "boolean",
                "description": "True only if every career's status in the data is OnTrack."
              },
              "focusAreas": {
                "type": "array",
                "description": "Up to 3 subjects to prioritise, most important first.",
                "items": {
                  "type": "object",
                  "properties": {
                    "subject": { "type": "string" },
                    "advice":  { "type": "string", "description": "One or two practical study actions." }
                  },
                  "required": ["subject", "advice"]
                }
              },
              "encouragement": {
                "type": "string",
                "description": "One short, genuine motivating line."
              }
            },
            "required": ["summary"]
          }
        }
        """)!;

    private const string SystemPrompt = """
        You are the study coach inside the DHET Khetha platform, talking to a South African
        Grade 8-12 learner about their school term marks and whether they are on track for
        their career goal(s).

        GROUNDING
        You are given the learner's marks, NSC levels, term-on-term trends and the status of
        each career goal (OnTrack, Close, NeedsAttention, NoMarks). Base everything on that
        data only. Do not invent marks, APS scores, admission cut-offs, bursary amounts or
        deadlines. Do not contradict a career's status. Term marks are only an early
        indicator - say so if the learner seems to over- or under-read them.

        ADVICE
        Prioritise the subjects that are blocking a career goal, then subjects that are
        declining. Give small, concrete actions (past papers, a study group, asking the
        teacher, a daily 30-minute slot) rather than generic pep talk. If a goal looks out of
        reach, be honest but kind and point to related careers or a rewrite/bridging route
        rather than discouraging them. If there is no data, ask them to enter their marks.

        ESCALATION
        For personal decisions (which route to take, funding, applications) refer them to a
        DHET Career Development Practitioner: toll-free 0800 87 22 22 or WhatsApp 072 204 5056
        (Mon-Fri, 08:00-16:30).

        TONE
        Warm, clear and simple - written for a teenager. Use South African terms (matric,
        APS, Life Orientation, TVET).
        """;

    public async Task<CoachReply?> CoachAsync(TermProgressResponse progress, CancellationToken ct = default)
    {
        var apiKey = opts.Value.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }

        var body = new JsonObject
        {
            ["model"] = opts.Value.Model,
            ["max_tokens"] = 1024,
            ["system"] = SystemPrompt,
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = "Here is my progress data:\n" + JsonSerializer.Serialize(progress, _json)
                }
            },
            ["tools"] = new JsonArray { _toolDefinition.DeepClone() },
            ["tool_choice"] = new JsonObject { ["type"] = "tool", ["name"] = "coach_learner" }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "messages") { Content = JsonContent.Create(body) };
        request.Headers.Add("x-api-key", apiKey);

        using var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        if (!doc.RootElement.TryGetProperty("content", out var content))
        {
            return null;
        }

        foreach (var block in content.EnumerateArray())
        {
            if (block.TryGetProperty("type", out var type) && type.GetString() == "tool_use"
                && block.TryGetProperty("input", out var input))
            {
                return JsonSerializer.Deserialize<CoachReply>(input.GetRawText(), _json);
            }
        }

        return null;
    }
}
