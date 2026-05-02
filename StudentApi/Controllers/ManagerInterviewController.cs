using Microsoft.AspNetCore.Mvc;
using StudentApi.Services;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ManagerInterviewController : ControllerBase
{
    private readonly ManagerInterviewService _service;
    private readonly ILogger<ManagerInterviewController> _logger;

    public ManagerInterviewController(
        ManagerInterviewService service,
        ILogger<ManagerInterviewController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Получить менеджера по заявке студента
    /// GET /api/v1/managerinterview/manager/{studentApplicationId}
    /// </summary>
    [HttpGet("manager/{studentApplicationId}")]
    public async Task<IActionResult> GetManager(int studentApplicationId)
    {
        try
        {
            var result = await _service.GetManagerByApplicationAsync(studentApplicationId);
            if (result == null)
                return NotFound(new { message = "Менеджер не найден" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manager");
            return StatusCode(500, new { message = "Ошибка при получении менеджера" });
        }
    }

    /// <summary>
    /// Получить свободные слоты менеджера по заявке
    /// GET /api/v1/managerinterview/slots/{studentApplicationId}
    /// </summary>
    [HttpGet("slots/{studentApplicationId}")]
    public async Task<IActionResult> GetFreeSlots(int studentApplicationId)
    {
        try
        {
            var slots = await _service.GetFreeSlotsAsync(studentApplicationId);
            return Ok(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manager slots");
            return StatusCode(500, new { message = "Ошибка при получении слотов" });
        }
    }

    /// <summary>
    /// Записаться на собеседование с менеджером
    /// POST /api/v1/managerinterview/book
    /// </summary>
    [HttpPost("book")]
    public async Task<IActionResult> Book([FromBody] BookManagerSlotRequest request)
    {
        if (request.SlotId <= 0 || request.StudentId <= 0 || request.StudentApplicationId <= 0)
            return BadRequest(new { message = "Некорректные данные" });

        try
        {
            var result = await _service.BookSlotAsync(
                request.SlotId,
                request.StudentApplicationId,
                request.StudentId);

            if (result == null)
                return BadRequest(new { message = "Не удалось записаться на собеседование" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking manager slot");
            return StatusCode(500, new { message = "Ошибка при записи" });
        }
    }

    /// <summary>
    /// Получить интервью по заявке студента
    /// GET /api/v1/managerinterview/my/{studentApplicationId}
    /// </summary>
    [HttpGet("my/{studentApplicationId}")]
    public async Task<IActionResult> GetMyInterview(int studentApplicationId)
    {
        try
        {
            var result = await _service.GetInterviewByApplicationAsync(studentApplicationId);
            return Ok(result); // null — интервью ещё нет, это нормально
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manager interview");
            return StatusCode(500, new { message = "Ошибка при получении интервью" });
        }
    }
}

public sealed class BookManagerSlotRequest
{
    public int SlotId { get; set; }
    public int StudentId { get; set; }
    public int StudentApplicationId { get; set; }
}