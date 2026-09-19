using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Controllers;

/// <summary>
/// Register the learner's device so the app can send push notifications, e.g. the report-card
/// reminders sent when schools reopen and monthly after that.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(AppDbContext db) : ControllerBase
{
    /// <summary>
    /// Registers a device token for the signed-in learner. Idempotent; a token already registered to
    /// another learner (e.g. a shared phone) is moved to the current one.
    /// </summary>
    [HttpPost("devices")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request, CancellationToken ct)
    {
        var learnerId = GetLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var device = await db.DeviceTokens.FirstOrDefaultAsync(d => d.Token == request.Token, ct);
        if (device is null)
        {
            db.DeviceTokens.Add(new DeviceToken
            {
                LearnerId = learnerId.Value, Token = request.Token, Platform = request.Platform
            });
        }
        else
        {
            device.LearnerId = learnerId.Value;
            device.Platform = request.Platform;
            device.RegisteredAtUtc = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>
    /// Unregisters a device token (e.g. on sign-out) so it stops receiving notifications. Idempotent.
    /// </summary>
    /// <param name="token">The device token to remove.</param>
    [HttpDelete("devices")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnregisterDevice([FromQuery] string token, CancellationToken ct)
    {
        var learnerId = GetLearnerId();
        if (learnerId is null)
        {
            return Unauthorized();
        }

        var device = await db.DeviceTokens.FirstOrDefaultAsync(d => d.Token == token && d.LearnerId == learnerId.Value, ct);
        if (device is not null)
        {
            db.DeviceTokens.Remove(device);
            await db.SaveChangesAsync(ct);
        }

        return NoContent();
    }

    private Guid? GetLearnerId() =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
