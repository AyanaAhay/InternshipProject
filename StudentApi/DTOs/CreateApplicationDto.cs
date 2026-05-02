using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs;

public class CreateApplicationDto
{
    [Required]
    public int IdStudent { get; set; }

    // НОВОЕ: можно прикрепить существующую анкету
    public int? IdQuestionnaire { get; set; }

    // Необязательное — студент может не выбирать из расписания
    //[Required]
    public int? IdScheduledPractice { get; set; }

    [Required]
    public int IdPracticeType { get; set; }

    [Required]
    public int IdSpecialization { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}