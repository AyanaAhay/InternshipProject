namespace StudentApi.DTOs;

// DTO для ответа с данными студента (без пароля)
public class StudentResponseDto
{
    public int IdStudent { get; set; }
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public DateTime Birthdate { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool HadLamaPractice { get; set; }
    public bool IsLamaEmployee { get; set; }
}