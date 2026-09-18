using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

/// <summary>
/// Issues signed JWT bearer tokens for authenticated learners.
/// </summary>
public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAtUtc) CreateToken(Learner learner)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expiryMinutes = jwtSection.GetValue<int?>("ExpiryMinutes") ?? 60;

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, learner.Id.ToString()),
            new(ClaimTypes.NameIdentifier, learner.Id.ToString()),
            new(ClaimTypes.Email, learner.Email),
            new(ClaimTypes.Name, learner.Name)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
