namespace TertiaryInstitutions.Services;

/// <summary>
/// Delivers a push notification to one device. Swap the registered implementation for a real
/// provider (e.g. Firebase Cloud Messaging, which also covers APNs for iOS) without touching the
/// reminder logic.
/// </summary>
public interface IPushSender
{
    Task SendAsync(string deviceToken, string platform, string title, string body,
        IReadOnlyDictionary<string, string> data, CancellationToken ct = default);
}

/// <summary>
/// Default sender: only logs. No push provider credentials are configured yet, so reminders are
/// computed and recorded but nothing reaches a phone until a real <see cref="IPushSender"/> is registered.
/// </summary>
public sealed class LoggingPushSender(ILogger<LoggingPushSender> logger) : IPushSender
{
    public Task SendAsync(string deviceToken, string platform, string title, string body,
        IReadOnlyDictionary<string, string> data, CancellationToken ct = default)
    {
        logger.LogInformation("PUSH (not delivered - no provider configured) [{Platform}] {Title}: {Body}", platform, title, body);
        return Task.CompletedTask;
    }
}
