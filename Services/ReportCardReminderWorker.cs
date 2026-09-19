using Microsoft.EntityFrameworkCore;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Background job that sends the "update your report card" push notification on each reminder date
/// (the day schools reopen, then monthly through the term) to learners who haven't yet entered the
/// marks that are due. Checks hourly and sends from 08:00 SAST; <see cref="ReminderLog"/> guarantees
/// at most one reminder per learner per day even across restarts.
/// </summary>
public sealed class ReportCardReminderWorker(
    IServiceScopeFactory scopes, ILogger<ReportCardReminderWorker> logger) : BackgroundService
{
    private const int SendFromHourSast = 8;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Report-card reminder run failed; will retry next hour.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    internal async Task RunOnceAsync(CancellationToken ct)
    {
        var now = ReportCardReminderPlanner.NowSast();
        if (now.Hour < SendFromHourSast)
        {
            return;
        }

        var today = DateOnly.FromDateTime(now);
        var due = ReportCardReminderPlanner.DueOn(today);
        if (due.Count == 0)
        {
            return;
        }

        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sender = scope.ServiceProvider.GetRequiredService<IPushSender>();

        foreach (var reminder in due)
        {
            var year = reminder.ForYear;
            var term = reminder.ForTerm;

            var devices = await db.DeviceTokens
                .Where(d => !db.TermResults.Any(r => r.LearnerId == d.LearnerId && r.Year == year && r.Term == term)
                         && !db.ReminderLogs.Any(l => l.LearnerId == d.LearnerId && l.ForYear == year
                                                      && l.ForTerm == term && l.SentOn == today))
                .ToListAsync(ct);

            var data = new Dictionary<string, string>
            {
                ["type"] = "report_card_reminder",
                ["year"] = year.ToString(),
                ["term"] = term.ToString()
            };

            foreach (var learnerDevices in devices.GroupBy(d => d.LearnerId))
            {
                var delivered = false;
                foreach (var device in learnerDevices)
                {
                    try
                    {
                        await sender.SendAsync(device.Token, device.Platform, reminder.Title, reminder.Message, data, ct);
                        delivered = true;
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        logger.LogWarning(ex, "Push to a device of learner {LearnerId} failed.", learnerDevices.Key);
                    }
                }

                if (delivered)
                {
                    db.ReminderLogs.Add(new ReminderLog
                    {
                        LearnerId = learnerDevices.Key, ForYear = year, ForTerm = term, SentOn = today
                    });
                }
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
