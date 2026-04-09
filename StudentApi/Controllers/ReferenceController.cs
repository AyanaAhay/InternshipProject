using Microsoft.AspNetCore.Mvc;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReferenceController : ControllerBase
{
    private readonly ManagerDataService _managerDataService;

    public ReferenceController(ManagerDataService managerDataService)
    {
        _managerDataService = managerDataService;
    }

    [HttpGet("practicetypes")]
    public async Task<IActionResult> GetPracticeTypes()
    {
        var types = await _managerDataService.GetPracticeTypesAsync();
        return Ok(types);
    }

    [HttpGet("specializations")]
    public async Task<IActionResult> GetSpecializations()
    {
        var specs = await _managerDataService.GetSpecializationsAsync();
        return Ok(specs);
    }

    [HttpGet("scheduledpractices")]
    public async Task<IActionResult> GetScheduledPractices(
        [FromQuery] int? practiceTypeId,
        [FromQuery] int? specializationId)
    {
        var practices = await _managerDataService.GetScheduledPracticesAsync(practiceTypeId, specializationId);
        return Ok(practices);
    }
}