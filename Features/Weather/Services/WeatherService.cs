namespace YourApp.Features.Weather.Services;

using YourApp.Features.Weather.Models;
using YourApp.Features.Weather.Interfaces;
using YourApp.Features.Api.Services;
using System.Diagnostics;

public class WeatherService : IWeatherService
{
    private readonly IApiService _apiService;
    private readonly IApiCacheService _cacheService;
    private readonly ILogger<WeatherService> _logger;
    private readonly IConfiguration _configuration;

    private const int CacheDurationMinutes = 30;

    public WeatherService(
        IApiService apiService,
        IApiCacheService cacheService,
        ILogger<WeatherService> logger,
        IConfiguration configuration)
    {
        _apiService = apiService;
        _cacheService = cacheService;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<WeatherResponse> GetCurrentWeatherAsync(string cityName)
    {
        try
        {
            var apiKey = _configuration["Weather:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Weather API key not configured");

            var cacheKey = $"weather_current_{cityName}";
            var cached = await _cacheService.GetCachedResponseAsync<WeatherResponse>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation($"Weather cache hit for {cityName}");
                return cached;
            }

            var url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric";
            var response = await _apiService.GetAsync<WeatherResponse>(url);

            if (response != null)
            {
                await _cacheService.CacheResponseAsync(cacheKey, response, CacheDurationMinutes);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching weather for {cityName}: {ex.Message}");
            throw;
        }
    }

    public async Task<WeatherResponse> GetCurrentWeatherByCoordinatesAsync(double latitude, double longitude)
    {
        try
        {
            var apiKey = _configuration["Weather:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Weather API key not configured");

            var cacheKey = $"weather_current_{latitude}_{longitude}";
            var cached = await _cacheService.GetCachedResponseAsync<WeatherResponse>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation($"Weather cache hit for {latitude}, {longitude}");
                return cached;
            }

            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric";
            var response = await _apiService.GetAsync<WeatherResponse>(url);

            if (response != null)
            {
                await _cacheService.CacheResponseAsync(cacheKey, response, CacheDurationMinutes);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching weather for coordinates {latitude}, {longitude}: {ex.Message}");
            throw;
        }
    }

    public async Task<ForecastResponse> GetForecastAsync(string cityName)
    {
        try
        {
            var apiKey = _configuration["Weather:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Weather API key not configured");

            var cacheKey = $"weather_forecast_{cityName}";
            var cached = await _cacheService.GetCachedResponseAsync<ForecastResponse>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation($"Forecast cache hit for {cityName}");
                return cached;
            }

            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid={apiKey}&units=metric";
            var response = await _apiService.GetAsync<ForecastResponse>(url);

            if (response != null)
            {
                await _cacheService.CacheResponseAsync(cacheKey, response, CacheDurationMinutes);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching forecast for {cityName}: {ex.Message}");
            throw;
        }
    }

    public async Task<ForecastResponse> GetForecastByCoordinatesAsync(double latitude, double longitude)
    {
        try
        {
            var apiKey = _configuration["Weather:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Weather API key not configured");

            var cacheKey = $"weather_forecast_{latitude}_{longitude}";
            var cached = await _cacheService.GetCachedResponseAsync<ForecastResponse>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation($"Forecast cache hit for {latitude}, {longitude}");
                return cached;
            }

            var url = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric";
            var response = await _apiService.GetAsync<ForecastResponse>(url);

            if (response != null)
            {
                await _cacheService.CacheResponseAsync(cacheKey, response, CacheDurationMinutes);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching forecast for coordinates {latitude}, {longitude}: {ex.Message}");
            throw;
        }
    }

    public string GetWeatherIconUrl(string iconCode)
    {
        return $"https://openweathermap.org/img/wn/{iconCode}@4x.png";
    }

    public double ConvertTemperature(double celsius, string unit)
    {
        return unit.ToUpper() switch
        {
            "F" => (celsius * 9 / 5) + 32,
            "K" => celsius + 273.15,
            _ => celsius // Default to Celsius
        };
    }

    public double ConvertWindSpeed(double meterPerSecond, string unit)
    {
        return unit.ToLower() switch
        {
            "km/h" => meterPerSecond * 3.6,
            "mph" => meterPerSecond * 2.237,
            _ => meterPerSecond // Default to m/s
        };
    }
}