using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

public class ApplicationService
{
    private readonly AppDbContext _context;
    private readonly ManagerApiClient _managerApiClient;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(
        AppDbContext context,
        ManagerApiClient managerApiClient,
        ILogger<ApplicationService> logger)
    {
        _context = context;
        _managerApiClient = managerApiClient;
        _logger = logger;
    }

    // Создание новой заявки
    public async Task<ApplicationResponseDto?> CreateApplicationAsync(CreateApplicationDto dto)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.IdStudent == dto.IdStudent);

        if (student == null)
            return null;

        if (dto.StartDate >= dto.EndDate)
            return null;

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

        return MapToResponseDto(application, student);
    }

    // Отправка заявки менеджеру
    public async Task<ApplicationResponseDto?> SubmitApplicationAsync(int applicationId)
    {
        var application = await _context.StudentApplications
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.IdStudentApplication == applicationId);

        if (application == null || application.Student == null)
            return null;

        if (application.StudentApplicationStatus == StudentApplicationStatus.Заявка)
        {
            // Отправляем заявку в систему менеджера
            var sent = await _managerApiClient.SendApplicationToManagerAsync(application, application.Student);

            if (sent)
            {
                application.StudentApplicationStatus = StudentApplicationStatus.НаРассмотренииМенеджером;
                application.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Application {applicationId} submitted to manager");
            }
            else
            {
                _logger.LogWarning($"Failed to submit application {applicationId} to manager");
                // Можно добавить статус ошибки
                return null;
            }
        }

        return MapToResponseDto(application, application.Student);
    }

    // Получение всех заявок студента
    public async Task<List<ApplicationResponseDto>> GetStudentApplicationsAsync(int studentId)
    {
        var applications = await _context.StudentApplications
            .Include(a => a.Student)
            .Where(a => a.IdStudent == studentId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return applications
            .Where(a => a.Student != null)
            .Select(a => MapToResponseDto(a, a.Student!))
            .ToList();
    }

    // Получение конкретной заявки
    public async Task<ApplicationResponseDto?> GetApplicationAsync(int applicationId)
    {
        var application = await _context.StudentApplications
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.IdStudentApplication == applicationId);

        if (application == null || application.Student == null)
            return null;

        // Проверяем актуальный статус у менеджера
        if (application.StudentApplicationStatus == StudentApplicationStatus.НаРассмотренииМенеджером ||
            application.StudentApplicationStatus == StudentApplicationStatus.НаРассмотренииРуководителем)
        {
            var statusFromManager = await _managerApiClient.GetApplicationStatusAsync(applicationId);
            if (statusFromManager != null)
            {
                // Обновляем статус на основе ответа от менеджера
                UpdateApplicationStatus(application, statusFromManager);
            }
        }

        return MapToResponseDto(application, application.Student);
    }

    private void UpdateApplicationStatus(StudentApplication application, ApplicationStatusDto statusDto)
    {
        var oldStatus = application.StudentApplicationStatus;

        application.StudentApplicationStatus = statusDto.Status switch
        {
            "Approved" => StudentApplicationStatus.Принят,
            "Rejected" => StudentApplicationStatus.Отказано,
            "Testing" => StudentApplicationStatus.Тестирование,
            "InterviewWithManager" => StudentApplicationStatus.СобеседованиеСМенеджером,
            "InterviewWithSupervisor" => StudentApplicationStatus.СобеседованиеСРуководителем,
            "DocumentsProcessing" => StudentApplicationStatus.ОформлениеДокументов,
            _ => application.StudentApplicationStatus
        };

        if (oldStatus != application.StudentApplicationStatus)
        {
            application.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }
    }

    private ApplicationResponseDto MapToResponseDto(StudentApplication application, Student student)
    {
        return new ApplicationResponseDto
        {
            IdStudentApplication = application.IdStudentApplication,
            IdStudent = application.IdStudent,
            StudentName = $"{student.Surname} {student.Name} {student.Patronymic}".Trim(),
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