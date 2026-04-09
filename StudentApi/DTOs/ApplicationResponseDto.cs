namespace StudentApi.DTOs;

// DTO для ответа с данными заявки (без чувствительной информации)
public class ApplicationResponseDto
{
    public int IdStudentApplication { get; set; }
    public int IdStudent { get; set; }
    public string StudentName { get; set; } = string.Empty;  // Полное имя студента для удобства
    public int? IdScheduledPractice { get; set; }
    public int? IdPracticeType { get; set; }
    public int? IdSpecialization { get; set; }
    public string Status { get; set; } = string.Empty;  // Статус в виде строки (для фронта)
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}