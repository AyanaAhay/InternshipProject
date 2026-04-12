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
}