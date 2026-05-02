using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

[Table("StudentDocument")]
public class StudentDocument
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdStudentDocument { get; set; }
    [Required]
    public int IdStudentApplication { get; set; }
    [ForeignKey(nameof(IdStudentApplication))]
    public virtual StudentApplication? StudentApplication { get; set; }
    [Required]
    public int IdStudent { get; set; }
    [ForeignKey(nameof(IdStudent))]
    public virtual Student? Student { get; set; }
    // Заглушка — пока int, позже будет приходить из системы менеджера
    public int? IdDocumentType { get; set; }
    // Заглушка — позже будет приходить из системы менеджера
    public int? IdSpecialization { get; set; }
    [Required]
    public UploadStatus UploadStatus { get; set; } = UploadStatus.NotUploaded;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public ContractStatus ContractStatus { get; set; } = ContractStatus.NotReceived;
    // Дата загрузки файла (null если ещё не загружен)
    [Column(TypeName = "timestamp without time zone")]
    public DateTime? UploadedAt { get; set; }
    // Относительный путь к файлу на сервере
    [MaxLength(500)]
    public string? FilePath { get; set; }

    // НОВОЕ - комментарий менеджера по документу
    [Column(TypeName = "text")] 
    public string? ManagerComment { get; set; }

    // Оригинальное имя файла (чтобы при скачивании вернуть правильное имя)
    [MaxLength(255)]
    public string? FileName { get; set; }
    // MIME-тип файла (application/pdf, image/jpeg и т.д.)
    [MaxLength(100)]
    public string? ContentType { get; set; }
    // Размер файла в байтах
    public long? FileSize { get; set; }
}