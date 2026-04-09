using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

// Класс Student представляет студента в системе
// [Table("Student")] - указываем имя таблицы в БД
[Table("Student")]
public class Student
{
    // [Key] - указывает, что это первичный ключ
    [Key]
    // [DatabaseGenerated(DatabaseGeneratedOption.Identity)] - значение генерируется БД автоматически
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdStudent { get; set; }  // Уникальный идентификатор студента

    // [Required] - поле обязательно для заполнения
    [Required]
    [MaxLength(255)]  // Максимальная длина строки
    public string Surname { get; set; } = string.Empty;  // Фамилия (по умолчанию пустая строка)

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;  // Имя

    [MaxLength(255)]
    public string? Patronymic { get; set; }  // Отчество (может быть null, т.к. не у всех есть)


    [Required]
    // Указываем PostgreSQL, что это просто timestamp без часового пояса
    [Column(TypeName = "timestamp without time zone")]
    public DateTime Birthdate { get; set; }

    //[Required]
    //public DateTime Birthdate { get; set; }  // Дата рождения

    [Required]
    [MaxLength(255)]
    public string Login { get; set; } = string.Empty;  // Логин для входа

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;  // Хэш пароля (храним не сам пароль, а его хэш)

    [Required]
    [MaxLength(255)]
    [EmailAddress]  // Проверяет, что строка похожа на email
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Phone]  // Проверяет, что строка похожа на номер телефона
    public string PhoneNumber { get; set; } = string.Empty;

    public bool HadLamaPractice { get; set; } = false;  // Был ли ранее на практике (по умолчанию false)

    public bool IsLamaEmployee { get; set; } = false;  // Является ли сотрудником (по умолчанию false)
}