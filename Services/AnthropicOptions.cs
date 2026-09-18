namespace TertiaryInstitutions.Services;

public sealed class AnthropicOptions
{
    public const string Section = "Anthropic";

    /// <summary>Anthropic API key. Set via user-secrets or environment variable.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Model ID to use. Defaults to claude-sonnet-4-6.</summary>
    public string Model { get; set; } = "claude-sonnet-4-6";
}
