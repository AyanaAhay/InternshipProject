using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Contracts.Enums;
using StudentApi.Contracts.DTOs;
using InternshipManager.Api.Contracts.Enums;
using InternshipManager.Api.Contracts.DTOs.SupervisorApplication;
using InternshipManager.Api.Contracts.DTOs.StudentSupervisorApplication;
using InternshipManager.Api.Contracts.DTOs.InterviewSlot;
using InternshipManager.Api.Contracts.DTOs.Interview;

namespace StudentApi.Services;

public class InterviewSlotService
{
    private readonly SupervisorApiClient _supervisorApi;
    private readonly AppDbContext _context;
    private readonly ILogger<InterviewSlotService> _logger;

    public InterviewSlotService(
        SupervisorApiClient supervisorApi,
        AppDbContext context,
        ILogger<InterviewSlotService> logger)
    {
        _supervisorApi = supervisorApi;
        _context = context;
        _logger = logger;
    }

    // Получить все доступные слоты для заявки студента
    public async Task<List<InterviewSlotForStudentDto>> GetAvailableSlotsAsync(
        int studentApplicationId)
    {
        var links = await _supervisorApi.GetStudentLinksAsync(studentApplicationId);
        var interviewLinks = links.Where(l => l.Status == "Interview").ToList();

        if (!interviewLinks.Any())
        {
            _logger.LogInformation(
                "No interview invitations for application {Id}",
                studentApplicationId);

            return new List<InterviewSlotForStudentDto>();
        }

        var allSlots = new List<InterviewSlotForStudentDto>();
        var processedSupervisors = new HashSet<int>();

        foreach (var link in interviewLinks)
        {
            var supervisorApp = await _supervisorApi
                .GetSupervisorApplicationAsync(link.IdSupervisorApplication);

            if (supervisorApp == null)
                continue;

            if (processedSupervisors.Contains(supervisorApp.IdEmployee))
                continue;

            processedSupervisors.Add(supervisorApp.IdEmployee);

            var slots = await _supervisorApi.GetAvailableSlotsAsync(
                supervisorApp.IdEmployee,
                studentApplicationId);

            allSlots.AddRange(slots.Select(s => new InterviewSlotForStudentDto
            {
                IdInterviewSlot = s.IdInterviewSlot,
                IdSupervisorApplication = link.IdSupervisorApplication,
                SupervisorId = supervisorApp.IdEmployee,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MeetingPlace = s.MeetingPlace
            }));
        }

        return allSlots
            .GroupBy(s => s.IdInterviewSlot)
            .Select(g => g.First())
            .OrderBy(s => s.StartTime)
            .ToList();
    }

    // Получить слоты по конкретному supervisorId
    public async Task<List<AvailableInterviewSlotDto>> GetAvailableSlotsBySupervisorAsync(
        int supervisorId,
        int studentApplicationId)
    {
        return await _supervisorApi.GetAvailableSlotsAsync(supervisorId, studentApplicationId);
    }

    // Получить приглашения на собеседование
    public async Task<List<StudentSupervisorLinkDetailDto>> GetInterviewInvitationsAsync(
        int studentApplicationId)
    {
        var links = await _supervisorApi.GetStudentLinksAsync(studentApplicationId);

        foreach (var link in links)
            link.StatusRu = GetStatusRu(link.Status);

        return links.Where(l => l.Status == "Interview").ToList();
    }

    /// <summary>
    /// Забронировать слот
    /// </summary>
    public async Task<BookSlotResponseDto?> BookSlotAsync(
        int slotId,
        int studentApplicationId,
        int? supervisorApplicationId = null) // НОВОЕ
    {
        var result = await _supervisorApi.BookSlotAsync(
            slotId,
            studentApplicationId,
            supervisorApplicationId); // НОВОЕ

        if (result != null)
            _logger.LogInformation(
                "Slot {SlotId} booked for application {AppId}, supervisor app {SupAppId}",
                slotId,
                studentApplicationId,
                supervisorApplicationId);

        return result;
    }

    // Отменить бронирование
    public async Task<bool> CancelBookingAsync(
        int slotId,
        int studentApplicationId)
    {
        var result = await _supervisorApi.CancelBookingAsync(slotId, studentApplicationId);

        if (result)
            _logger.LogInformation(
                "Booking cancelled for slot {SlotId}, app {AppId}",
                slotId,
                studentApplicationId);

        return result;
    }

    // Получить забронированный слот
    public async Task<BookedInterviewSlotDto?> GetBookedSlotAsync(
        int studentApplicationId)
    {
        return await _supervisorApi.GetBookedSlotAsync(studentApplicationId);
    }

    // Получить все собеседования студента
    public async Task<List<StudentInterviewResponseDto>> GetStudentInterviewsAsync(
        int studentApplicationId)
    {
        var interviews = await _supervisorApi.GetStudentInterviewsAsync(studentApplicationId);

        foreach (var iv in interviews)
        {
            iv.StatusRu = iv.Status switch
            {
                "Scheduled" => "Предстоит",
                "IsOver" => "Пройдено",
                "Cancelled" => "Отменено",
                _ => iv.Status
            };
        }

        return interviews;
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