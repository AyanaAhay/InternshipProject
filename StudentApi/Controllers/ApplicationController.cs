using Microsoft.AspNetCore.Mvc;
using StudentApi.DTOs;
using StudentApi.Services;

namespace StudentApi.Controllers;

using StudentApi.Models;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly ApplicationService _applicationService;

    public ApplicationController(ApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    // POST /api/v1/application — создание заявки
    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _applicationService.CreateApplicationAsync(dto);
        if (result == null)
            return BadRequest(new { message = "Не удалось создать заявку. Проверьте данные." });
        return CreatedAtAction(nameof(GetApplication), new { id = result.IdStudentApplication }, result);
    }

    // POST /api/v1/application/{id}/submit — отправка менеджеру
    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitApplication(int id)
    {
        var result = await _applicationService.SubmitApplicationAsync(id);
        if (result == null)
            return NotFound(new { message = "Заявка не найдена" });
        return Ok(result);
    }

    // POST /api/v1/application/{id}/cancel — отмена студентом
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelApplication(int id)
    {
        var result = await _applicationService.CancelApplicationAsync(id);
        if (result == null)
            return BadRequest(new { message = "Невозможно отменить заявку (не найдена или уже завершена)" });
        return Ok(result);
    }

    // PUT /api/v1/application/{id}/status — изменение статуса внешней системой
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateApplicationStatusDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _applicationService.UpdateStatusAsync(id, dto.Status, dto.RejectionComment);
        if (result == null)
            return NotFound(new { message = "Заявка не найдена" });
        return Ok(result);
    }

    // PUT /api/v1/application/{id}/questionnaire/{questionnaireId} — привязка анкеты
    [HttpPut("{id}/questionnaire/{questionnaireId}")]
    public async Task<IActionResult> AttachQuestionnaire(int id, int questionnaireId)
    {
        var result = await _applicationService.AttachQuestionnaireAsync(id, questionnaireId);
        if (result == null)
            return BadRequest(new { message = "Не удалось привязать анкету. Проверьте ID." });
        return Ok(result);
    }

    // GET /api/v1/application — все заявки (для внешних систем)
    //[HttpGet]
    //public async Task<IActionResult> GetAllApplications()
    //{
    //    var result = await _applicationService.GetAllApplicationsAsync();
    //    return Ok(result);
    //}

    // GET /api/v1/application — все заявки (для внешних систем)
    // ?status=UnderManagerReview — фильтр по статусу
    // ?excludeDrafts=false — показать включая черновики
    [HttpGet]
    public async Task<IActionResult> GetAllApplications(
        [FromQuery] StudentApplicationStatus? status = null,
        [FromQuery] bool excludeDrafts = true)
    {
        var result = await _applicationService.GetAllApplicationsAsync(status, excludeDrafts);
        return Ok(result);
    }

    // GET /api/v1/application/student/{studentId} — заявки студента
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentApplications(int studentId)
    {
        var result = await _applicationService.GetStudentApplicationsAsync(studentId);
        return Ok(result);
    }

    // GET /api/v1/application/{id} — конкретная заявка
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplication(int id)
    {
        var result = await _applicationService.GetApplicationAsync(id);
        if (result == null)
            return NotFound(new { message = "Заявка не найдена" });
        return Ok(result);
    }
}