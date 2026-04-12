//using Microsoft.AspNetCore.Mvc;
//using StudentApi.DTOs;
//using StudentApi.Services;

//namespace StudentApi.Controllers;

//[ApiController]
//[Route("api/v1/[controller]")]
//public class ApplicationController : ControllerBase
//{
//    private readonly ApplicationService _applicationService;

//    public ApplicationController(ApplicationService applicationService)
//    {
//        _applicationService = applicationService;
//    }

//    // POST /api/v1/application - создание новой заявки
//    [HttpPost]
//    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDto dto)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var result = await _applicationService.CreateApplicationAsync(dto);

//        if (result == null)
//            return BadRequest(new { message = "Не удалось создать заявку (проверьте даты и ID студента)" });

//        return CreatedAtAction(nameof(GetApplication), new { id = result.IdStudentApplication }, result);
//    }

//    // POST /api/v1/application/{id}/submit - отправка заявки менеджеру
//    [HttpPost("{id}/submit")]
//    public async Task<IActionResult> SubmitApplication(int id)
//    {
//        var result = await _applicationService.SubmitApplicationAsync(id);

//        if (result == null)
//            return NotFound(new { message = "Заявка не найдена" });

//        return Ok(result);
//    }

//    // GET /api/v1/application/student/{studentId} - получить все заявки студента
//    [HttpGet("student/{studentId}")]
//    public async Task<IActionResult> GetStudentApplications(int studentId)
//    {
//        var result = await _applicationService.GetStudentApplicationsAsync(studentId);
//        return Ok(result);
//    }

//    // GET /api/v1/application/{id} - получить конкретную заявку
//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetApplication(int id)
//    {
//        var result = await _applicationService.GetApplicationAsync(id);

//        if (result == null)
//            return NotFound(new { message = "Заявка не найдена" });

//        return Ok(result);
//    }
//}




using Microsoft.AspNetCore.Mvc;
using StudentApi.DTOs;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly ApplicationService _applicationService;

    public ApplicationController(ApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    // POST /api/v1/application - создание новой заявки
    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _applicationService.CreateApplicationAsync(dto);

        if (result == null)
            return BadRequest(new { message = "Не удалось создать заявку (проверьте даты и ID студента)" });

        return CreatedAtAction(nameof(GetApplication), new { id = result.IdStudentApplication }, result);
    }

    // POST /api/v1/application/{id}/submit - отправка заявки менеджеру
    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitApplication(int id)
    {
        var result = await _applicationService.SubmitApplicationAsync(id);

        if (result == null)
            return NotFound(new { message = "Заявка не найдена" });

        return Ok(result);
    }

    // GET /api/v1/application/student/{studentId} - получить все заявки студента
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentApplications(int studentId)
    {
        var result = await _applicationService.GetStudentApplicationsAsync(studentId);
        return Ok(result);
    }

    // GET /api/v1/application/{id} - получить конкретную заявку
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplication(int id)
    {
        var result = await _applicationService.GetApplicationAsync(id);

        if (result == null)
            return NotFound(new { message = "Заявка не найдена" });

        return Ok(result);
    }
}
