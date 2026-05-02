using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

public class DocumentService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DocumentService> _logger;
    private readonly ManagerApiClient _managerApi;
    private readonly string _uploadPath;

    public DocumentService(
        AppDbContext context,
        ILogger<DocumentService> logger,
        IConfiguration configuration,
        ManagerApiClient managerApi)
    {
        _context = context;
        _logger = logger;
        _managerApi = managerApi;
        _uploadPath = configuration["FileStorage:UploadPath"] ?? "uploads/documents";

        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    // ========== Заглушка удалена ==========
    public List<DocumentTypeDto> GetDocumentTypes()
    {
        // Возвращаем пустой список. Фронтенд больше не должен запрашивать этот метод,
        // так как список документов теперь формируется динамически под каждую специализацию.
        return new List<DocumentTypeDto>();
    }

    // ========== Создание записей документов для заявки ==========
    public async Task<List<DocumentResponseDto>> CreateDocumentSlotsAsync(int studentApplicationId)
    {
        var application = await _context.StudentApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdStudentApplication == studentApplicationId);

        if (application == null)
            return new List<DocumentResponseDto>();

        // Проверяем — не созданы ли уже
        var existing = await _context.StudentDocuments
            .AsNoTracking()
            .Where(d => d.IdStudentApplication == studentApplicationId)
            .ToListAsync();

        if (existing.Any())
        {
            // Если документы уже есть, нам всё равно нужно получить их названия от менеджера
            return await GetByApplicationAsync(studentApplicationId);
        }

        // Получаем список документов ТОЛЬКО от менеджера
        List<DocumentTypeDto> documentTypes = new List<DocumentTypeDto>();

        if (application.IdSpecialization.HasValue)
        {
            var specDocs = await _managerApi.GetDocumentsForSpecializationAsync(
                application.IdSpecialization.Value);

            if (specDocs != null && specDocs.Documents.Any())
            {
                documentTypes = specDocs.Documents.Select(d => new DocumentTypeDto
                {
                    IdDocumentType = d.IdDocumentType,
                    Name = d.DocumentName,
                    Description = d.Description,
                    IsRequired = d.Mandatory
                }).ToList();

                _logger.LogInformation(
                    "Got {Count} document types from Manager API for specialization {Id}",
                    documentTypes.Count, application.IdSpecialization.Value);
            }
            else
            {
                _logger.LogWarning(
                    "Manager API returned no documents for specialization {Id}",
                    application.IdSpecialization.Value);
                // Если менеджер ничего не вернул, мы НЕ создаём слоты.
                return new List<DocumentResponseDto>();
            }
        }
        else
        {
            // Если у заявки нет специализации, мы тоже не можем создать слоты
            return new List<DocumentResponseDto>();
        }

        // Создаём слоты только если получили реальные типы от менеджера
        var documents = documentTypes.Select(dt => new StudentDocument
        {
            IdStudentApplication = studentApplicationId,
            IdStudent = application.IdStudent,
            IdDocumentType = dt.IdDocumentType,
            IdSpecialization = application.IdSpecialization,
            UploadStatus = UploadStatus.NotUploaded,
            VerificationStatus = VerificationStatus.Pending,
            ContractStatus = ContractStatus.NotReceived
        }).ToList();

        _context.StudentDocuments.AddRange(documents);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Created {Count} document slots for application {Id}",
            documents.Count, studentApplicationId);

        return documents.Select(d => MapToResponseDto(d, documentTypes)).ToList();
    }

    // ========== Загрузка файла ==========
    public async Task<DocumentResponseDto?> UploadFileAsync(int documentId, IFormFile file)
    {
        var document = await _context.StudentDocuments
            .FirstOrDefaultAsync(d => d.IdStudentDocument == documentId);

        if (document == null)
            return null;

        var directory = Path.Combine(
            _uploadPath,
            document.IdStudent.ToString(),
            document.IdStudentApplication.ToString());

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var uniqueFileName = $"{document.IdDocumentType}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(directory, uniqueFileName);

        // Удаляем старый файл если был
        if (!string.IsNullOrEmpty(document.FilePath) && File.Exists(document.FilePath))
            File.Delete(document.FilePath);

        // Сохраняем на диск
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Обновляем запись
        document.FilePath = filePath;
        document.FileName = file.FileName;
        document.ContentType = file.ContentType;
        document.FileSize = file.Length;
        document.UploadStatus = UploadStatus.Uploaded;
        document.UploadedAt = DateTime.Now;
        document.VerificationStatus = VerificationStatus.Pending;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "File uploaded for document {Id}: {FileName}", documentId, file.FileName);

        // Чтобы вернуть правильное имя документа, нужно запросить его у менеджера
        return await GetByIdAsync(documentId);
    }

    // ========== Скачивание файла ==========
    public async Task<(byte[] FileBytes, string FileName, string ContentType)?> DownloadFileAsync(
        int documentId)
    {
        var document = await _context.StudentDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.IdStudentDocument == documentId);

        if (document == null ||
            string.IsNullOrEmpty(document.FilePath) ||
            !File.Exists(document.FilePath))
            return null;

        var bytes = await File.ReadAllBytesAsync(document.FilePath);

        return (
            bytes,
            document.FileName ?? "document",
            document.ContentType ?? "application/octet-stream"
        );
    }

    // ========== Документы по заявке ==========
    public async Task<List<DocumentResponseDto>> GetByApplicationAsync(int studentApplicationId)
    {
        var documents = await _context.StudentDocuments
            .AsNoTracking()
            .Where(d => d.IdStudentApplication == studentApplicationId)
            .OrderBy(d => d.IdDocumentType)
            .ToListAsync();

        // Запрашиваем актуальные названия у менеджера
        var application = await _context.StudentApplications.FindAsync(studentApplicationId);

        List<DocumentTypeDto>? documentTypes = null;

        if (application?.IdSpecialization != null)
        {
            var specDocs = await _managerApi.GetDocumentsForSpecializationAsync(application.IdSpecialization.Value);

            if (specDocs != null && specDocs.Documents.Any())
            {
                documentTypes = specDocs.Documents.Select(d => new DocumentTypeDto
                {
                    IdDocumentType = d.IdDocumentType,
                    Name = d.DocumentName,
                    IsRequired = d.Mandatory
                }).ToList();
            }
        }

        return documents.Select(d => MapToResponseDto(d, documentTypes)).ToList();
    }

    // ========== Все документы студента ==========
    public async Task<List<DocumentResponseDto>> GetByStudentAsync(int studentId)
    {
        var documents = await _context.StudentDocuments
            .AsNoTracking()
            .Where(d => d.IdStudent == studentId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

        // Для списка всех документов студента мы не можем легко подтянуть названия от менеджера,
        // так как они могут относиться к разным заявкам и специализациям.
        // Оставляем как есть (названия будут null, фронтенд покажет "Документ #ID").
        return documents.Select(d => MapToResponseDto(d)).ToList();
    }

    // ========== Непроверенные документы по заявке (для менеджера) ==========
    public async Task<List<DocumentResponseDto>> GetPendingByApplicationAsync(int studentApplicationId)
    {
        var documents = await _context.StudentDocuments
            .AsNoTracking()
            .Where(d => d.IdStudentApplication == studentApplicationId && d.UploadStatus == UploadStatus.Uploaded && d.VerificationStatus == VerificationStatus.Pending)
            .OrderBy(d => d.IdDocumentType)
            .ToListAsync();

        // Запрашиваем актуальные названия у менеджера
        var application = await _context.StudentApplications.FindAsync(studentApplicationId);
        List<DocumentTypeDto>? documentTypes = null;

        if (application?.IdSpecialization != null)
        {
            var specDocs = await _managerApi.GetDocumentsForSpecializationAsync(application.IdSpecialization.Value);
            if (specDocs != null && specDocs.Documents.Any())
            {
                documentTypes = specDocs.Documents.Select(d => new DocumentTypeDto
                {
                    IdDocumentType = d.IdDocumentType,
                    Name = d.DocumentName,
                    IsRequired = d.Mandatory
                }).ToList();
            }
        }

        return documents.Select(d => MapToResponseDto(d, documentTypes)).ToList();
    }

    // ========== Обновление статуса проверки (от менеджера) ==========
    public async Task<DocumentResponseDto?> UpdateVerificationStatusAsync(
        int documentId,
        VerificationStatus status,
        string? comment = null)
    {
        var document = await _context.StudentDocuments
            .FirstOrDefaultAsync(d => d.IdStudentDocument == documentId);

        if (document == null)
            return null;

        document.VerificationStatus = status;
        document.ManagerComment = comment; // Сохраняем комментарий

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Document {Id} verification → {Status}",
            documentId,
            status);

        return await GetByIdAsync(documentId);
    }

    // ========== Обновление статуса договора ==========
    public async Task<DocumentResponseDto?> UpdateContractStatusAsync(
        int documentId, ContractStatus status)
    {
        var document = await _context.StudentDocuments
            .FirstOrDefaultAsync(d => d.IdStudentDocument == documentId);

        if (document == null)
            return null;

        document.ContractStatus = status;
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Document {Id} contract → {Status}", documentId, status);

        return await GetByIdAsync(documentId);
    }

    // ========== Удаление файла ==========
    public async Task<DocumentResponseDto?> DeleteFileAsync(int documentId)
    {
        var document = await _context.StudentDocuments
            .FirstOrDefaultAsync(d => d.IdStudentDocument == documentId);

        if (document == null)
            return null;

        // Удаляем файл с диска
        if (!string.IsNullOrEmpty(document.FilePath) && File.Exists(document.FilePath))
        {
            File.Delete(document.FilePath);
            _logger.LogInformation("File deleted from disk: {Path}", document.FilePath);
        }

        // Сбрасываем поля
        document.FilePath = null;
        document.FileName = null;
        document.ContentType = null;
        document.FileSize = null;
        document.UploadStatus = UploadStatus.NotUploaded;
        document.UploadedAt = null;
        document.VerificationStatus = VerificationStatus.Pending;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Document {Id} file removed, status reset", documentId);

        return await GetByIdAsync(documentId);
    }

    // ========== Получение одного документа ==========
    public async Task<DocumentResponseDto?> GetByIdAsync(int documentId)
    {
        var document = await _context.StudentDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.IdStudentDocument == documentId);

        if (document == null)
            return null;

        // Запрашиваем актуальное название у менеджера
        var application = await _context.StudentApplications.FindAsync(document.IdStudentApplication);
        List<DocumentTypeDto>? documentTypes = null;

        if (application?.IdSpecialization != null)
        {
            var specDocs = await _managerApi.GetDocumentsForSpecializationAsync(application.IdSpecialization.Value);
            if (specDocs != null && specDocs.Documents.Any())
            {
                documentTypes = specDocs.Documents.Select(d => new DocumentTypeDto
                {
                    IdDocumentType = d.IdDocumentType,
                    Name = d.DocumentName,
                    IsRequired = d.Mandatory
                }).ToList();
            }
        }

        return MapToResponseDto(document, documentTypes);
    }

    // ========== Маппинг ==========
    private DocumentResponseDto MapToResponseDto(
        StudentDocument d,
        List<DocumentTypeDto>? documentTypes = null)
    {
        // Ищем название и обязательность ТОЛЬКО в переданном списке от менеджера
        var typeInfo = documentTypes?.FirstOrDefault(t => t.IdDocumentType == d.IdDocumentType);

        return new DocumentResponseDto
        {
            IdStudentDocument = d.IdStudentDocument,
            IdStudentApplication = d.IdStudentApplication,
            IdStudent = d.IdStudent,
            IdDocumentType = d.IdDocumentType,
            DocumentTypeName = typeInfo?.Name, // Будет null, если менеджер не вернул список
            IdSpecialization = d.IdSpecialization,
            UploadStatus = d.UploadStatus,
            VerificationStatus = d.VerificationStatus,
            ContractStatus = d.ContractStatus,
            UploadedAt = d.UploadedAt,
            FileName = d.FileName,
            FileSize = d.FileSize,
            IsRequired = typeInfo?.IsRequired ?? false,
            ManagerComment = d.ManagerComment // Комментарий менеджера
        };
    }
}