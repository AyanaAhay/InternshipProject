namespace StudentApi.DTOs;

public class ApplicationStatusDto
{
    public int ApplicationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ManagerComment { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public bool IsApproved { get; set; }
}