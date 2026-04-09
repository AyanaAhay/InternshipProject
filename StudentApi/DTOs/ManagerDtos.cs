namespace StudentApi.DTOs;

public class PracticeTypeDto
{
    public int IdPracticeType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class SpecializationDto
{
    public int IdSpecialization { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ScheduledPracticeDto
{
    public int IdScheduledPractice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SpecializationDto? Specialization { get; set; }
    public PracticeTypeDto? PracticeType { get; set; }
}