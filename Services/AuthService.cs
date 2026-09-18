using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TertiaryInstitutions.Data;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Services;

public enum RegisterOutcome
{
    Success,
    EmailAlreadyRegistered
}

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<Learner> _passwordHasher;
    private readonly JwtTokenService _tokenService;

    public AuthService(AppDbContext db, IPasswordHasher<Learner> passwordHasher, JwtTokenService tokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<(RegisterOutcome Outcome, AuthResponse? Response)> RegisterAsync(RegisterRequest request)
    {
        var emailExists = await _db.Learners.AnyAsync(l => l.Email == request.Email);
        if (emailExists)
        {
            return (RegisterOutcome.EmailAlreadyRegistered, null);
        }

        var learner = new Learner
        {
            Name = request.Name,
            Email = request.Email,
            Grade = request.Grade,
            Language = request.Language,
            Track = request.Track
        };
        learner.PasswordHash = _passwordHasher.HashPassword(learner, request.Password);

        _db.Learners.Add(learner);
        await _db.SaveChangesAsync();

        return (RegisterOutcome.Success, BuildAuthResponse(learner));
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var learner = await _db.Learners.FirstOrDefaultAsync(l => l.Email == request.Email);
        if (learner is null)
        {
            return null;
        }

        var verification = _passwordHasher.VerifyHashedPassword(learner, learner.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return BuildAuthResponse(learner);
    }

    private AuthResponse BuildAuthResponse(Learner learner)
    {
        var (token, expiresAtUtc) = _tokenService.CreateToken(learner);
        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            Learner = LearnerProfileResponse.FromLearner(learner)
        };
    }
}
