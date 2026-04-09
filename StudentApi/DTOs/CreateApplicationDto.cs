using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs;

// DTO для создания заявки студента
public class CreateApplicationDto
{
    [Required]
    public int IdStudent { get; set; }  // ID студента, создающего заявку

    //public int? IdScheduledPractice { get; set; }  // Пока опционально

    //public int? IdPracticeType { get; set; }

    //public int? IdSpecialization { get; set; }

    // Сделать поля обязательными (были nullable)
    [Required]
    public int IdScheduledPractice { get; set; }

    [Required]
    public int IdPracticeType { get; set; }

    [Required]
    public int IdSpecialization { get; set; }

    [Required]
    public DateTime StartDate { get; set; }  // Желаемая дата начала

    [Required]
    public DateTime EndDate { get; set; }  // Желаемая дата окончания
}