using Microsoft.EntityFrameworkCore;
using StudentApi.Models;

namespace StudentApi.Data;

/// <summary>
/// Контекст базы данных Entity Framework Core.
/// Содержит DbSet для всех таблиц и конфигурацию связей.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Таблицы базы данных
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentApplication> StudentApplications { get; set; }
    public DbSet<Questionnaire> Questionnaires { get; set; }
    public DbSet<PsychologicalQuestions> PsychologicalQuestions { get; set; }
    public DbSet<Relative> Relatives { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<PlacePractice> PlacePractices { get; set; }
    public DbSet<PlaceWork> PlaceWorks { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<StudentProject> StudentProjects { get; set; }
    public DbSet<PracticePriority> PracticePriorities { get; set; }
    // НОВОЕ
    public DbSet<StudentDocument> StudentDocuments { get; set; }
    public DbSet<PracticeReview> PracticeReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========== Конфигурация StudentApplication ==========

        // Enum хранится как строка в БД (читаемо и безопасно при изменении enum)
        modelBuilder.Entity<StudentApplication>()
            .Property(a => a.StudentApplicationStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Enum статусов документа как строки
        modelBuilder.Entity<StudentDocument>()
            .Property(d => d.UploadStatus)
            .HasConversion<string>()
            .HasMaxLength(30); 
        
        modelBuilder.Entity<StudentDocument>()
            .Property(d => d.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(30); 

        modelBuilder.Entity<StudentDocument>()
            .Property(d => d.ContractStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        // Студент → Заявки (1:N). При удалении студента удаляются все его заявки.
        modelBuilder.Entity<StudentApplication>()
            .HasOne(a => a.Student)
            .WithMany(s => s.Applications)
            .HasForeignKey(a => a.IdStudent)
            .OnDelete(DeleteBehavior.Cascade);

        // Заявка → Анкета (N:1). Много заявок могут ссылаться на одну анкету.
        // При удалении анкеты IdQuestionnaire в заявке становится NULL.
        modelBuilder.Entity<StudentApplication>()
            .HasOne(a => a.Questionnaire)
            .WithMany()
            .HasForeignKey(a => a.IdQuestionnaire)
            .OnDelete(DeleteBehavior.SetNull);

        // ========== Конфигурация Questionnaire ==========

        // Анкета → Студент (N:1). Анкета принадлежит студенту.
        // При удалении студента удаляются все его анкеты.
        modelBuilder.Entity<Questionnaire>()
            .HasOne(q => q.Student)
            .WithMany(s => s.Questionnaires)
            .HasForeignKey(q => q.IdStudent)
            .OnDelete(DeleteBehavior.Cascade);

        // Документ → Заявка
        modelBuilder.Entity<StudentDocument>()
            .HasOne(d => d.StudentApplication)
            .WithMany(a => a.Documents)
            .HasForeignKey(d => d.IdStudentApplication)
            .OnDelete(DeleteBehavior.Cascade); 
        
        // Документ → Студент
        modelBuilder.Entity<StudentDocument>()
            .HasOne(d => d.Student)
            .WithMany(s => s.Documents)
            .HasForeignKey(d => d.IdStudent)
            .OnDelete(DeleteBehavior.Cascade);


        // ========== Конфигурация PsychologicalQuestions ==========

        // Связь 1:1 с Questionnaire. При удалении анкеты удаляются и психологические вопросы.
        modelBuilder.Entity<PsychologicalQuestions>()
            .HasOne(p => p.Questionnaire)
            .WithOne(q => q.PsychologicalQuestions)
            .HasForeignKey<PsychologicalQuestions>(p => p.IdQuestionnaire)
            .OnDelete(DeleteBehavior.Cascade);

        // ========== Конфигурация Student ==========

        // Уникальный индекс на поле Login
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Login)
            .IsUnique();


        // Отзыв → Заявка (1:1, отзыв необязателен)
        modelBuilder.Entity<PracticeReview>() 
            .HasOne(r => r.StudentApplication) 
            .WithOne(a => a.PracticeReview) 
            .HasForeignKey<PracticeReview>(r => r.IdStudentApplication) 
            .OnDelete(DeleteBehavior.Cascade); 
        // Уникальность: один отзыв на одну заявку
        modelBuilder.Entity<PracticeReview>()
            .HasIndex(r => r.IdStudentApplication)
            .IsUnique();
    }
}