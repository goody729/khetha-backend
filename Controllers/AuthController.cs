using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Models;
using TertiaryInstitutions.Services;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new learner account and returns a JWT bearer token.
    /// </summary>
    /// <param name="request">The new learner's profile, email and password.</param>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var (outcome, response) = await _authService.RegisterAsync(request);

        if (outcome == RegisterOutcome.EmailAlreadyRegistered)
        {
            return Conflict($"A learner with email '{request.Email}' is already registered.");
        }

        return CreatedAtAction(nameof(Register), response);
    }

    /// <summary>
    /// Logs a learner in and returns a JWT bearer token.
    /// </summary>
    /// <param name="request">The learner's email and password.</param>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return response is null ? Unauthorized("Invalid email or password.") : Ok(response);
    }
}
