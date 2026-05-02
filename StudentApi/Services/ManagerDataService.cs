using ManagerService.Contracts.DTOs;
using ManagerService.Contracts.Enums;
using StudentApi.Contracts.DTOs;

namespace StudentApi.Services;

public class ManagerDataService
{
    private readonly ManagerApiClient _apiClient;
    private List<PracticeTypeDTO>? _cachedPracticeTypes;
    private List<SpecializationDTO>? _cachedSpecializations;
    private DateTime _cacheExpiration = DateTime.MinValue;
    private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

    public ManagerDataService(ManagerApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private bool IsCacheValid => DateTime.Now < _cacheExpiration;

    public async Task<List<PracticeTypeDTO>> GetPracticeTypesAsync(bool useCache = true)
    {
        if (useCache && IsCacheValid && _cachedPracticeTypes != null)
            return _cachedPracticeTypes;

        _cachedPracticeTypes = await _apiClient.GetPracticeTypesAsync();
        _cacheExpiration = DateTime.Now + _cacheLifetime;
        return _cachedPracticeTypes;
    }

    public async Task<List<SpecializationDTO>> GetSpecializationsAsync(bool useCache = true)
    {
        if (useCache && IsCacheValid && _cachedSpecializations != null)
            return _cachedSpecializations;

        _cachedSpecializations = await _apiClient.GetSpecializationsAsync();
        _cacheExpiration = DateTime.Now + _cacheLifetime;
        return _cachedSpecializations;
    }

    public async Task<List<ScheduledPracticeDTO>> GetScheduledPracticesAsync(
        int? practiceTypeId = null,
        int? specializationId = null)
    {
        return await _apiClient.GetScheduledPracticesAsync(practiceTypeId, specializationId);
    }
}