using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace StudentApi.Services;

// Сервис для аутентификации (регистрация и вход)
public class AuthService
{
    private readonly AppDbContext _context;

    // Внедрение зависимости через конструктор
    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    // Хэширование пароля (безопасное хранение)
    // В реальном проекте лучше использовать BCrypt или Identity Framework
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    // Регистрация нового студента
    public async Task<StudentResponseDto?> RegisterAsync(StudentRegisterDto dto)
    {
        // Проверяем, не существует ли уже пользователь с таким логином
        var existingStudent = await _context.Students
            .FirstOrDefaultAsync(s => s.Login == dto.Login);

        if (existingStudent != null)
        {
            return null;  // Логин уже занят
        }

        // Создаём нового студента
        var student = new Student
        {
            Surname = dto.Surname,
            Name = dto.Name,
            Patronymic = dto.Patronymic,
            Birthdate = dto.Birthdate,
            Login = dto.Login,
            PasswordHash = HashPassword(dto.Password),  // Храним только хэш!
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            HadLamaPractice = dto.HadLamaPractice,
            IsLamaEmployee = dto.IsLamaEmployee
        };

        // Добавляем в базу
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Возвращаем DTO (без пароля)
        return new StudentResponseDto
        {
            IdStudent = student.IdStudent,
            Surname = student.Surname,
            Name = student.Name,
            Patronymic = student.Patronymic,
            Birthdate = student.Birthdate,
            Login = student.Login,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            HadLamaPractice = student.HadLamaPractice,
            IsLamaEmployee = student.IsLamaEmployee
        };
    }

    // Вход в систему
    public async Task<StudentResponseDto?> LoginAsync(StudentLoginDto dto)
    {
        // Ищем студента по логину
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Login == dto.Login);

        if (student == null)
            return null;  // Пользователь не найден

        // Проверяем пароль (сравниваем хэш введённого пароля с хэшем из БД)
        if (student.PasswordHash != HashPassword(dto.Password))
            return null;  // Неверный пароль

        // Возвращаем данные студента (без пароля)
        return new StudentResponseDto
        {
            IdStudent = student.IdStudent,
            Surname = student.Surname,
            Name = student.Name,
            Patronymic = student.Patronymic,
            Birthdate = student.Birthdate,
            Login = student.Login,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            HadLamaPractice = student.HadLamaPractice,
            IsLamaEmployee = student.IsLamaEmployee
        };
    }
}