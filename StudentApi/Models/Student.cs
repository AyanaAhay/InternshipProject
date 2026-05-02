using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

/// <summary>
/// Модель студента.
/// Связи:
/// - Student (1) → StudentApplication (N) - один студент может иметь много заявок
/// - Student (1) → Questionnaire (N) - один студент может иметь много анкет
/// </summary>
[Table("Student")]
public class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdStudent { get; set; }

    // ФИО
    [Required]
    [MaxLength(255)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Patronymic { get; set; }

    // Дата рождения
    [Required]
    [Column(TypeName = "timestamp without time zone")]
    public DateTime Birthdate { get; set; }

    // Учетные данные
    [Required]
    [MaxLength(255)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    // Контакты
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    // Флаги
    public bool HadLamaPractice { get; set; } = false;
    public bool IsLamaEmployee { get; set; } = false;

    // Навигационные свойства
    public virtual ICollection<StudentApplication> Applications { get; set; } = new List<StudentApplication>();
    public virtual ICollection<Questionnaire> Questionnaires { get; set; } = new List<Questionnaire>();
    public virtual ICollection<StudentDocument> Documents { get; set; } = new List<StudentDocument>();
}