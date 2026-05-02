using System.ComponentModel.DataAnnotations;
using StudentApi.Models;

namespace StudentApi.DTOs;

/// <summary>
/// DTO для изменения статуса заявки внешними системами
/// (менеджер, руководитель)
/// </summary>
public class UpdateApplicationStatusDto
{
    [Required]
    public StudentApplicationStatus Status { get; set; }

    // Комментарий — обязателен при отказе
    public string? RejectionComment { get; set; }
}