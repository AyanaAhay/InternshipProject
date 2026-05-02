using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Contracts.Enums;
using StudentApi.Models;
using StudentApi.Contracts.DTOs;
using InternshipManager.Api.Contracts.Enums;
using InternshipManager.Api.Contracts.DTOs.StudentSupervisorApplication;

namespace StudentApi.Services;

public class StudentSupervisorLinkService
{
    private readonly SupervisorApiClient _supervisorApi;
    private readonly AppDbContext _context;
    private readonly ILogger<StudentSupervisorLinkService> _logger;

    public StudentSupervisorLinkService(
        SupervisorApiClient supervisorApi,
        AppDbContext context,
        ILogger<StudentSupervisorLinkService> logger)
    {
        _supervisorApi = supervisorApi;
        _context = context;
        _logger = logger;
    }

    // Получить все связки студента по заявке
    public async Task<List<StudentSupervisorLinkDetailDto>> GetLinksAsync(
        int studentApplicationId)
    {
        var links = await _supervisorApi.GetStudentLinksAsync(studentApplicationId);

        foreach (var link in links)
            link.StatusRu = GetStatusRu(link.Status);

        return links;
    }

    // Выбрать конкретного руководителя
    public async Task<bool> ChooseAsync(
        int supervisorApplicationId,
        int studentApplicationId)
    {
        var result = await _supervisorApi.ChooseSupervisorAsync(
            supervisorApplicationId,
            studentApplicationId);

        if (!result)
        {
            _logger.LogWarning(
                "Failed to choose supervisor {SupAppId} for student {StdAppId}",
                supervisorApplicationId,
                studentApplicationId);

            return false;
        }

        // Обновляем статус заявки студента
        var application = await _context.StudentApplications
            .FirstOrDefaultAsync(a => a.IdStudentApplication == studentApplicationId);

        if (application != null)
        {
            application.StudentApplicationStatus = StudentApplicationStatus.DocumentsSigning;
            application.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Application {Id} → DocumentsSigning",
                studentApplicationId);
        }

        return true;
    }

    private static string GetStatusRu(string status) =>
        status switch
        {
            "UnderReviewbySupervisor" => "На рассмотрении у руководителя",
            "Interview" => "Приглашён на собеседование",
            "DocumentProcessing" => "Оформление документов",
            "Rejected" => "Отказано",
            "Accepted" => "Принят",
            _ => status
        };
}