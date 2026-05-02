using System.Text;
using System.Text.Json;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

public class ManagerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ManagerApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ManagerApiClient(HttpClient httpClient, ILogger<ManagerApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // Получение типов практик
    public async Task<List<PracticeTypeDto>> GetPracticeTypesAsync()
    {
        try
        {
            // Исправленный URL согласно маршрутам Manager API
            var response = await _httpClient.GetAsync("api/v1/PracticeTypes/GetPracticeTypes");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Manager API returned {response.StatusCode} for PracticeTypes");
                return new List<PracticeTypeDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<PracticeTypeDto>>(json, _jsonOptions);
            return result ?? new List<PracticeTypeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении типов практик из Manager API");
            return new List<PracticeTypeDto>();
        }
    }

    // Получение специализаций
    public async Task<List<SpecializationDto>> GetSpecializationsAsync()
    {
        try
        {
            // Исправленный URL согласно маршрутам Manager API
            var response = await _httpClient.GetAsync("api/v1/Specialization/GetSpecializations");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Manager API returned {response.StatusCode} for Specializations");
                return new List<SpecializationDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<SpecializationDto>>(json, _jsonOptions);
            return result ?? new List<SpecializationDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении специализаций из Manager API");
            return new List<SpecializationDto>();
        }
    }

    // Получение запланированных практик
    public async Task<List<ScheduledPracticeDto>> GetScheduledPracticesAsync(
        int? practiceTypeId = null,
        int? specializationId = null)
    {
        try
        {
            // Исправленный URL согласно маршрутам Manager API
            var url = "api/v1/ScheduledPractice/GetScheduledPractices";
            var queryParams = new List<string>();

            if (practiceTypeId.HasValue)
                queryParams.Add($"practiceTypeId={practiceTypeId}");
            if (specializationId.HasValue)
                queryParams.Add($"specializationId={specializationId}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Manager API returned {response.StatusCode} for ScheduledPractices");
                return new List<ScheduledPracticeDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ScheduledPracticeDto>>(json, _jsonOptions);
            return result ?? new List<ScheduledPracticeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении запланированных практик из Manager API");
            return new List<ScheduledPracticeDto>();
        }
    }

    // Отправка заявки в систему менеджера
    public async Task<bool> SendApplicationToManagerAsync(StudentApplication application, Student student)
    {
        try
        {
            // Создаем DTO для отправки менеджеру
            var applicationDto = new
            {
                studentId = student.IdStudent,
                studentName = $"{student.Surname} {student.Name} {student.Patronymic}".Trim(),
                studentEmail = student.Email,
                studentPhone = student.PhoneNumber,
                idScheduledPractice = application.IdScheduledPractice,
                idPracticeType = application.IdPracticeType,
                idSpecialization = application.IdSpecialization,
                startDate = application.StartDate,
                endDate = application.EndDate,
                applicationId = application.IdStudentApplication,
                submittedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(applicationDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Отправляем в Manager API
            var response = await _httpClient.PostAsync("api/v1/ManagerApplication/ReceiveApplication", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Application {application.IdStudentApplication} successfully sent to Manager");
                return true;
            }

            _logger.LogWarning($"Failed to send application to Manager. Status: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending application to Manager API");
            return false;
        }
    }

    // Получение статуса заявки от менеджера
    public async Task<ApplicationStatusDto?> GetApplicationStatusAsync(int applicationId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/ManagerApplication/GetApplicationStatus/{applicationId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApplicationStatusDto>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application status from Manager API");
            return null;
        }
    }



    // Отправка анкеты в систему менеджера
    public async Task<bool> SendQuestionnaireToManagerAsync(QuestionnaireResponseDto questionnaire)
    {
        try
        {
            var json = JsonSerializer.Serialize(questionnaire, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/ManagerQuestionnaire/ReceiveQuestionnaire", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Questionnaire {questionnaire.IdQuestionnaire} successfully sent to Manager");
                return true;
            }

            _logger.LogWarning($"Failed to send questionnaire to Manager. Status: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending questionnaire to Manager API");
            return false;
        }
    }


    // Получить свободные слоты менеджера
    // GET /api/v1/ManagerSlot/getFreeSlots
    public async Task<List<ManagerSlotDto>> GetFreeManagerSlotsAsync()
    {
        try
        {
            // var response = await _httpClient.GetAsync("api/v1/ManagerSlot/getFreeSlots");
            var response = await _httpClient.GetAsync("api/v1/ManagerSlot/getFreeSlots");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Manager API returned {Status} for free slots", response.StatusCode);
                return new List<ManagerSlotDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ManagerSlotDto>>(json, _jsonOptions);

            return result ?? new List<ManagerSlotDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free manager slots");
            return new List<ManagerSlotDto>();
        }
    }

    // Записаться на собеседование с менеджером
    // POST /api/v1/ManagerInterview/signUpStudentForInterview
    public async Task<bool> SignUpForManagerInterviewAsync(SignUpForManagerInterviewDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "api/v1/ManagerInterview/signUpStudentForInterview", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Student {StudentId} signed up for manager interview slot {SlotId}",
                    dto.IdStudent,
                    dto.IdSlot);

                return true;
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Sign up failed: {Status} — {Error}", response.StatusCode, error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing up for manager interview");
            return false;
        }
    }

    // Получить список документов для специализации
    // GET /api/v1/Specialization/GetDocumentsForSpecialization/{id}
    public async Task<SpecializationDocumentsResponseDto?> GetDocumentsForSpecializationAsync(int specializationId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"api/v1/Specialization/GetDocumentsForSpecialization/{specializationId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Manager API returned {Status} for specialization documents {Id}",
                    response.StatusCode,
                    specializationId);

                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SpecializationDocumentsResponseDto>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents for specialization {Id}", specializationId);
            return null;
        }
    }


    // НОВОЕ (добавление собеседования с менеджером) - 26.04.2026

    // Получить менеджера по заявке студента
    // GET /api/v1/StudentApplication/GetManagerByApplication/{idStudentApplication}
    public async Task<ManagerInfoDto?> GetManagerByApplicationAsync(int studentApplicationId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"api/v1/StudentApplication/GetManagerByApplication/{studentApplicationId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Manager API returned {Status} for GetManagerByApplication {Id}",
                    response.StatusCode, studentApplicationId);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ManagerInfoDto>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manager by application {Id}", studentApplicationId);
            return null;
        }
    }

    // Получить свободные слоты менеджера
    // GET /api/v1/ManagerSlot/GetFreeSlotsByManagerId/{managerId}
    public async Task<List<ManagerSlotDetailDto>> GetFreeSlotsByManagerAsync(int managerId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"api/v1/ManagerSlot/GetFreeSlotsByManagerId/{managerId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Manager API returned {Status} for GetFreeSlotsByManager {Id}",
                    response.StatusCode, managerId);
                return new List<ManagerSlotDetailDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ManagerSlotDetailDto>>(json, _jsonOptions);
            return result ?? new List<ManagerSlotDetailDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free slots for manager {Id}", managerId);
            return new List<ManagerSlotDetailDto>();
        }
    }

    // Записаться на собеседование с менеджером
    // POST /api/v1/ManagerInterview/Create
    public async Task<ManagerInterviewResponseDto?> CreateManagerInterviewAsync(
        CreateManagerInterviewDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/ManagerInterview/Create", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "Manager interview creation failed: {Status} — {Error}",
                    response.StatusCode, error);
                return null;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ManagerInterviewResponseDto>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating manager interview");
            return null;
        }
    }

    // Получить интервью по заявке студента
    // GET /api/v1/ManagerInterview/GetByApplicationId/{applicationId}
    public async Task<ManagerInterviewResponseDto?> GetManagerInterviewByApplicationAsync(
        int studentApplicationId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"api/v1/ManagerInterview/GetByApplicationId/{studentApplicationId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ManagerInterviewResponseDto>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manager interview for application {Id}", studentApplicationId);
            return null;
        }
    }

    // Получить список направлений от менеджера
    // GET /api/v1/PracticeArea/GetAll
    public async Task<List<PracticeAreaDto>> GetPracticeAreasAsync() { 
        try { 
            var response = await _httpClient.GetAsync("api/v1/PracticeArea/GetAll"); 
            if (!response.IsSuccessStatusCode) return new List<PracticeAreaDto>(); 
            var json = await response.Content.ReadAsStringAsync(); 
            return JsonSerializer.Deserialize<List<PracticeAreaDto>>(json, _jsonOptions) ?? new List<PracticeAreaDto>(); 
        } catch { 
            return new List<PracticeAreaDto>(); 
        }
    }
}