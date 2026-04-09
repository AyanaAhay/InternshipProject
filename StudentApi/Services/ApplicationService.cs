using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

// Сервис для работы с заявками студентов
public class ApplicationService
{
    private readonly AppDbContext _context;

    public ApplicationService(AppDbContext context)
    {
        _context = context;
    }

    // Создание новой заявки
    public async Task<ApplicationResponseDto?> CreateApplicationAsync(CreateApplicationDto dto)
    {
        // Проверяем, существует ли студент
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.IdStudent == dto.IdStudent);

        if (student == null)
            return null;  // Студент не найден

        // Проверяем даты
        if (dto.StartDate >= dto.EndDate)
            return null;  // Дата начала должна быть раньше даты окончания

        // Создаём заявку со статусом "Заявка"
        var application = new StudentApplication
        {
            IdStudent = dto.IdStudent,
            IdScheduledPractice = dto.IdScheduledPractice,
            IdPracticeType = dto.IdPracticeType,
            IdSpecialization = dto.IdSpecialization,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            StudentApplicationStatus = StudentApplicationStatus.Заявка,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.StudentApplications.Add(application);
        await _context.SaveChangesAsync();

        // Возвращаем DTO заявки
        return new ApplicationResponseDto
        {
            IdStudentApplication = application.IdStudentApplication,
            IdStudent = application.IdStudent,
            StudentName = $"{student.Surname} {student.Name}",
            IdScheduledPractice = application.IdScheduledPractice,
            IdPracticeType = application.IdPracticeType,
            IdSpecialization = application.IdSpecialization,
            Status = application.StudentApplicationStatus.ToString(),
            StartDate = application.StartDate,
            EndDate = application.EndDate,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }

    // Отправка заявки менеджеру (изменение статуса)
    public async Task<ApplicationResponseDto?> SubmitApplicationAsync(int applicationId)
    {
        var application = await _context.StudentApplications
            .Include(a => a.Student)  // Загружаем данные студента
            .FirstOrDefaultAsync(a => a.IdStudentApplication == applicationId);

        if (application == null)
            return null;  // Заявка не найдена

        // Меняем статус только если заявка в Заявка
        if (application.StudentApplicationStatus == StudentApplicationStatus.Заявка)
        {
            application.StudentApplicationStatus = StudentApplicationStatus.НаРассмотренииМенеджером;
            application.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        return new ApplicationResponseDto
        {
            IdStudentApplication = application.IdStudentApplication,
            IdStudent = application.IdStudent,
            StudentName = $"{application.Student?.Surname} {application.Student?.Name}",
            IdScheduledPractice = application.IdScheduledPractice,
            IdPracticeType = application.IdPracticeType,
            IdSpecialization = application.IdSpecialization,
            Status = application.StudentApplicationStatus.ToString(),
            StartDate = application.StartDate,
            EndDate = application.EndDate,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }

    // Получение всех заявок студента
    public async Task<List<ApplicationResponseDto>> GetStudentApplicationsAsync(int studentId)
    {
        var applications = await _context.StudentApplications
            .Include(a => a.Student)
            .Where(a => a.IdStudent == studentId)
            .OrderByDescending(a => a.CreatedAt)  // Сначала новые
            .ToListAsync();

        return applications.Select(a => new ApplicationResponseDto
        {
            IdStudentApplication = a.IdStudentApplication,
            IdStudent = a.IdStudent,
            StudentName = $"{a.Student?.Surname} {a.Student?.Name}",
            IdScheduledPractice = a.IdScheduledPractice,
            IdPracticeType = a.IdPracticeType,
            IdSpecialization = a.IdSpecialization,
            Status = a.StudentApplicationStatus.ToString(),
            StartDate = a.StartDate,
            EndDate = a.EndDate,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        }).ToList();
    }

    // Получение конкретной заявки
    public async Task<ApplicationResponseDto?> GetApplicationAsync(int applicationId)
    {
        var application = await _context.StudentApplications
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.IdStudentApplication == applicationId);

        if (application == null)
            return null;

        return new ApplicationResponseDto
        {
            IdStudentApplication = application.IdStudentApplication,
            IdStudent = application.IdStudent,
            StudentName = $"{application.Student?.Surname} {application.Student?.Name}",
            IdScheduledPractice = application.IdScheduledPractice,
            IdPracticeType = application.IdPracticeType,
            IdSpecialization = application.IdSpecialization,
            Status = application.StudentApplicationStatus.ToString(),
            StartDate = application.StartDate,
            EndDate = application.EndDate,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }
}