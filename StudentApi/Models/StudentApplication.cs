using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

// Перечисление статусов заявки студента (на русском)
public enum StudentApplicationStatus
{
    Заявка,                      // Студент создал, но не отправил
    НаРассмотренииМенеджером,      // Отправлена менеджеру
    Тестирование,                  // Менеджер назначил тест
    СобеседованиеСМенеджером,      // Назначено собеседование
    НаРассмотренииРуководителем,   // Передана руководителю
    СобеседованиеСРуководителем,   // Собеседование с руководителем
    ОформлениеДокументов,          // Нужно оформить документы
    Принят,                        // Принят на практику
    Отказано                       // Отказано
}

[Table("StudentApplication")]
public class StudentApplication
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdStudentApplication { get; set; }  // Уникальный ID заявки

    [Required]
    public int IdStudent { get; set; }  // ID студента (внешний ключ)

    // [ForeignKey(nameof(IdStudent))] - указываем, что это внешний ключ к таблице Student
    [ForeignKey(nameof(IdStudent))]
    public virtual Student? Student { get; set; }  // Навигационное свойство (чтобы получить данные студента)

    // Временно поля для ID других сущностей (потом будут получаться из других сервисов)
    public int? IdScheduledPractice { get; set; }  // ID практики из расписания (пока может быть null)
    public int? IdPracticeType { get; set; }  // ID типа практики
    public int? IdSpecialization { get; set; }  // ID специализации

    [Required]
    public StudentApplicationStatus StudentApplicationStatus { get; set; } = StudentApplicationStatus.Заявка;  // Статус (по умолчанию - Заявка)

    [Required]
    [Column(TypeName = "timestamp without time zone")]
    public DateTime StartDate { get; set; }  // Желаемая дата начала практики

    [Required]
    [Column(TypeName = "timestamp without time zone")]
    public DateTime EndDate { get; set; }  // Желаемая дата окончания

    // Дата создания заявки (автоматически устанавливается при создании)
    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Дата обновления заявки
    [Column(TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}