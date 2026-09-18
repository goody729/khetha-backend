using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

public sealed class KhethaAiService(HttpClient http, IOptions<AnthropicOptions> opts)
{
    private static readonly JsonSerializerOptions _deserializeOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Reused across requests — DeepClone() per call keeps it safe.
    private static readonly JsonNode _toolDefinition = JsonNode.Parse("""
        {
          "name": "reply_to_learner",
          "description": "Format your reply to the learner using this exact structure.",
          "input_schema": {
            "type": "object",
            "properties": {
              "text": {
                "type": "string",
                "description": "Main answer, 2-4 sentences, encouraging and clear."
              },
              "pathways": {
                "type": "array",
                "description": "Concrete career or study options, only when genuinely relevant.",
                "items": {
                  "type": "object",
                  "properties": {
                    "title":       { "type": "string" },
                    "description": { "type": "string" }
                  },
                  "required": ["title", "description"]
                }
              },
              "followUp": {
                "type": "string",
                "description": "One natural next question to keep the conversation going."
              }
            },
            "required": ["text"]
          }
        }
        """)!;

    private const string SystemPrompt = """
        You are "Ask Khetha AI," the official AI career advisor for the DHET (Department of
        Higher Education and Training) Khetha platform. You help South African Grade 10–12
        learners and school-leavers make decisions about subjects, APS scores, career paths,
        TVET colleges, universities, and funding.

        SCOPE
        Only answer questions about: subject choices, APS calculation, career/qualification
        pathways, TVET and university admission requirements, NSFAS and other bursaries/SETAs,
        matric rewrites (Second Chance Programme), and related South African education policy.
        If a question falls outside this scope, say so briefly and redirect the learner to ask
        about their studies or career instead.

        ACCURACY
        Do not invent specific numbers — bursary amounts, APS cut-offs, deadlines, institution
        names, or eligibility thresholds — unless they are provided to you in the request
        context. These change yearly and learners will act on what you say. If you don't have
        grounded, current information for a specific claim, say that clearly and direct the
        learner to the human helpline below rather than guessing.

        ESCALATION
        For anything requiring personalized, binding, or highly specific guidance (individual
        eligibility decisions, application status, disputes), direct the learner to a certified
        DHET Career Development Practitioner: toll-free 0800 87 22 22 or WhatsApp 072 204 5056
        (Mon–Fri, 08:00–16:30).

        TONE
        Encouraging, clear, and simple — written for a teenager, not a policy document. Use
        South African terms (APS, matric, Grade 12, TVET, Life Orientation, NSFAS, rand) and
        avoid jargon without explanation.
        """;

    private static readonly ChatReply _fallback = new(
        "I'm having trouble answering right now. Please try again shortly, or call the " +
        "DHET helpline toll-free on 0800 87 22 22 (Mon–Fri, 08:00–16:30).");

    public async Task<ChatReply> AskAsync(string message, CancellationToken ct = default)
    {
        var apiKey = opts.Value.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                "Anthropic API key is not configured. " +
                "Run: dotnet user-secrets set \"Anthropic:ApiKey\" \"sk-ant-...\"");

        var body = new JsonObject
        {
            ["model"]       = opts.Value.Model,
            ["max_tokens"]  = 1024,
            ["system"]      = SystemPrompt,
            ["messages"]    = new JsonArray
            {
                new JsonObject { ["role"] = "user", ["content"] = message }
            },
            ["tools"]       = new JsonArray { _toolDefinition.DeepClone() },
            ["tool_choice"] = new JsonObject { ["type"] = "tool", ["name"] = "reply_to_learner" }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "messages")
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Add("x-api-key", apiKey);

        using var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));

        var toolInput = FindToolInput(doc);
        if (toolInput is null)
            return _fallback;

        return JsonSerializer.Deserialize<ChatReply>(toolInput.Value.GetRawText(), _deserializeOpts)
               ?? _fallback;
    }

    private static JsonElement? FindToolInput(JsonDocument doc)
    {
        if (!doc.RootElement.TryGetProperty("content", out var content))
            return null;

        foreach (var block in content.EnumerateArray())
        {
            if (block.TryGetProperty("type", out var type) &&
                type.GetString() == "tool_use" &&
                block.TryGetProperty("input", out var input))
            {
                return input;
            }
        }

        return null;
    }
}
