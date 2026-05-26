namespace YourApp.Features.Weather.Services;

using YourApp.Features.Weather.Models;
using YourApp.Features.Weather.Interfaces;
using YourApp.Features.Offline.Database;
using Microsoft.EntityFrameworkCore;

public class WeatherPreferenceService : IWeatherPreferenceService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<WeatherPreferenceService> _logger;

    public WeatherPreferenceService(
        AppDbContext dbContext,
        ILogger<WeatherPreferenceService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<WeatherPreference> GetPreferencesAsync()
    {
        try
        {
            var preferences = await _dbContext.WeatherPreferences.FirstOrDefaultAsync();
            
            if (preferences == null)
            {
                preferences = new WeatherPreference();
                await _dbContext.WeatherPreferences.AddAsync(preferences);
                await _dbContext.SaveChangesAsync();
            }

            return preferences;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving preferences: {ex.Message}");
            return new WeatherPreference();
        }
    }

    public async Task SavePreferencesAsync(WeatherPreference preferences)
    {
        try
        {
            var existing = await _dbContext.WeatherPreferences.FirstOrDefaultAsync();
            
            if (existing == null)
            {
                preferences.UpdatedAt = DateTime.UtcNow;
                await _dbContext.WeatherPreferences.AddAsync(preferences);
            }
            else
            {
                existing.TemperatureUnit = preferences.TemperatureUnit;
                existing.WindSpeedUnit = preferences.WindSpeedUnit;
                existing.Use24HourFormat = preferences.Use24HourFormat;
                existing.EnableNotifications = preferences.EnableNotifications;
                existing.RefreshIntervalMinutes = preferences.RefreshIntervalMinutes;
                existing.UpdatedAt = DateTime.UtcNow;
                _dbContext.WeatherPreferences.Update(existing);
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Weather preferences saved");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving preferences: {ex.Message}");
            throw;
        }
    }
}