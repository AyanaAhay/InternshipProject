using System.ComponentModel.DataAnnotations;
using StudentApi.Contracts.Enums;

namespace StudentApi.Contracts.DTOs;

public class CreateQuestionnaireDto
{
    // ИЗМЕНЕНО: анкета привязана к студенту, а не к заявке
    [Required]
    public int IdStudent { get; set; }
    [Required]
    [MaxLength(255)]
    public string Citizenship { get; set; } = string.Empty;
    [Required]
    [MaxLength(255)]
    public string Birthplace { get; set; } = string.Empty;
    [Required]
    public string SourceInfo { get; set; } = string.Empty;
    public bool VacationSideJob { get; set; }
    public bool VolunteeringReadiness { get; set; }
    public bool CriminalLiability { get; set; }
    public bool AdminLiability { get; set; }
    public string? ChronicConditions { get; set; }
    public string? MedContraindications { get; set; }
    [Required]
    public string Residency { get; set; } = string.Empty;
    [Required]
    public string RegistrationPlace { get; set; } = string.Empty;
    [Required]
    public bool DataProcessingConsent { get; set; }
    public PsychologicalQuestionsDto? PsychologicalQuestions { get; set; }
    public List<RelativeDto> Relatives { get; set; } = new();
    public List<EducationDto> Educations { get; set; } = new();
    public List<PlacePracticeDto> PlacePractices { get; set; } = new();
    public List<PlaceWorkDto> PlaceWorks { get; set; } = new();
    public List<SkillDto> Skills { get; set; } = new();
    public List<StudentProjectDto> StudentProjects { get; set; } = new();
    public List<PracticePriorityDto> PracticePriorities { get; set; } = new();

    // НОВОЕ 
    public int[]? DesiredPracticeAreaIds { get; set; }
    public string? OtherDesiredPracticeArea { get; set; }
    public string? WhatToLearn { get; set; }
    public string? PracticeWishes { get; set; }
    public string? ThesisTopic { get; set; }
}

public class QuestionnaireResponseDto
{
    public int IdQuestionnaire { get; set; }
    public int IdStudent { get; set; }
    public string StudentFullName { get; set; } = string.Empty;
    public DateTime StudentBirthdate { get; set; }
    public string StudentPhone { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string Citizenship { get; set; } = string.Empty;
    public string Birthplace { get; set; } = string.Empty;
    public string SourceInfo { get; set; } = string.Empty;
    public bool VacationSideJob { get; set; }
    public bool VolunteeringReadiness { get; set; }
    public bool CriminalLiability { get; set; }
    public bool AdminLiability { get; set; }
    public string? ChronicConditions { get; set; }
    public string? MedContraindications { get; set; }
    public string Residency { get; set; } = string.Empty;
    public string RegistrationPlace { get; set; } = string.Empty;
    public bool DataProcessingConsent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public PsychologicalQuestionsDto? PsychologicalQuestions { get; set; }
    public List<RelativeResponseDto> Relatives { get; set; } = new();
    public List<EducationResponseDto> Educations { get; set; } = new();
    public List<PlacePracticeResponseDto> PlacePractices { get; set; } = new();
    public List<PlaceWorkResponseDto> PlaceWorks { get; set; } = new();
    public List<SkillResponseDto> Skills { get; set; } = new();
    public List<StudentProjectResponseDto> StudentProjects { get; set; } = new();
    public List<PracticePriorityResponseDto> PracticePriorities { get; set; } = new();

    // НОВОЕ 
    public int[]? DesiredPracticeAreaIds { get; set; }
    public string? OtherDesiredPracticeArea { get; set; }
    public string? WhatToLearn { get; set; }
    public string? PracticeWishes { get; set; }
    public string? ThesisTopic { get; set; }
}

// Остальные DTO без изменений — оставляем как было:
public class PsychologicalQuestionsDto
{
    public string? LateInstances { get; set; }
    public string? ValuedQualities { get; set; }
    public string? UnacceptableQualities { get; set; }
    public string? Friendliness { get; set; }
    public string? SubordinateAction { get; set; }
    public string? WorkTimeDedication { get; set; }
    public string? StressfulWorkReadiness { get; set; }
    public string? DisciplineImportance { get; set; }
}

public class RelativeDto
{
    [Required]
    public string RelationDegree { get; set; } = string.Empty;
    [Required]
    public string Surname { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public DateTime? Birthdate { get; set; }
    public string? PlaceStudy { get; set; }
    public string? PlaceWork { get; set; }
}

public class RelativeResponseDto : RelativeDto
{
    public int IdRelative { get; set; }
}

public class EducationDto
{
    [Required]
    public string DegreeOfEducation { get; set; } = string.Empty;
    [Required]
    public string EducationalInstitution { get; set; } = string.Empty;
    public string? Faculty { get; set; }
    public string? Specialization { get; set; }
    public DateTime? EducationStartDate { get; set; }
    public DateTime? EducationEndDate { get; set; }
    [Required]
    public int? CourseNumber { get; set; } // курс обучения
    public string? GroupNumber { get; set; }
    public string? SurnameTutor { get; set; }
    public string? NameTutor { get; set; }
    public string? PatronymicTutor { get; set; }
}

public class EducationResponseDto : EducationDto
{
    public int IdEducation { get; set; }
}

public class PlacePracticeDto
{
    [Required]
    public string OrganizationName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? PracticeStartDate { get; set; }
    public DateTime? PracticeEndDate { get; set; }
    public string? MainFunctions { get; set; }
    public string? Feedback { get; set; } // обратная связь
}

public class PlacePracticeResponseDto : PlacePracticeDto
{
    public int IdPlacePractice { get; set; }
}

public class PlaceWorkDto
{
    [Required]
    public string OrganizationName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? WorkStartDate { get; set; }
    public DateTime? WorkEndDate { get; set; }
    public string? Position { get; set; }
    public string? MainFunctions { get; set; }
    public string? ReasonForDismissal { get; set; }
}

public class PlaceWorkResponseDto : PlaceWorkDto
{
    public int IdPlaceWork { get; set; }
}

public class SkillDto
{
    [Required]
    public string SkillName { get; set; } = string.Empty;
}

public class SkillResponseDto : SkillDto
{
    public int IdSkill { get; set; }
}

public class StudentProjectDto
{
    [Required]
    public string ProjectName { get; set; } = string.Empty;
    public DateTime? DateParticipation { get; set; }
    public string? Organizer { get; set; }
    public bool IsOurOrganizationEvent { get; set; }
}

public class StudentProjectResponseDto : StudentProjectDto
{
    public int IdStudentProject { get; set; }
}

public class PracticePriorityDto
{
    [Required]
    public string Wording { get; set; } = string.Empty;
    [Required]
    [Range(1, 10)]
    public int Estimation { get; set; }
}

public class PracticePriorityResponseDto : PracticePriorityDto
{
    public int IdPracticePriority { get; set; }
}