//using AutoMapper;
//using StudentApi.DTOs;
//using StudentApi.Models;

//namespace StudentApi.Mappings;

//public class QuestionnaireProfile : Profile
//{
//    public QuestionnaireProfile()
//    {
//        // Questionnaire mappings
//        CreateMap<CreateQuestionnaireDto, Questionnaire>();
//        CreateMap<Questionnaire, QuestionnaireResponseDto>()
//            .ForMember(dest => dest.StudentFullName, opt => opt.Ignore())
//            .ForMember(dest => dest.IdStudent, opt => opt.Ignore())
//            .ForMember(dest => dest.StudentBirthdate, opt => opt.Ignore())
//            .ForMember(dest => dest.StudentPhone, opt => opt.Ignore())
//            .ForMember(dest => dest.StudentEmail, opt => opt.Ignore());

//        // Psychological questions
//        CreateMap<PsychologicalQuestionsDto, PsychologicalQuestions>();
//        CreateMap<PsychologicalQuestions, PsychologicalQuestionsDto>();

//        // Relatives
//        CreateMap<RelativeDto, Relative>();
//        CreateMap<Relative, RelativeResponseDto>();

//        // Education
//        CreateMap<EducationDto, Education>();
//        CreateMap<Education, EducationResponseDto>();

//        // Place practice
//        CreateMap<PlacePracticeDto, PlacePractice>();
//        CreateMap<PlacePractice, PlacePracticeResponseDto>();

//        // Place work
//        CreateMap<PlaceWorkDto, PlaceWork>();
//        CreateMap<PlaceWork, PlaceWorkResponseDto>();

//        // Skills
//        CreateMap<SkillDto, Skill>();
//        CreateMap<Skill, SkillResponseDto>();

//        // Student projects
//        CreateMap<StudentProjectDto, StudentProject>();
//        CreateMap<StudentProject, StudentProjectResponseDto>();

//        // Practice priorities
//        CreateMap<PracticePriorityDto, PracticePriority>();
//        CreateMap<PracticePriority, PracticePriorityResponseDto>();
//    }
//}