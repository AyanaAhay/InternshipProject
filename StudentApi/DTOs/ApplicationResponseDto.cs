using StudentApi.Models;

namespace StudentApi.DTOs;

public class ApplicationResponseDto
{
    public int IdStudentApplication { get; set; }

    public int IdStudent { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int? IdQuestionnaire { get; set; }

    public int? IdScheduledPractice { get; set; }

    public int? IdPracticeType { get; set; }

    public int? IdSpecialization { get; set; }

    // Enum — глобальный JsonStringEnumConverter сериализует как строку автоматически
    public StudentApplicationStatus Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // комментарий
    public string? RejectionComment { get; set; }
}