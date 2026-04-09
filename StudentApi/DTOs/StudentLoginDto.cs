using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs;

// DTO для входа в систему
public class StudentLoginDto
{
    [Required]
    public string Login { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}