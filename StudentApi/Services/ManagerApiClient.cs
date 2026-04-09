using System.Text.Json;
using StudentApi.DTOs;

namespace StudentApi.Services;

public class ManagerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ManagerApiClient> _logger;

    public ManagerApiClient(HttpClient httpClient, ILogger<ManagerApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<PracticeTypeDto>> GetPracticeTypesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/PracticeTypes");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PracticeTypeDto>>(json) ?? new List<PracticeTypeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении типов практик");
            return new List<PracticeTypeDto>();
        }
    }

    public async Task<List<SpecializationDto>> GetSpecializationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/Specializations");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<SpecializationDto>>(json) ?? new List<SpecializationDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении специализаций");
            return new List<SpecializationDto>();
        }
    }

    public async Task<List<ScheduledPracticeDto>> GetScheduledPracticesAsync(
        int? practiceTypeId = null,
        int? specializationId = null)
    {
        try
        {
            var url = "/ScheduledPractice";
            var queryParams = new List<string>();
            if (practiceTypeId.HasValue) queryParams.Add($"practiceTypeId={practiceTypeId}");
            if (specializationId.HasValue) queryParams.Add($"specializationId={specializationId}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ScheduledPracticeDto>>(json) ?? new List<ScheduledPracticeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении запланированных практик");
            return new List<ScheduledPracticeDto>();
        }
    }
}