using System.ComponentModel.DataAnnotations;

//using StudentApi.Models;
using StudentApi.Contracts.Enums;

//namespace StudentApi.DTOs;
namespace StudentApi.Contracts.DTOs;

// DTO для изменения статуса заявки внешними системами
public class UpdateApplicationStatusDto
{
    [Required]
    public StudentApplicationStatus Status { get; set; }

    // Комментарий — обязателен при отказе
    public string? RejectionComment { get; set; }
}