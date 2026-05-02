using Microsoft.AspNetCore.Mvc;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InterviewController : ControllerBase
{
    private readonly InterviewSlotService _interviewService;
    private readonly ILogger<InterviewController> _logger;

    public InterviewController(
        InterviewSlotService interviewService,
        ILogger<InterviewController> logger)
    {
        _interviewService = interviewService;
        _logger = logger;
    }

    // GET /api/v1/interview/invitations/{studentApplicationId}
    [HttpGet("invitations/{studentApplicationId}")]
    public async Task<IActionResult> GetInvitations(int studentApplicationId)
    {
        try
        {
            var result = await _interviewService.GetInterviewInvitationsAsync(studentApplicationId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invitations");
            return StatusCode(500, new { message = "Ошибка при получении приглашений" });
        }
    }

    // GET /api/v1/interview/available/{studentApplicationId}
    [HttpGet("available/{studentApplicationId}")]
    public async Task<IActionResult> GetAvailableSlots(int studentApplicationId)
    {
        try
        {
            var slots = await _interviewService.GetAvailableSlotsAsync(studentApplicationId);
            return Ok(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available slots");
            return StatusCode(500, new { message = "Ошибка при получении слотов" });
        }
    }

    // GET /api/v1/interview/slots/{supervisorId}?studentApplicationId=1
    [HttpGet("slots/{supervisorId}")]
    public async Task<IActionResult> GetSlotsBySupervisor(
        int supervisorId,
        [FromQuery] int studentApplicationId)
    {
        if (studentApplicationId <= 0)
            return BadRequest(new { message = "studentApplicationId обязателен" });

        try
        {
            var slots = await _interviewService.GetAvailableSlotsBySupervisorAsync(
                supervisorId,
                studentApplicationId);

            return Ok(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting slots by supervisor");
            return StatusCode(500, new { message = "Ошибка при получении слотов" });
        }
    }

    // POST /api/v1/interview/book/{slotId}
    // POST /api/v1/interview/book/{slotId}
    [HttpPost("book/{slotId}")]
    public async Task<IActionResult> BookSlot(
        int slotId,
        [FromQuery] int studentApplicationId,
        [FromQuery] int? supervisorApplicationId = null) // НОВОЕ
    {
        if (studentApplicationId <= 0)
            return BadRequest(new { message = "studentApplicationId обязателен" });

        try
        {
            var result = await _interviewService.BookSlotAsync(
                slotId,
                studentApplicationId,
                supervisorApplicationId); // НОВОЕ

            if (result == null)
                return BadRequest(new { message = "Не удалось забронировать слот" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking slot");
            return StatusCode(500, new { message = "Ошибка при бронировании" });
        }
    }

    // POST /api/v1/interview/cancel-booking/{slotId}
    [HttpPost("cancel-booking/{slotId}")]
    public async Task<IActionResult> CancelBooking(
        int slotId,
        [FromQuery] int studentApplicationId)
    {
        if (studentApplicationId <= 0)
            return BadRequest(new { message = "studentApplicationId обязателен" });

        try
        {
            var result = await _interviewService.CancelBookingAsync(slotId, studentApplicationId);

            if (!result)
                return BadRequest(new { message = "Не удалось отменить бронирование" });

            return Ok(new { message = "Бронирование отменено" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking");
            return StatusCode(500, new { message = "Ошибка при отмене" });
        }
    }

    // GET /api/v1/interview/booked/{studentApplicationId}
    [HttpGet("booked/{studentApplicationId}")]
    public async Task<IActionResult> GetBookedSlot(int studentApplicationId)
    {
        try
        {
            var result = await _interviewService.GetBookedSlotAsync(studentApplicationId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booked slot");
            return StatusCode(500, new { message = "Ошибка при получении слота" });
        }
    }

    // GET /api/v1/interview/my/{studentApplicationId}
    [HttpGet("my/{studentApplicationId}")]
    public async Task<IActionResult> GetMyInterviews(int studentApplicationId)
    {
        try
        {
            var result = await _interviewService.GetStudentInterviewsAsync(studentApplicationId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student interviews");
            return StatusCode(500, new { message = "Ошибка при получении собеседований" });
        }
    }
}