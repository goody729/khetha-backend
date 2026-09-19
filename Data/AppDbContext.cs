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
    public DbSet<SavedCareer> SavedCareers => Set<SavedCareer>();
    public DbSet<JourneyProgress> JourneyProgresses => Set<JourneyProgress>();
    public DbSet<TermResult> TermResults => Set<TermResult>();
    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();
    public DbSet<ReminderLog> ReminderLogs => Set<ReminderLog>();

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

        modelBuilder.Entity<SavedCareer>(b =>
        {
            b.HasOne<Learner>()
                .WithMany()
                .HasForeignKey(s => s.LearnerId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(s => new { s.LearnerId, s.CareerId }).IsUnique();
        });

        modelBuilder.Entity<TermResult>(b =>
        {
            b.Property(r => r.Subject).IsRequired().HasMaxLength(100);
            b.HasOne<Learner>()
                .WithMany()
                .HasForeignKey(r => r.LearnerId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(r => new { r.LearnerId, r.Year, r.Term, r.Subject }).IsUnique();
            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_TermResult_Term_Range", "\"Term\" >= 1 AND \"Term\" <= 4");
                t.HasCheckConstraint("CK_TermResult_Percentage_Range", "\"Percentage\" >= 0 AND \"Percentage\" <= 100");
            });
        });

        modelBuilder.Entity<DeviceToken>(b =>
        {
            b.Property(d => d.Token).IsRequired().HasMaxLength(512);
            b.Property(d => d.Platform).IsRequired().HasMaxLength(20);
            b.HasOne<Learner>()
                .WithMany()
                .HasForeignKey(d => d.LearnerId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(d => d.Token).IsUnique();
        });

        modelBuilder.Entity<ReminderLog>(b =>
        {
            b.HasOne<Learner>()
                .WithMany()
                .HasForeignKey(l => l.LearnerId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(l => new { l.LearnerId, l.ForYear, l.ForTerm, l.SentOn }).IsUnique();
        });

        modelBuilder.Entity<JourneyProgress>(b =>
        {
            b.HasKey(j => j.LearnerId);
            b.HasOne<Learner>()
                .WithOne()
                .HasForeignKey<JourneyProgress>(j => j.LearnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
