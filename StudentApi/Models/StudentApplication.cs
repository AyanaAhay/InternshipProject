using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentApi.Contracts.Enums;

namespace StudentApi.Models;

/// <summary>
/// Модель заявки студента на практику
/// Связи:
/// - Student (1) → StudentApplication (N) - один студент может иметь много заявок
/// - Questionnaire (1) → StudentApplication (N) - одна анкета может использоваться в многих заявках
/// </summary>
[Table("StudentApplication")]
public class StudentApplication
{
    /// <summary>
    /// Уникальный идентификатор заявки (первичный ключ)
    /// Генерируется автоматически базой данных
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdStudentApplication { get; set; }

    /// <summary>
    /// Внешний ключ на таблицу Student
    /// Указывает, какому студенту принадлежит заявка
    /// </summary>
    [Required]
    public int IdStudent { get; set; }

    /// <summary>
    /// Навигационное свойство для связи со студентом
    /// Позволяет получить полные данные студента через Include()
    /// </summary>
    [ForeignKey(nameof(IdStudent))]
    public virtual Student? Student { get; set; }

    /// <summary>
    /// Внешний ключ на таблицу Questionnaire
    /// Может быть NULL, если анкета еще не прикреплена
    /// Одна анкета может быть использована в нескольких заявках
    /// </summary>
    public int? IdQuestionnaire { get; set; }

    /// <summary>
    /// Навигационное свойство для связи с анкетой
    /// Позволяет получить данные прикрепленной анкеты
    /// </summary>
    [ForeignKey(nameof(IdQuestionnaire))]
    public virtual Questionnaire? Questionnaire { get; set; }

    /// <summary>
    /// ID запланированной практики из расписания (справочник из Manager API)
    /// </summary>
    public int? IdScheduledPractice { get; set; }

    /// <summary>
    /// ID типа практики (справочник из Manager API)
    /// Например: "Производственная", "Преддипломная"
    /// </summary>
    public int? IdPracticeType { get; set; }

    /// <summary>
    /// ID специализации (справочник из Manager API)
    /// Например: "Программирование", "Тестирование"
    /// </summary>
    public int? IdSpecialization { get; set; }

    /// <summary>
    /// Текущий статус заявки
    /// По умолчанию - Draft (Черновик)
    /// </summary>
    [Required]
    public StudentApplicationStatus StudentApplicationStatus { get; set; } = StudentApplicationStatus.Draft;

    /// <summary>
    /// Желаемая дата начала практики
    /// </summary>
    [Required]
    [Column(TypeName = "timestamp without time zone")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Желаемая дата окончания практики
    /// </summary>
    [Required]
    [Column(TypeName = "timestamp without time zone")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Дата создания заявки
    /// Автоматически устанавливается при создании
    /// </summary>
    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Дата последнего обновления заявки
    /// Обновляется при каждом изменении
    /// </summary>
    [Column(TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Комментарий к отказу (от менеджера или руководителя)
    [Column(TypeName = "text")] 
    public string? RejectionComment { get; set; }

    public virtual ICollection<StudentDocument> Documents { get; set; } = new List<StudentDocument>();
    // Навигационное свойство — отзыв (может быть null)
    public virtual PracticeReview? PracticeReview { get; set; }
}