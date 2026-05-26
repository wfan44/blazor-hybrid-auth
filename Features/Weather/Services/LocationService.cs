namespace YourApp.Features.Weather.Services;

using YourApp.Features.Weather.Interfaces;
using Microsoft.Maui.Devices.Sensors;

public class LocationService : ILocationService
{
    private readonly IGeolocation _geolocation;
    private readonly ILogger<LocationService> _logger;

    public bool IsLocationEnabled => _geolocation != null;

    public LocationService(
        IGeolocation geolocation,
        ILogger<LocationService> logger)
    {
        _geolocation = geolocation;
        _logger = logger;
    }

    public async Task<Location> GetCurrentLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            var location = await _geolocation.GetLocationAsync(request);

            if (location != null)
            {
                _logger.LogInformation($"Location obtained: {location.Latitude}, {location.Longitude}");
            }
            else
            {
                _logger.LogWarning("Unable to obtain location");
            }

            return location;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting location: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> RequestLocationPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            var granted = status == PermissionStatus.Granted;
            _logger.LogInformation($"Location permission: {(granted ? "Granted" : "Denied")}");
            return granted;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error requesting location permission: {ex.Message}");
            return false;
        }
    }
}