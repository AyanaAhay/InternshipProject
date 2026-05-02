using Microsoft.AspNetCore.Mvc;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/v1/studentlink")]
public class StudentSupervisorLinkController : ControllerBase
{
    private readonly StudentSupervisorLinkService _service;
    private readonly ILogger<StudentSupervisorLinkController> _logger;

    public StudentSupervisorLinkController(
        StudentSupervisorLinkService service,
        ILogger<StudentSupervisorLinkController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Получить все связки студента по заявке
    /// GET /api/v1/studentlink/{studentApplicationId}
    /// </summary>
    [HttpGet("{studentApplicationId}")]
    public async Task<IActionResult> GetLinks(int studentApplicationId)
    {
        try
        {
            var result = await _service.GetLinksAsync(studentApplicationId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student links");
            return StatusCode(500, new { message = "Ошибка при получении связок" });
        }
    }

    /// <summary>
    /// Выбрать конкретного руководителя (практику)
    /// POST /api/v1/studentlink/{studentApplicationId}/choose/{supervisorApplicationId}
    /// </summary>
    [HttpPost("{studentApplicationId}/choose/{supervisorApplicationId}")]
    public async Task<IActionResult> Choose(
        int studentApplicationId,
        int supervisorApplicationId)
    {
        try
        {
            var result = await _service.ChooseAsync(
                supervisorApplicationId,
                studentApplicationId);

            if (!result)
                return BadRequest(new { message = "Не удалось выбрать место практики" });

            return Ok(new { message = "Место практики выбрано!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error choosing supervisor");
            return StatusCode(500, new { message = "Ошибка при выборе практики" });
        }
    }
}