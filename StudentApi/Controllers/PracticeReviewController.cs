using Microsoft.AspNetCore.Mvc;
using StudentApi.Contracts.DTOs;
using StudentApi.Services;

namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PracticeReviewController : ControllerBase
    {
        private readonly PracticeReviewService _reviewService;
        private readonly ILogger<PracticeReviewController> _logger;

        public PracticeReviewController(PracticeReviewService reviewService, ILogger<PracticeReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        /// <summary>
        /// Создать отзыв о практике
        /// POST /api/v1/practicereview
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePracticeReviewDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _reviewService.CreateAsync(dto);
                if (result == null)
                    return BadRequest(new { message = "Не удалось создать отзыв. Проверьте: заявка существует, статус — Accepted, отзыв ещё не оставлен." });
                return CreatedAtAction(nameof(GetById), new { id = result.IdPracticeReview }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Обновить отзыв
        /// PUT /api/v1/practicereview/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePracticeReviewDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _reviewService.UpdateAsync(id, dto);
                if (result == null) return NotFound(new { message = "Отзыв не найден" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {Id}", id);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получить отзыв по ID
        /// GET /api/v1/practicereview/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _reviewService.GetByIdAsync(id);
                if (result == null) return NotFound(new { message = "Отзыв не найден" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review {Id}", id);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получить отзыв по заявке
        /// GET /api/v1/practicereview/application/{studentApplicationId}
        /// </summary>
        [HttpGet("application/{studentApplicationId}")]
        public async Task<IActionResult> GetByApplication(int studentApplicationId)
        {
            try
            {
                var result = await _reviewService.GetByApplicationAsync(studentApplicationId);
                if (result == null) return Ok(null); // Отзыва нет — это нормально
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review for application {Id}", studentApplicationId);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Все отзывы студента
        /// GET /api/v1/practicereview/student/{studentId}
        /// </summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            try
            {
                var result = await _reviewService.GetByStudentAsync(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for student {Id}", studentId);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Все отзывы (для внешних систем)
        /// GET /api/v1/practicereview
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _reviewService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reviews");
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Удалить отзыв
        /// DELETE /api/v1/practicereview/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _reviewService.DeleteAsync(id);
                if (!result) return NotFound(new { message = "Отзыв не найден" });
                return Ok(new { message = "Отзыв удалён" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {Id}", id);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }
    }
}