using Microsoft.AspNetCore.Mvc;
using StudentApi.Contracts.DTOs;
using StudentApi.Services;

namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private const long MaxFileSize = 10 * 1024 * 1024;
        private static readonly string[] AllowedContentTypes = { "application/pdf", "image/jpeg", "image/png", "image/jpg" };

        public DocumentController(DocumentService documentService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        [HttpGet("types")]
        public IActionResult GetDocumentTypes()
        {
            var types = _documentService.GetDocumentTypes();
            return Ok(types);
        }

        [HttpPost("slots/{studentApplicationId}")]
        public async Task<IActionResult> CreateDocumentSlots(int studentApplicationId)
        {
            try
            {
                var result = await _documentService.CreateDocumentSlotsAsync(studentApplicationId);
                if (!result.Any())
                    return BadRequest(new { message = "Заявка не найдена" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document slots");
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("{documentId}/upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadFile(int documentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Файл не выбран" });

            if (file.Length > MaxFileSize)
                return BadRequest(new { message = "Файл слишком большой. Максимум 10 МБ." });

            if (!AllowedContentTypes.Contains(file.ContentType.ToLower()))
                return BadRequest(new { message = "Недопустимый формат файла. Разрешены: PDF, JPEG, PNG.", allowed = AllowedContentTypes });

            try
            {
                var result = await _documentService.UploadFileAsync(documentId, file);
                if (result == null)
                    return NotFound(new { message = "Документ не найден" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file for document {Id}", documentId);
                return StatusCode(500, new { message = "Ошибка при загрузке файла" });
            }
        }

        [HttpGet("{documentId}/download")]
        public async Task<IActionResult> DownloadFile(int documentId)
        {
            try
            {
                var result = await _documentService.DownloadFileAsync(documentId);
                if (result == null)
                    return NotFound(new { message = "Файл не найден" });
                return File(result.Value.FileBytes, result.Value.ContentType, result.Value.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file for document {Id}", documentId);
                return StatusCode(500, new { message = "Ошибка при скачивании файла" });
            }
        }

        [HttpGet("application/{studentApplicationId}")]
        public async Task<IActionResult> GetByApplication(int studentApplicationId)
        {
            try
            {
                var result = await _documentService.GetByApplicationAsync(studentApplicationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents for application {Id}", studentApplicationId);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudent(int studentId)
        {
            try
            {
                var result = await _documentService.GetByStudentAsync(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents for student {Id}", studentId);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetById(int documentId)
        {
            try
            {
                var result = await _documentService.GetByIdAsync(documentId);
                if (result == null)
                    return NotFound(new { message = "Документ не найден" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document {Id}", documentId);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpDelete("{documentId}/file")]
        public async Task<IActionResult> DeleteFile(int documentId)
        {
            try
            {
                var result = await _documentService.DeleteFileAsync(documentId);
                if (result == null)
                    return NotFound(new { message = "Документ не найден" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file for document {Id}", documentId);
                return StatusCode(500, new { message = "Ошибка при удалении файла" });
            }
        }

        [HttpPut("{documentId}/verification")]
        public async Task<IActionResult> UpdateVerificationStatus(int documentId, [FromBody] UpdateVerificationStatusDto dto)
        {
            try
            {
                // var result = await _documentService.UpdateVerificationStatusAsync(documentId, dto.VerificationStatus);
                // НОВОЕ - комментарий менеджера по документу
                var result = await _documentService.UpdateVerificationStatusAsync(documentId, dto.VerificationStatus, dto.Comment);
                if (result == null)
                    return NotFound(new { message = "Документ не найден" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating verification for document {Id}", documentId);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPut("{documentId}/contract")]
        public async Task<IActionResult> UpdateContractStatus(int documentId, [FromBody] UpdateContractStatusDto dto)
        {
            try
            {
                var result = await _documentService.UpdateContractStatusAsync(documentId, dto.ContractStatus);
                if (result == null)
                    return NotFound(new { message = "Документ не найден" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contract status for document {Id}", documentId);
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary> 
        /// Получить непроверенные документы по заявке (для менеджера) 
        /// GET /api/v1/document/pending/{studentApplicationId} 
        /// </summary> 
        [HttpGet("pending/{studentApplicationId}")] 
        public async Task<IActionResult> GetPendingByApplication(int studentApplicationId) 
        { 
            try { 
                var result = await _documentService.GetPendingByApplicationAsync(studentApplicationId); 
                return Ok(result); 
            } catch (Exception ex) { 
                _logger.LogError(ex, "Error getting pending documents for application {Id}", studentApplicationId); 
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" }); 
            } 
        }
    }
}