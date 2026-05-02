using System.Text;
using System.Text.Json;
using InternshipManager.Api.Contracts.Enums;
using InternshipManager.Api.Contracts.DTOs.SupervisorApplication;
using InternshipManager.Api.Contracts.DTOs.StudentSupervisorApplication;
using InternshipManager.Api.Contracts.DTOs.InterviewSlot;
using InternshipManager.Api.Contracts.DTOs.Interview;
using StudentApi.Contracts.DTOs;

namespace StudentApi.Services;

public class SupervisorApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SupervisorApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public SupervisorApiClient(
        HttpClient httpClient,
        ILogger<SupervisorApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // ===== Связки студент-руководитель =====
    // GET /api/v1/StudentSupervisorApplication/student/{studentApplicationId}
    public async Task<List<StudentSupervisorLinkDetailDto>> GetStudentLinksAsync(
        int studentApplicationId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"api/v1/StudentSupervisorApplication/student/{studentApplicationId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Supervisor API returned {Status} for student links",
                    response.StatusCode);

                return new List<StudentSupervisorLinkDetailDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<StudentSupervisorLinkDetailDto>>(
                json,
                _jsonOptions);

            return result ?? new List<StudentSupervisorLinkDetailDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student links");
            return new List<StudentSupervisorLinkDetailDto>();
        }
    }

    // PUT /api/v1/StudentSupervisorApplication/{supAppId}/{studAppId}/choose
    public async Task<bool> ChooseSupervisorAsync(
        int supervisorApplicationId,
        int studentApplicationId)
    {
        try
        {
            var response = await _httpClient.PutAsync(
                $"api/v1/StudentSupervisorApplication/{supervisorApplicationId}/{studentApplicationId}/choose",
                null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Student {StdAppId} chose supervisor application {SupAppId}",
                    studentApplicationId,
                    supervisorApplicationId);

                return true;
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning(
                "Choose failed: {Status} — {Error}",
                response.StatusCode,
                error);

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error choosing supervisor");
            return false;
        }
    }

    // ===== Заявки руководителя =====
    // GET /api/v1/SupervisorApplication/{id}
    public async Task<SupervisorApplicationDto?> GetSupervisorApplicationAsync(
        int supervisorApplicationId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"api/v1/SupervisorApplication/{supervisorApplicationId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Supervisor API returned {Status} for supervisor application {Id}",
                    response.StatusCode,
                    supervisorApplicationId);

                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SupervisorApplicationDto>(
                json,
                _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supervisor application {Id}", supervisorApplicationId);
            return null;
        }
    }

    // ===== Слоты собеседований =====
    // GET /api/v1/InterviewSlot/available/{supervisorId}?studentApplicationId={id}
    public async Task<List<AvailableInterviewSlotDto>> GetAvailableSlotsAsync(
        int supervisorId,
        int studentApplicationId)
    {
        try
        {
            var url = $"api/v1/InterviewSlot/available/{supervisorId}" +
                      $"?studentApplicationId={studentApplicationId}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Supervisor API returned {Status} for available slots",
                    response.StatusCode);

                return new List<AvailableInterviewSlotDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<AvailableInterviewSlotDto>>(
                json,
                _jsonOptions);

            return result ?? new List<AvailableInterviewSlotDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available slots");
            return new List<AvailableInterviewSlotDto>();
        }
    }

    //// PUT /api/v1/InterviewSlot/{slotId}/book
    //public async Task<BookSlotResponseDto?> BookSlotAsync(
    //    int slotId,
    //    int studentApplicationId)
    //{
    //    try
    //    {
    //        var body = new BookSlotRequestDto
    //        {
    //            IdStudentApplication = studentApplicationId
    //        };

    //        var json = JsonSerializer.Serialize(body, _jsonOptions);
    //        var content = new StringContent(json, Encoding.UTF8, "application/json");

    //        var response = await _httpClient.PutAsync(
    //            $"api/v1/InterviewSlot/{slotId}/book",
    //            content);

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            var error = await response.Content.ReadAsStringAsync();
    //            _logger.LogWarning(
    //                "Booking failed: {Status} — {Body}",
    //                response.StatusCode,
    //                error);

    //            return null;
    //        }

    //        var responseJson = await response.Content.ReadAsStringAsync();
    //        return JsonSerializer.Deserialize<BookSlotResponseDto>(
    //            responseJson,
    //            _jsonOptions);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error booking slot");
    //        return null;
    //    }
    //}

    /// <summary>
    /// PUT /api/v1/InterviewSlot/{slotId}/book
    /// </summary>
    public async Task<BookSlotResponseDto?> BookSlotAsync(
        int slotId,
        int studentApplicationId,
        int? supervisorApplicationId = null) // НОВОЕ
    {
        try
        {
            var body = new BookSlotRequestDto
            {
                IdStudentApplication = studentApplicationId,
                IdSupervisorApplication = supervisorApplicationId // НОВОЕ
            };

            var json = JsonSerializer.Serialize(body, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(
                $"api/v1/InterviewSlot/{slotId}/book",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Booking failed: {Status} — {Body}", response.StatusCode, error);
                return null;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BookSlotResponseDto>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking slot");
            return null;
        }
    }

    // PUT /api/v1/InterviewSlot/{id}/cancel-booking
    public async Task<bool> CancelBookingAsync(
        int slotId,
        int studentApplicationId)
    {
        try
        {
            var body = new BookSlotRequestDto
            {
                IdStudentApplication = studentApplicationId
            };

            var json = JsonSerializer.Serialize(body, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(
                $"api/v1/InterviewSlot/{slotId}/cancel-booking",
                content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Booking cancelled for slot {SlotId}, student app {AppId}",
                    slotId,
                    studentApplicationId);

                return true;
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning(
                "Cancel booking failed: {Status} — {Error}",
                response.StatusCode,
                error);

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking");
            return false;
        }
    }

    // GET /api/v1/InterviewSlot/booked?studentApplicationId={id}
    public async Task<BookedInterviewSlotDto?> GetBookedSlotAsync(
        int studentApplicationId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"api/v1/InterviewSlot/booked?studentApplicationId={studentApplicationId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BookedInterviewSlotDto>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booked slot for application {Id}", studentApplicationId);
            return null;
        }
    }

    // ===== Собеседования =====
    // GET /api/v1/Interview/student/{studentApplicationId}
    public async Task<List<StudentInterviewResponseDto>> GetStudentInterviewsAsync(
        int studentApplicationId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"api/v1/Interview/student/{studentApplicationId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Supervisor API returned {Status} for student interviews",
                    response.StatusCode);

                return new List<StudentInterviewResponseDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<StudentInterviewResponseDto>>(
                json,
                _jsonOptions);

            return result ?? new List<StudentInterviewResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student interviews");
            return new List<StudentInterviewResponseDto>();
        }
    }
}