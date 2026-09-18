using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

/// <summary>Ask Khetha AI — the DHET career advice assistant.</summary>
[ApiController]
[Route("api/ask")]
public class AskKhethaController(KhethaAiService aiService) : ControllerBase
{
    /// <summary>Send a question to Ask Khetha AI and receive structured career guidance.</summary>
    /// <param name="request">The learner's question.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A structured reply with optional pathways and a follow-up prompt.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ChatReply), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChatReply>> Ask(
        [FromBody] ChatRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message is required.");

        var reply = await aiService.AskAsync(request.Message, ct);
        return Ok(reply);
    }
}
