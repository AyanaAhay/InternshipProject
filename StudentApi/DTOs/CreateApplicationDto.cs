using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs;

public class CreateApplicationDto
{
    [Required]
    public int IdStudent { get; set; }

    [Required]  // ← было int?, стало int
    public int IdScheduledPractice { get; set; }

    [Required]  // ← было int?, стало int
    public int IdPracticeType { get; set; }

    [Required]  // ← было int?, стало int
    public int IdSpecialization { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}