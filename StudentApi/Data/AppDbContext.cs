using Microsoft.EntityFrameworkCore;
using StudentApi.Models;

namespace StudentApi.Data;

// AppDbContext - это класс, через который мы работаем с базой данных
// Он наследуется от DbContext (базовый класс Entity Framework)
public class AppDbContext : DbContext
{
    // Конструктор принимает настройки подключения и передаёт их базовому классу
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet<T> представляет таблицу в базе данных
    // Через эти свойства мы будем выполнять запросы: _context.Students.ToListAsync() и т.д.
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentApplication> StudentApplications { get; set; }

    // Метод OnModelCreating вызывается при создании модели БД
    // Здесь можно дополнительно настроить связи между таблицами
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка связи между StudentApplication и Student
        modelBuilder.Entity<StudentApplication>()
            // Одна заявка принадлежит одному студенту
            .HasOne(sa => sa.Student)
            // У одного студента может быть много заявок
            .WithMany()
            // Внешний ключ - IdStudent
            .HasForeignKey(sa => sa.IdStudent)
            // При удалении студента - удаляем его заявки (каскадное удаление)
            .OnDelete(DeleteBehavior.Cascade);

        // Индекс для быстрого поиска по статусу (ускоряет запросы)
        modelBuilder.Entity<StudentApplication>()
            .HasIndex(sa => sa.StudentApplicationStatus);

        // Индекс для быстрого поиска по студенту
        modelBuilder.Entity<StudentApplication>()
            .HasIndex(sa => sa.IdStudent);

        // Уникальный индекс на логин (не может быть двух студентов с одинаковым логином)
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Login)
            .IsUnique();

        // Уникальный индекс на email
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();
    }
}