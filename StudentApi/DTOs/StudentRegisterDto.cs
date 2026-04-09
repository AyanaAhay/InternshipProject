using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs;

// DTO для регистрации нового студента
// Используем DTO, чтобы не передавать целиком модель Student (из соображений безопасности)
public class StudentRegisterDto
{
    [Required]
    [MaxLength(255)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Patronymic { get; set; }

    [Required]
    public DateTime Birthdate { get; set; }

    [Required]
    [MaxLength(255)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]  // Минимальная длина пароля
    public string Password { get; set; } = string.Empty;  // Обычный пароль (не хэш)

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    public bool HadLamaPractice { get; set; } = false;
    public bool IsLamaEmployee { get; set; } = false;
}