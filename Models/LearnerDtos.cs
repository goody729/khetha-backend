using System.ComponentModel.DataAnnotations;

namespace TertiaryInstitutions.Models;

public class RegisterRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Range(8, 12)]
    public int Grade { get; set; }

    [Required]
    public string Language { get; set; } = string.Empty;

    [Required]
    public string Track { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class UpdateLearnerProfileRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(8, 12)]
    public int Grade { get; set; }

    [Required]
    public string Language { get; set; } = string.Empty;

    [Required]
    public string Track { get; set; } = string.Empty;
}

public class LearnerProfileResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Track { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public static LearnerProfileResponse FromLearner(Learner learner) => new()
    {
        Id = learner.Id,
        Name = learner.Name,
        Email = learner.Email,
        Grade = learner.Grade,
        Language = learner.Language,
        Track = learner.Track,
        CreatedAtUtc = learner.CreatedAtUtc
    };
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public LearnerProfileResponse Learner { get; set; } = new();
}
