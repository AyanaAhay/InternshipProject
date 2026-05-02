using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Contracts.DTOs;
using StudentApi.Contracts.Enums;
using StudentApi.Models;

namespace StudentApi.Services;

public class ApplicationService
{
    private readonly AppDbContext _context;
    private readonly ManagerApiClient _managerApiClient;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(AppDbContext context, ManagerApiClient managerApiClient, ILogger<ApplicationService> logger)
    {
        _context = context;
        _managerApiClient = managerApiClient;
        _logger = logger;
    }

    // ========== Создание заявки ==========
    public async Task<ApplicationResponseDto?> CreateApplicationAsync(CreateApplicationDto dto)
    {
        var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.IdStudent == dto.IdStudent);
        if (student == null)
            return null;
        if (dto.StartDate >= dto.EndDate)
            return null;
        if (dto.IdQuestionnaire.HasValue)
        {
            var questionnaire = await _context.Questionnaires.AsNoTracking().FirstOrDefaultAsync(q => q.IdQuestionnaire == dto.IdQuestionnaire.Value && q.IdStudent == dto.IdStudent);
            if (questionnaire == null)
                return null;
        }
        var application = new StudentApplication
        {
            IdStudent = dto.IdStudent,
            IdQuestionnaire = dto.IdQuestionnaire,
            IdScheduledPractice = dto.IdScheduledPractice,
            IdPracticeType = dto.IdPracticeType,
            IdSpecialization = dto.IdSpecialization,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            StudentApplicationStatus = StudentApplicationStatus.Draft,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _context.StudentApplications.Add(application);
        await _context.SaveChangesAsync();
        return MapToResponseDto(application, student);
    }

    // ========== Отправка заявки менеджеру ==========
    public async Task<ApplicationResponseDto?> SubmitApplicationAsync(int applicationId)
    {
        _logger.LogInformation("Submitting application {Id}", applicationId);
        var application = await _context.StudentApplications.Include(a => a.Student).FirstOrDefaultAsync(a => a.IdStudentApplication == applicationId);
        if (application?.Student == null)
            return null;
        if (application.StudentApplicationStatus == StudentApplicationStatus.Draft)
        {
            application.StudentApplicationStatus = StudentApplicationStatus.UnderManagerReview;
            application.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        return MapToResponseDto(application, application.Student);
    }

    // ========== Отмена заявки студентом ==========
    public async Task<ApplicationResponseDto?> CancelApplicationAsync(int applicationId)
    {
        var application = await _context.StudentApplications.Include(a => a.Student).FirstOrDefaultAsync(a => a.IdStudentApplication == applicationId);
        if (application?.Student == null)
            return null;
        var terminalStatuses = new[] { StudentApplicationStatus.Accepted, StudentApplicationStatus.Rejected, StudentApplicationStatus.CancelledByStudent };
        if (terminalStatuses.Contains(application.StudentApplicationStatus))
            return null;
        application.StudentApplicationStatus = StudentApplicationStatus.CancelledByStudent;
        application.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return MapToResponseDto(application, application.Student);
    }

    // ========== Изменение статуса внешней системой ==========
    public async Task<ApplicationResponseDto?> UpdateStatusAsync(int applicationId, StudentApplicationStatus newStatus, string? rejectionComment = null)
    {
        var application = await _context.StudentApplications.Include(a => a.Student).FirstOrDefaultAsync(a => a.IdStudentApplication == applicationId);
        if (application?.Student == null)
            return null;
        application.StudentApplicationStatus = newStatus;
        application.UpdatedAt = DateTime.Now;

        // Сохраняем комментарий при отказе
        if (newStatus == StudentApplicationStatus.Rejected && rejectionComment != null) 
        { 
            application.RejectionComment = rejectionComment; 
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Application {Id} → {Status}", applicationId, newStatus);
        return MapToResponseDto(application, application.Student);
    }

    // ========== Привязка анкеты к заявке ==========
    public async Task<ApplicationResponseDto?> AttachQuestionnaireAsync(int applicationId, int questionnaireId)
    {
        var application = await _context.StudentApplications.Include(a => a.Student).FirstOrDefaultAsync(a => a.IdStudentApplication == applicationId);
        if (application?.Student == null)
            return null;
        var questionnaire = await _context.Questionnaires.AsNoTracking().FirstOrDefaultAsync(q => q.IdQuestionnaire == questionnaireId && q.IdStudent == application.IdStudent);
        if (questionnaire == null)
            return null;
        application.IdQuestionnaire = questionnaireId;
        application.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return MapToResponseDto(application, application.Student);
    }

    // ========== Все заявки (для внешних систем) ==========
    //public async Task<List<ApplicationResponseDto>> GetAllApplicationsAsync()
    //{
    //    var applications = await _context.StudentApplications.AsNoTracking().Include(a => a.Student).OrderByDescending(a => a.CreatedAt).ToListAsync();
    //    return applications.Where(a => a.Student != null).Select(a => MapToResponseDto(a, a.Student!)).ToList();
    //}


    public async Task<List<ApplicationResponseDto>> GetAllApplicationsAsync(
        StudentApplicationStatus? statusFilter = null,
        bool excludeDrafts = true)
    {
        var query = _context.StudentApplications
            .AsNoTracking()
            .Include(a => a.Student)
            .AsQueryable();

        if (statusFilter.HasValue)
        {
            // Даже при фильтре по статусу — не отдаём черновики
            if (statusFilter.Value == StudentApplicationStatus.Draft)
                return new List<ApplicationResponseDto>();

            query = query.Where(a => a.StudentApplicationStatus == statusFilter.Value);
        }
        else if (excludeDrafts)
        {
            query = query.Where(a =>
                a.StudentApplicationStatus != StudentApplicationStatus.Draft &&
                a.StudentApplicationStatus != StudentApplicationStatus.CancelledByStudent);
        }

        var applications = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return applications
            .Where(a => a.Student != null)
            .Select(a => MapToResponseDto(a, a.Student!))
            .ToList();
    }


    // ========== Заявки конкретного студента ==========
    public async Task<List<ApplicationResponseDto>> GetStudentApplicationsAsync(int studentId)
    {
        var applications = await _context.StudentApplications.AsNoTracking().Include(a => a.Student).Where(a => a.IdStudent == studentId).OrderByDescending(a => a.CreatedAt).ToListAsync();
        return applications.Where(a => a.Student != null).Select(a => MapToResponseDto(a, a.Student!)).ToList();
    }

    // ========== Конкретная заявка ==========
    public async Task<ApplicationResponseDto?> GetApplicationAsync(int applicationId)
    {
        var application = await _context.StudentApplications.AsNoTracking().Include(a => a.Student).FirstOrDefaultAsync(a => a.IdStudentApplication == applicationId);
        if (application?.Student == null)
            return null;
        return MapToResponseDto(application, application.Student);
    }

    // ========== Маппинг ==========
    private ApplicationResponseDto MapToResponseDto(StudentApplication application, Student student)
    {
        return new ApplicationResponseDto
        {
            IdStudentApplication = application.IdStudentApplication,
            IdStudent = application.IdStudent,
            StudentName = $"{student.Surname} {student.Name} {student.Patronymic}".Trim(),
            IdQuestionnaire = application.IdQuestionnaire,
            IdScheduledPractice = application.IdScheduledPractice,
            IdPracticeType = application.IdPracticeType,
            IdSpecialization = application.IdSpecialization,
            Status = application.StudentApplicationStatus,
            StartDate = application.StartDate,
            EndDate = application.EndDate,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt,
            RejectionComment = application.RejectionComment, // НОВОЕ
        };
    }
}