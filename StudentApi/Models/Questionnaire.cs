using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

/// <summary>
/// Модель анкеты студента.
/// Связь: Student (1) → Questionnaire (N) - один студент может иметь много анкет.
/// Связь: Questionnaire (1) → StudentApplication (N) - одна анкета может использоваться в многих заявках.
/// </summary>
[Table("Questionnaire")]
public class Questionnaire
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdQuestionnaire { get; set; }

    // Прямая связь со студентом (анкета принадлежит студенту, а не заявке)
    [Required]
    public int IdStudent { get; set; }

    [ForeignKey(nameof(IdStudent))]
    public virtual Student? Student { get; set; }

    // Основная информация
    [Required]
    [MaxLength(255)]
    public string Citizenship { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Birthplace { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "text")]
    public string SourceInfo { get; set; } = string.Empty;

    // Адреса
    [Required]
    [Column(TypeName = "text")]
    public string Residency { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "text")]
    public string RegistrationPlace { get; set; } = string.Empty;

    // Готовность
    public bool VacationSideJob { get; set; }
    public bool VolunteeringReadiness { get; set; }

    // Юридическая информация
    public bool CriminalLiability { get; set; }
    public bool AdminLiability { get; set; }

    // Медицинская информация
    [Column(TypeName = "text")]
    public string? ChronicConditions { get; set; }

    [Column(TypeName = "text")]
    public string? MedContraindications { get; set; }

    // Согласие на обработку данных
    public bool DataProcessingConsent { get; set; }

    // НОВОЕ
    public int[]? DesiredPracticeAreaIds { get; set; } 
    
    // Выбранные направления
    [MaxLength(255)] 
    public string? OtherDesiredPracticeArea { get; set; } 
    
    // Другое направление
    [Column(TypeName = "text")] 
    public string? WhatToLearn { get; set; } 
    
    // Чему хочет научиться
    [Column(TypeName = "text")] 
    public string? PracticeWishes { get; set; } 
    
    // Пожелания
    [Column(TypeName = "text")] 
    public string? ThesisTopic { get; set; } // Тема диплома

    // Аудит
    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Навигационные свойства для связанных таблиц
    public virtual PsychologicalQuestions? PsychologicalQuestions { get; set; }
    public virtual ICollection<Relative> Relatives { get; set; } = new List<Relative>();
    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();
    public virtual ICollection<PlacePractice> PlacePractices { get; set; } = new List<PlacePractice>();
    public virtual ICollection<PlaceWork> PlaceWorks { get; set; } = new List<PlaceWork>();
    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public virtual ICollection<StudentProject> StudentProjects { get; set; } = new List<StudentProject>();
    public virtual ICollection<PracticePriority> PracticePriorities { get; set; } = new List<PracticePriority>();
}