using StudentApi.Models;

namespace StudentApi.DTOs;

// ========== Заглушка: тип документа ==========
public class DocumentTypeDto
{
    public int IdDocumentType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
}

// ========== Ответ с данными документа ==========
public class DocumentResponseDto
{
    public int IdStudentDocument { get; set; }
    public int IdStudentApplication { get; set; }
    public int IdStudent { get; set; }
    public int? IdDocumentType { get; set; }
    public string? DocumentTypeName { get; set; }
    public int? IdSpecialization { get; set; }
    public UploadStatus UploadStatus { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public ContractStatus ContractStatus { get; set; }
    public DateTime? UploadedAt { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public bool IsRequired { get; set; }

    // НОВОЕ - комментарий менеджера по документу
    public string? ManagerComment { get; set; }
}

// ========== Обновление статуса проверки (от менеджера) ==========
public class UpdateVerificationStatusDto
{
    public VerificationStatus VerificationStatus { get; set; }
    // НОВОЕ - комментарий менеджера по документу
    public string? Comment { get; set; }
}

// ========== Обновление статуса договора ==========
public class UpdateContractStatusDto
{
    public ContractStatus ContractStatus { get; set; }
}

// ========== Документ для специализации (от менеджера) ==========
public class SpecializationDocumentRequirementDto
{
    public int IdDocumentType { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Mandatory { get; set; }
}

// ========== Ответ от менеджера — документы для специализации ==========
public class SpecializationDocumentsResponseDto
{
    public int IdSpecialization { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<SpecializationDocumentRequirementDto> Documents { get; set; } = new();
}