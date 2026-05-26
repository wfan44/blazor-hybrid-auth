namespace YourApp.Features.Weather.Interfaces;

using YourApp.Features.Weather.Models;

public interface IWeatherService
{
    Task<WeatherResponse> GetCurrentWeatherAsync(string cityName);
    Task<WeatherResponse> GetCurrentWeatherByCoordinatesAsync(double latitude, double longitude);
    Task<ForecastResponse> GetForecastAsync(string cityName);
    Task<ForecastResponse> GetForecastByCoordinatesAsync(double latitude, double longitude);
    string GetWeatherIconUrl(string iconCode);
    double ConvertTemperature(double celsius, string unit);
    double ConvertWindSpeed(double meterPerSecond, string unit);
}

public interface ILocationService
{
    Task<Location> GetCurrentLocationAsync();
    Task<bool> RequestLocationPermissionAsync();
    bool IsLocationEnabled { get; }
}

public interface ISavedLocationService
{
    Task<List<SavedLocation>> GetAllLocationsAsync();
    Task<SavedLocation> GetDefaultLocationAsync();
    Task SaveLocationAsync(SavedLocation location);
    Task DeleteLocationAsync(int id);
    Task SetDefaultLocationAsync(int id);
}

public interface IWeatherPreferenceService
{
    Task<WeatherPreference> GetPreferencesAsync();
    Task SavePreferencesAsync(WeatherPreference preferences);
}