using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Contracts.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

public class QuestionnaireService
{
    private readonly AppDbContext _context;
    private readonly ILogger<QuestionnaireService> _logger;

    public QuestionnaireService(AppDbContext context, ILogger<QuestionnaireService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ========== Создание анкеты ==========
    public async Task<QuestionnaireResponseDto?> CreateQuestionnaireAsync(CreateQuestionnaireDto dto)
    {
        try
        {
            // Проверяем, что студент существует
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdStudent == dto.IdStudent);

            if (student == null)
                return null;

            var questionnaire = MapToEntity(dto);
            questionnaire.CreatedAt = DateTime.Now;
            questionnaire.UpdatedAt = DateTime.Now;

            _context.Questionnaires.Add(questionnaire);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Questionnaire {Id} created for student {StudentId}",
                questionnaire.IdQuestionnaire, dto.IdStudent);

            return await GetQuestionnaireByIdAsync(questionnaire.IdQuestionnaire);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating questionnaire");
            throw;
        }
    }

    // ========== Получение по ID ==========
    public async Task<QuestionnaireResponseDto?> GetQuestionnaireByIdAsync(int id)
    {
        var questionnaire = await GetFullQuestionnaireQuery()
            .FirstOrDefaultAsync(q => q.IdQuestionnaire == id);

        if (questionnaire == null)
            return null;

        return MapToResponseDto(questionnaire);
    }

    // ========== Все анкеты студента ==========
    public async Task<List<QuestionnaireResponseDto>> GetStudentQuestionnairesAsync(int studentId)
    {
        var questionnaires = await GetFullQuestionnaireQuery()
            .Where(q => q.IdStudent == studentId)
            .ToListAsync();

        return questionnaires.Select(q => MapToResponseDto(q)).ToList();
    }

    // ========== Обновление анкеты ==========
    public async Task<QuestionnaireResponseDto?> UpdateQuestionnaireAsync(int id, CreateQuestionnaireDto dto)
    {
        var questionnaire = await _context.Questionnaires
            .Include(q => q.PsychologicalQuestions)
            .Include(q => q.Relatives)
            .Include(q => q.Educations)
            .Include(q => q.PlacePractices)
            .Include(q => q.PlaceWorks)
            .Include(q => q.Skills)
            .Include(q => q.StudentProjects)
            .Include(q => q.PracticePriorities)
            .FirstOrDefaultAsync(q => q.IdQuestionnaire == id);

        if (questionnaire == null)
            return null;

        // Удаляем старые связанные данные
        if (questionnaire.PsychologicalQuestions != null)
            _context.PsychologicalQuestions.Remove(questionnaire.PsychologicalQuestions);

        _context.Relatives.RemoveRange(questionnaire.Relatives);
        _context.Educations.RemoveRange(questionnaire.Educations);
        _context.PlacePractices.RemoveRange(questionnaire.PlacePractices);
        _context.PlaceWorks.RemoveRange(questionnaire.PlaceWorks);
        _context.Skills.RemoveRange(questionnaire.Skills);
        _context.StudentProjects.RemoveRange(questionnaire.StudentProjects);
        _context.PracticePriorities.RemoveRange(questionnaire.PracticePriorities);

        // Обновляем основные поля
        UpdateEntityFromDto(questionnaire, dto);
        questionnaire.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Questionnaire {Id} updated", id);

        return await GetQuestionnaireByIdAsync(id);
    }

    // ========== Удаление анкеты ==========
    public async Task<bool> DeleteQuestionnaireAsync(int id)
    {
        var questionnaire = await _context.Questionnaires.FindAsync(id);
        if (questionnaire == null)
            return false;

        _context.Questionnaires.Remove(questionnaire);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Questionnaire {Id} deleted", id);
        return true;
    }

    // ========== Общий запрос с Include ==========
    private IQueryable<Questionnaire> GetFullQuestionnaireQuery()
    {
        return _context.Questionnaires
            .AsNoTracking()
            .Include(q => q.Student)
            .Include(q => q.PsychologicalQuestions)
            .Include(q => q.Relatives)
            .Include(q => q.Educations)
            .Include(q => q.PlacePractices)
            .Include(q => q.PlaceWorks)
            .Include(q => q.Skills)
            .Include(q => q.StudentProjects)
            .Include(q => q.PracticePriorities);
    }

    // ========== Маппинг DTO → Entity ==========
    private Questionnaire MapToEntity(CreateQuestionnaireDto dto)
    {
        var questionnaire = new Questionnaire
        {
            IdStudent = dto.IdStudent,
            Citizenship = dto.Citizenship,
            Birthplace = dto.Birthplace,
            SourceInfo = dto.SourceInfo,
            VacationSideJob = dto.VacationSideJob,
            VolunteeringReadiness = dto.VolunteeringReadiness,
            CriminalLiability = dto.CriminalLiability,
            AdminLiability = dto.AdminLiability,
            ChronicConditions = dto.ChronicConditions,
            MedContraindications = dto.MedContraindications,
            Residency = dto.Residency,
            RegistrationPlace = dto.RegistrationPlace,
            DataProcessingConsent = dto.DataProcessingConsent,
            // НОВОЕ
            DesiredPracticeAreaIds = dto.DesiredPracticeAreaIds,
            OtherDesiredPracticeArea = dto.OtherDesiredPracticeArea,
            WhatToLearn = dto.WhatToLearn,
            PracticeWishes = dto.PracticeWishes,
            ThesisTopic = dto.ThesisTopic,
        };

        if (dto.PsychologicalQuestions != null)
        {
            questionnaire.PsychologicalQuestions = new PsychologicalQuestions
            {
                LateInstances = dto.PsychologicalQuestions.LateInstances,
                ValuedQualities = dto.PsychologicalQuestions.ValuedQualities,
                UnacceptableQualities = dto.PsychologicalQuestions.UnacceptableQualities,
                Friendliness = dto.PsychologicalQuestions.Friendliness,
                SubordinateAction = dto.PsychologicalQuestions.SubordinateAction,
                WorkTimeDedication = dto.PsychologicalQuestions.WorkTimeDedication,
                StressfulWorkReadiness = dto.PsychologicalQuestions.StressfulWorkReadiness,
                DisciplineImportance = dto.PsychologicalQuestions.DisciplineImportance
            };
        }

        questionnaire.Relatives = dto.Relatives.Select(r => new Relative
        {
            RelationDegree = r.RelationDegree,
            Surname = r.Surname,
            Name = r.Name,
            Patronymic = r.Patronymic,
            Birthdate = r.Birthdate,
            PlaceStudy = r.PlaceStudy,
            PlaceWork = r.PlaceWork
        }).ToList();

        questionnaire.Educations = dto.Educations.Select(e => new Education
        {
            DegreeOfEducation = e.DegreeOfEducation,
            EducationalInstitution = e.EducationalInstitution,
            Faculty = e.Faculty,
            Specialization = e.Specialization,
            EducationStartDate = e.EducationStartDate,
            EducationEndDate = e.EducationEndDate,
            // НОВОЕ - курс обучения
            CourseNumber = e.CourseNumber,
            GroupNumber = e.GroupNumber,
            SurnameTutor = e.SurnameTutor,
            NameTutor = e.NameTutor,
            PatronymicTutor = e.PatronymicTutor
        }).ToList();

        questionnaire.PlacePractices = dto.PlacePractices.Select(p => new PlacePractice
        {
            OrganizationName = p.OrganizationName,
            Address = p.Address,
            PhoneNumber = p.PhoneNumber,
            PracticeStartDate = p.PracticeStartDate,
            PracticeEndDate = p.PracticeEndDate,
            MainFunctions = p.MainFunctions,
            // НОВОЕ - обратная связь
            Feedback = p.Feedback
        }).ToList();

        questionnaire.PlaceWorks = dto.PlaceWorks.Select(w => new PlaceWork
        {
            OrganizationName = w.OrganizationName,
            Address = w.Address,
            PhoneNumber = w.PhoneNumber,
            WorkStartDate = w.WorkStartDate,
            WorkEndDate = w.WorkEndDate,
            Position = w.Position,
            MainFunctions = w.MainFunctions,
            ReasonForDismissal = w.ReasonForDismissal
        }).ToList();

        questionnaire.Skills = dto.Skills.Select(s => new Skill
        {
            SkillName = s.SkillName
        }).ToList();

        questionnaire.StudentProjects = dto.StudentProjects.Select(p => new StudentProject
        {
            ProjectName = p.ProjectName,
            DateParticipation = p.DateParticipation,
            Organizer = p.Organizer,
            IsOurOrganizationEvent = p.IsOurOrganizationEvent
        }).ToList();

        questionnaire.PracticePriorities = dto.PracticePriorities.Select(p => new PracticePriority
        {
            Wording = p.Wording,
            Estimation = p.Estimation
        }).ToList();

        return questionnaire;
    }

    // ========== Обновление Entity из DTO ==========
    private void UpdateEntityFromDto(Questionnaire questionnaire, CreateQuestionnaireDto dto)
    {
        questionnaire.Citizenship = dto.Citizenship;
        questionnaire.Birthplace = dto.Birthplace;
        questionnaire.SourceInfo = dto.SourceInfo;
        questionnaire.VacationSideJob = dto.VacationSideJob;
        questionnaire.VolunteeringReadiness = dto.VolunteeringReadiness;
        questionnaire.CriminalLiability = dto.CriminalLiability;
        questionnaire.AdminLiability = dto.AdminLiability;
        questionnaire.ChronicConditions = dto.ChronicConditions;
        questionnaire.MedContraindications = dto.MedContraindications;
        questionnaire.Residency = dto.Residency;
        questionnaire.RegistrationPlace = dto.RegistrationPlace;
        questionnaire.DataProcessingConsent = dto.DataProcessingConsent;
        // НОВОЕ
        questionnaire.DesiredPracticeAreaIds = dto.DesiredPracticeAreaIds; 
        questionnaire.OtherDesiredPracticeArea = dto.OtherDesiredPracticeArea; 
        questionnaire.WhatToLearn = dto.WhatToLearn; 
        questionnaire.PracticeWishes = dto.PracticeWishes; 
        questionnaire.ThesisTopic = dto.ThesisTopic;

        if (dto.PsychologicalQuestions != null)
        {
            questionnaire.PsychologicalQuestions = new PsychologicalQuestions
            {
                IdQuestionnaire = questionnaire.IdQuestionnaire,
                LateInstances = dto.PsychologicalQuestions.LateInstances,
                ValuedQualities = dto.PsychologicalQuestions.ValuedQualities,
                UnacceptableQualities = dto.PsychologicalQuestions.UnacceptableQualities,
                Friendliness = dto.PsychologicalQuestions.Friendliness,
                SubordinateAction = dto.PsychologicalQuestions.SubordinateAction,
                WorkTimeDedication = dto.PsychologicalQuestions.WorkTimeDedication,
                StressfulWorkReadiness = dto.PsychologicalQuestions.StressfulWorkReadiness,
                DisciplineImportance = dto.PsychologicalQuestions.DisciplineImportance
            };
        }

        questionnaire.Relatives = dto.Relatives.Select(r => new Relative
        {
            IdQuestionnaire = questionnaire.IdQuestionnaire,
            RelationDegree = r.RelationDegree,
            Surname = r.Surname,
            Name = r.Name,
            Patronymic = r.Patronymic,
            Birthdate = r.Birthdate,
            PlaceStudy = r.PlaceStudy,
            PlaceWork = r.PlaceWork
        }).ToList();

        questionnaire.Educations = dto.Educations.Select(e => new Education
        {
            IdQuestionnaire = questionnaire.IdQuestionnaire,
            DegreeOfEducation = e.DegreeOfEducation,
            EducationalInstitution = e.EducationalInstitution,
            Faculty = e.Faculty,
            Specialization = e.Specialization,
            EducationStartDate = e.EducationStartDate,
            EducationEndDate = e.EducationEndDate,
            // НОВОЕ
            CourseNumber = e.CourseNumber,
            GroupNumber = e.GroupNumber,
            SurnameTutor = e.SurnameTutor,
            NameTutor = e.NameTutor,
            PatronymicTutor = e.PatronymicTutor
        }).ToList();

        questionnaire.PlacePractices = dto.PlacePractices.Select(p => new PlacePractice
        {
            IdQuestionnaire = questionnaire.IdQuestionnaire,
            OrganizationName = p.OrganizationName,
            Address = p.Address,
            PhoneNumber = p.PhoneNumber,
            PracticeStartDate = p.PracticeStartDate,
            PracticeEndDate = p.PracticeEndDate,
            MainFunctions = p.MainFunctions,
            // НОВОЕ
            Feedback = p.Feedback,
        }).ToList();

        questionnaire.PlaceWorks = dto.PlaceWorks.Select(w => new PlaceWork
        {
            IdQuestionnaire = questionnaire.IdQuestionnaire,
            OrganizationName = w.OrganizationName,
            Address = w.Address,
            PhoneNumber = w.PhoneNumber,
            WorkStartDate = w.WorkStartDate,
            WorkEndDate = w.WorkEndDate,
            Position = w.Position,
            MainFunctions = w.MainFunctions,
            ReasonForDismissal = w.ReasonForDismissal
        }).ToList();

        questionnaire.Skills = dto.Skills.Select(s => new Skill
        {
            IdQuestionnaire = questionnaire.IdQuestionnaire,
            SkillName = s.SkillName
        }).ToList();

        questionnaire.StudentProjects = dto.StudentProjects.Select(p => new StudentProject
        {
            IdQuestionnaire = questionnaire.IdQuestionnaire,
            ProjectName = p.ProjectName,
            DateParticipation = p.DateParticipation,
            Organizer = p.Organizer,
            IsOurOrganizationEvent = p.IsOurOrganizationEvent
        }).ToList();

        questionnaire.PracticePriorities = dto.PracticePriorities.Select(p => new PracticePriority
        {
            IdQuestionnaire = questionnaire.IdQuestionnaire,
            Wording = p.Wording,
            Estimation = p.Estimation
        }).ToList();
    }

    // ========== Маппинг Entity → ResponseDto ==========
    private QuestionnaireResponseDto MapToResponseDto(Questionnaire q)
    {
        var dto = new QuestionnaireResponseDto
        {
            IdQuestionnaire = q.IdQuestionnaire,
            IdStudent = q.IdStudent,
            Citizenship = q.Citizenship,
            Birthplace = q.Birthplace,
            SourceInfo = q.SourceInfo,
            VacationSideJob = q.VacationSideJob,
            VolunteeringReadiness = q.VolunteeringReadiness,
            CriminalLiability = q.CriminalLiability,
            AdminLiability = q.AdminLiability,
            ChronicConditions = q.ChronicConditions,
            MedContraindications = q.MedContraindications,
            Residency = q.Residency,
            RegistrationPlace = q.RegistrationPlace,
            DataProcessingConsent = q.DataProcessingConsent,
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt,
            // НОВОЕ
            DesiredPracticeAreaIds = q.DesiredPracticeAreaIds,
            OtherDesiredPracticeArea = q.OtherDesiredPracticeArea,
            WhatToLearn = q.WhatToLearn,
            PracticeWishes = q.PracticeWishes,
            ThesisTopic = q.ThesisTopic,
        };
        if (q.Student != null)
        {
            dto.StudentFullName = $"{q.Student.Surname} {q.Student.Name} {q.Student.Patronymic}".Trim();
            dto.StudentBirthdate = q.Student.Birthdate;
            dto.StudentPhone = q.Student.PhoneNumber;
            dto.StudentEmail = q.Student.Email;
        }
        if (q.PsychologicalQuestions != null)
        {
            dto.PsychologicalQuestions = new PsychologicalQuestionsDto
            {
                LateInstances = q.PsychologicalQuestions.LateInstances,
                ValuedQualities = q.PsychologicalQuestions.ValuedQualities,
                UnacceptableQualities = q.PsychologicalQuestions.UnacceptableQualities,
                Friendliness = q.PsychologicalQuestions.Friendliness,
                SubordinateAction = q.PsychologicalQuestions.SubordinateAction,
                WorkTimeDedication = q.PsychologicalQuestions.WorkTimeDedication,
                StressfulWorkReadiness = q.PsychologicalQuestions.StressfulWorkReadiness,
                DisciplineImportance = q.PsychologicalQuestions.DisciplineImportance
            };
        }
        dto.Relatives = q.Relatives.Select(r => new RelativeResponseDto
        {
            IdRelative = r.IdRelative,
            RelationDegree = r.RelationDegree,
            Surname = r.Surname,
            Name = r.Name,
            Patronymic = r.Patronymic,
            Birthdate = r.Birthdate,
            PlaceStudy = r.PlaceStudy,
            PlaceWork = r.PlaceWork
        }).ToList();
        dto.Educations = q.Educations.Select(e => new EducationResponseDto
        {
            IdEducation = e.IdEducation,
            DegreeOfEducation = e.DegreeOfEducation,
            EducationalInstitution = e.EducationalInstitution,
            Faculty = e.Faculty,
            Specialization = e.Specialization,
            EducationStartDate = e.EducationStartDate,
            EducationEndDate = e.EducationEndDate,
            // НОВОЕ
            CourseNumber = e.CourseNumber,
            GroupNumber = e.GroupNumber,
            SurnameTutor = e.SurnameTutor,
            NameTutor = e.NameTutor,
            PatronymicTutor = e.PatronymicTutor
        }).ToList();
        dto.PlacePractices = q.PlacePractices.Select(p => new PlacePracticeResponseDto
        {
            IdPlacePractice = p.IdPlacePractice,
            OrganizationName = p.OrganizationName,
            Address = p.Address,
            PhoneNumber = p.PhoneNumber,
            PracticeStartDate = p.PracticeStartDate,
            PracticeEndDate = p.PracticeEndDate,
            MainFunctions = p.MainFunctions,
            // НОВОЕ
            Feedback = p.Feedback,
        }).ToList();
        dto.PlaceWorks = q.PlaceWorks.Select(w => new PlaceWorkResponseDto
        {
            IdPlaceWork = w.IdPlaceWork,
            OrganizationName = w.OrganizationName,
            Address = w.Address,
            PhoneNumber = w.PhoneNumber,
            WorkStartDate = w.WorkStartDate,
            WorkEndDate = w.WorkEndDate,
            Position = w.Position,
            MainFunctions = w.MainFunctions,
            ReasonForDismissal = w.ReasonForDismissal
        }).ToList();
        dto.Skills = q.Skills.Select(s => new SkillResponseDto
        {
            IdSkill = s.IdSkill,
            SkillName = s.SkillName
        }).ToList();
        dto.StudentProjects = q.StudentProjects.Select(p => new StudentProjectResponseDto
        {
            IdStudentProject = p.IdStudentProject,
            ProjectName = p.ProjectName,
            DateParticipation = p.DateParticipation,
            Organizer = p.Organizer,
            IsOurOrganizationEvent = p.IsOurOrganizationEvent
        }).ToList();
        dto.PracticePriorities = q.PracticePriorities.Select(p => new PracticePriorityResponseDto
        {
            IdPracticePriority = p.IdPracticePriority,
            Wording = p.Wording,
            Estimation = p.Estimation
        }).ToList();
        return dto;
    }
}