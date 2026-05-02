using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Contracts.DTOs;
using ManagerService.Contracts.DTOs;
using ManagerService.Contracts.Enums;

namespace StudentApi.Services;

public class ManagerInterviewService
{
    private readonly ManagerApiClient _managerApi;
    private readonly AppDbContext _context;
    private readonly ILogger<ManagerInterviewService> _logger;

    public ManagerInterviewService(
        ManagerApiClient managerApi,
        AppDbContext context,
        ILogger<ManagerInterviewService> logger)
    {
        _managerApi = managerApi;
        _context = context;
        _logger = logger;
    }

    // Получить менеджера по заявке
    public async Task<ManagerInfoDto?> GetManagerByApplicationAsync(int studentApplicationId)
    {
        return await _managerApi.GetManagerByApplicationAsync(studentApplicationId);
    }

    // Получить свободные слоты менеджера
    public async Task<List<ManagerSlotDetailDto>> GetFreeSlotsAsync(int studentApplicationId)
    {
        // Сначала получаем менеджера по заявке
        var manager = await _managerApi.GetManagerByApplicationAsync(studentApplicationId);

        if (manager == null)
        {
            _logger.LogWarning("No manager found for application {Id}", studentApplicationId);
            return new List<ManagerSlotDetailDto>();
        }

        // Затем получаем его слоты
        return await _managerApi.GetFreeSlotsByManagerAsync(manager.IdEmployee);
    }

    // Записаться на собеседование
    public async Task<ManagerInterviewResponseDto?> BookSlotAsync(
        int slotId,
        int studentApplicationId,
        int studentId)
    {
        var dto = new CreateManagerInterviewDto
        {
            IdSlot = slotId,
            IdStudent = studentId,
            IdStudentApplication = studentApplicationId
        };

        var result = await _managerApi.CreateManagerInterviewAsync(dto);

        if (result != null)
        {
            // Добавляем русский статус
            result.StatusRu = result.Status switch
            {
                "Scheduled" => "Назначено",
                "IsOver" => "Завершено",
                "Cancelled" => "Отменено",
                _ => result.Status
            };

            _logger.LogInformation(
                "Manager interview booked for application {AppId}, slot {SlotId}",
                studentApplicationId, slotId);
        }

        return result;
    }

    // Получить интервью по заявке
    public async Task<ManagerInterviewResponseDto?> GetInterviewByApplicationAsync(int studentApplicationId)
    {
        var result = await _managerApi.GetManagerInterviewByApplicationAsync(studentApplicationId);

        if (result != null)
        {
            result.StatusRu = result.Status switch
            {
                "Scheduled" => "Назначено",
                "IsOver" => "Завершено",
                "Cancelled" => "Отменено",
                _ => result.Status
            };
        }

        return result;
    }
}