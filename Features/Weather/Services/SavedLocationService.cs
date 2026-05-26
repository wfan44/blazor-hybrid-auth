namespace YourApp.Features.Weather.Services;

using YourApp.Features.Weather.Models;
using YourApp.Features.Weather.Interfaces;
using YourApp.Features.Offline.Database;
using Microsoft.EntityFrameworkCore;

public class SavedLocationService : ISavedLocationService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<SavedLocationService> _logger;

    public SavedLocationService(
        AppDbContext dbContext,
        ILogger<SavedLocationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<SavedLocation>> GetAllLocationsAsync()
    {
        try
        {
            return await _dbContext.SavedLocations.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving saved locations: {ex.Message}");
            return new List<SavedLocation>();
        }
    }

    public async Task<SavedLocation> GetDefaultLocationAsync()
    {
        try
        {
            return await _dbContext.SavedLocations
                .FirstOrDefaultAsync(l => l.IsDefault);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving default location: {ex.Message}");
            return null;
        }
    }

    public async Task SaveLocationAsync(SavedLocation location)
    {
        try
        {
            if (location.Id == 0)
            {
                // If this is being set as default, unset others
                if (location.IsDefault)
                {
                    var otherDefaults = await _dbContext.SavedLocations
                        .Where(l => l.IsDefault)
                        .ToListAsync();
                    foreach (var loc in otherDefaults)
                    {
                        loc.IsDefault = false;
                    }
                }

                await _dbContext.SavedLocations.AddAsync(location);
            }
            else
            {
                if (location.IsDefault)
                {
                    var otherDefaults = await _dbContext.SavedLocations
                        .Where(l => l.IsDefault && l.Id != location.Id)
                        .ToListAsync();
                    foreach (var loc in otherDefaults)
                    {
                        loc.IsDefault = false;
                    }
                }

                _dbContext.SavedLocations.Update(location);
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Location saved: {location.CityName}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving location: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteLocationAsync(int id)
    {
        try
        {
            var location = await _dbContext.SavedLocations.FindAsync(id);
            if (location != null)
            {
                _dbContext.SavedLocations.Remove(location);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Location deleted: {id}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting location: {ex.Message}");
            throw;
        }
    }

    public async Task SetDefaultLocationAsync(int id)
    {
        try
        {
            var allLocations = await _dbContext.SavedLocations.ToListAsync();
            foreach (var location in allLocations)
            {
                location.IsDefault = location.Id == id;
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Default location set: {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error setting default location: {ex.Message}");
            throw;
        }
    }
}