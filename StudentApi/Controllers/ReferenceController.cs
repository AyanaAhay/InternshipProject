using Microsoft.AspNetCore.Mvc;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReferenceController : ControllerBase
{
    private readonly ManagerDataService _managerDataService;
    private readonly ILogger<ReferenceController> _logger;

    public ReferenceController(ManagerDataService managerDataService, ILogger<ReferenceController> logger)
    {
        _managerDataService = managerDataService;
        _logger = logger;
    }

    [HttpGet("practicetypes")]
    public async Task<IActionResult> GetPracticeTypes()
    {
        try
        {
            var types = await _managerDataService.GetPracticeTypesAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting practice types");
            return StatusCode(500, new { message = "Ошибка при получении типов практик" });
        }
    }

    [HttpGet("specializations")]
    public async Task<IActionResult> GetSpecializations()
    {
        try
        {
            var specs = await _managerDataService.GetSpecializationsAsync();
            return Ok(specs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting specializations");
            return StatusCode(500, new { message = "Ошибка при получении специализаций" });
        }
    }

    [HttpGet("scheduledpractices")]
    public async Task<IActionResult> GetScheduledPractices(
        [FromQuery] int? practiceTypeId,
        [FromQuery] int? specializationId)
    {
        try
        {
            var practices = await _managerDataService.GetScheduledPracticesAsync(practiceTypeId, specializationId);
            return Ok(practices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled practices");
            return StatusCode(500, new { message = "Ошибка при получении запланированных практик" });
        }
    }
}