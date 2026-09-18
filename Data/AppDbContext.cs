using Microsoft.EntityFrameworkCore;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Data;

/// <summary>
/// EF Core context for the project's persisted data (learner accounts and their assessment
/// results). Reference data such as universities, courses and subjects intentionally stays
/// static/in-memory and is not part of this context.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Learner> Learners => Set<Learner>();
    public DbSet<AssessmentSubmission> AssessmentSubmissions => Set<AssessmentSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Learner>(b =>
        {
            b.Property(l => l.Name).IsRequired().HasMaxLength(200);
            b.Property(l => l.Email).IsRequired().HasMaxLength(256);
            b.HasIndex(l => l.Email).IsUnique();
            b.Property(l => l.PasswordHash).IsRequired();
            b.Property(l => l.Language).HasMaxLength(50);
            b.Property(l => l.Track).HasMaxLength(50);
            b.ToTable(t => t.HasCheckConstraint("CK_Learner_Grade_Range", "\"Grade\" >= 8 AND \"Grade\" <= 12"));
        });

        modelBuilder.Entity<AssessmentSubmission>(b =>
        {
            b.HasOne<Learner>()
                .WithMany(l => l.AssessmentSubmissions)
                .HasForeignKey(a => a.LearnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
