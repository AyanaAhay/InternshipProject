using Microsoft.AspNetCore.Mvc;
using StudentApi.Contracts.DTOs;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class QuestionnaireController : ControllerBase
{
    private readonly QuestionnaireService _questionnaireService;
    private readonly ILogger<QuestionnaireController> _logger;

    public QuestionnaireController(QuestionnaireService questionnaireService, ILogger<QuestionnaireController> logger)
    {
        _questionnaireService = questionnaireService;
        _logger = logger;
    }

    // POST /api/v1/questionnaire — создание анкеты
    [HttpPost]
    public async Task<IActionResult> CreateQuestionnaire([FromBody] CreateQuestionnaireDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var result = await _questionnaireService.CreateQuestionnaireAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Не удалось создать анкету. Проверьте ID студента." });
            return CreatedAtAction(nameof(GetQuestionnaire), new { id = result.IdQuestionnaire }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating questionnaire");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    // GET /api/v1/questionnaire/{id} — получить анкету по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuestionnaire(int id)
    {
        try
        {
            var result = await _questionnaireService.GetQuestionnaireByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Анкета не найдена" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questionnaire {Id}", id);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    // GET /api/v1/questionnaire/student/{studentId} — все анкеты студента
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentQuestionnaires(int studentId)
    {
        try
        {
            var result = await _questionnaireService.GetStudentQuestionnairesAsync(studentId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questionnaires for student {Id}", studentId);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    // PUT /api/v1/questionnaire/{id} — обновить анкету
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuestionnaire(int id, [FromBody] CreateQuestionnaireDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var result = await _questionnaireService.UpdateQuestionnaireAsync(id, dto);
            if (result == null)
                return NotFound(new { message = "Анкета не найдена" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating questionnaire {Id}", id);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    // DELETE /api/v1/questionnaire/{id} — удалить анкету
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestionnaire(int id)
    {
        try
        {
            var result = await _questionnaireService.DeleteQuestionnaireAsync(id);
            if (!result)
                return NotFound(new { message = "Анкета не найдена" });
            return Ok(new { message = "Анкета успешно удалена" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting questionnaire {Id}", id);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }
}