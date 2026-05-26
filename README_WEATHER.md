# Weather Dashboard

A comprehensive weather dashboard for MAUI + Blazor Hybrid applications with offline-first support.

## Features

✅ **Current Weather Display**
- Real-time weather data from OpenWeatherMap API
- Temperature, humidity, wind speed, pressure
- Sunrise/sunset times
- Weather conditions with icons
- Min/max temperatures
- Visibility and cloud coverage

✅ **5-Day Forecast**
- Hourly forecast for next 5 days
- Precipitation probability
- Wind speed and direction
- Interactive forecast cards

✅ **Location Management**
- Save favorite locations
- Set default location
- Quick access buttons for saved locations
- Current location detection with permissions

✅ **User Preferences**
- Temperature unit selection (°C, °F, K)
- Wind speed units (m/s, km/h, mph)
- 12/24 hour time format
- Notification settings
- Refresh interval configuration

✅ **Offline Support**
- API response caching (30 minutes default)
- Works with cached data when offline
- Background sync when connection restored
- SQLite local database storage

✅ **Advanced Features**
- Wind direction indicator
- Unit conversion (temperature and wind speed)
- Search by city name
- Geolocation support
- Real-time network status

## Setup Instructions

### 1. Get OpenWeatherMap API Key

1. Go to [openweathermap.org](https://openweathermap.org/api)
2. Sign up for a free account
3. Generate an API key
4. Copy the key

### 2. Configure the API Key

Update `MauiProgram.cs` or `appsettings.json`:

```csharp
{ "Weather:ApiKey", "YOUR_OPENWEATHERMAP_API_KEY" }
```

### 3. Set Permissions (Platform-Specific)

**Android (AndroidManifest.xml)**
```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.INTERNET" />
```

**iOS (Info.plist)**
```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>This app needs your location to show weather</string>
```

**Windows**
No additional permissions required

### 4. Database Migration

The database is automatically initialized on app startup. Tables created:
- `SavedLocations` - User's saved weather locations
- `WeatherPreferences` - User preferences
- `CachedApiResponse` - Cached weather data
- `OfflineData` - Additional offline data

## Usage

### Navigation

```
/weather                  - Main weather dashboard
/weather/{CityName}       - Weather for specific city
/weather-forecast/{City}  - 5-day forecast
/weather-settings         - Preferences and locations
```

### Search Weather

1. Enter city name in search box
2. Press Enter or click "Search"
3. View current weather and conditions

### Use Current Location

1. Click "Use Current Location" button (if available)
2. Grant location permission
3. Weather will automatically load

### Save Locations

1. View weather for a city
2. Click "💾 Save Location"
3. Manage in Settings page

### View Forecast

1. From weather dashboard, click "📊 View Forecast"
2. See 5-day hourly forecast
3. Data updates with refresh

### Configure Preferences

1. Go to Settings (/weather-settings)
2. Change temperature unit
3. Select wind speed unit
4. Enable/disable notifications
5. Set refresh interval
6. Click "Save Preferences"

## Data Flow

```
┌─────────────────────┐
│  User Interface     │
│  (Blazor Pages)     │
└──────────┬──────────┘
           │
      ┌────▼────┐
      │ Services│
      ├─────────┤
      │ Weather │────────┐
      │Location │    ┌───▼────┐
      │ Prefs   │    │ API    │
      └────┬────┘    │ Cache  │
           │         └───┬────┘
      ┌────▼────┐        │
      │ SQLite  │◄───────┘
      │Database │
      └─────────┘
      (Offline)
```

## API Integration

### OpenWeatherMap Endpoints

**Current Weather**
```
https://api.openweathermap.org/data/2.5/weather?
  q={cityName}
  &appid={apiKey}
  &units=metric
```

**Weather by Coordinates**
```
https://api.openweathermap.org/data/2.5/weather?
  lat={latitude}
  &lon={longitude}
  &appid={apiKey}
  &units=metric
```

**5-Day Forecast**
```
https://api.openweathermap.org/data/2.5/forecast?
  q={cityName}
  &appid={apiKey}
  &units=metric
```

## Offline Behavior

- **GET requests**: Returns cached data if available
- **POST/PUT/DELETE**: Queued for sync when online
- **Cache expiration**: 30 minutes (configurable)
- **Sync strategy**: Automatic on connection restore

## Unit Conversions

### Temperature
- Celsius (C) - Default
- Fahrenheit (F) = (C × 9/5) + 32
- Kelvin (K) = C + 273.15

### Wind Speed
- Meters per second (m/s) - API default
- km/h = m/s × 3.6
- mph = m/s × 2.237

## Troubleshooting

### "API Key not configured"
- Check `MauiProgram.cs` or `appsettings.json`
- Verify key format (no extra spaces)

### "City not found"
- Use exact city name (e.g., "London" not "london city")
- Check spelling
- Try with country code (e.g., "London, UK")

### Location permission denied
- Grant permission in app settings
- Check device privacy settings
- Try manual city search instead

### Offline cache not working
- Ensure database initialized (check app data directory)
- Check refresh interval setting
- Clear cache and retry

## Performance

- API calls cached for 30 minutes
- Background sync every 30 seconds
- Database optimized with indexes
- Images loaded asynchronously

## Future Enhancements

- [ ] Weather alerts and warnings
- [ ] Air quality index (AQI)
- [ ] UV index
- [ ] Pollen count
- [ ] Air pollution data
- [ ] Weather comparison (multiple cities)
- [ ] Historical weather data
- [ ] Custom cache duration per location
- [ ] Weather widgets
- [ ] Push notifications

## Dependencies

- `System.Net.Http` - HTTP requests
- `Microsoft.EntityFrameworkCore` - Database
- `Microsoft.Maui.Controls` - UI
- `Microsoft.AspNetCore.Components` - Blazor
- `Microsoft.Maui.Devices.Sensors` - Location

## License

MIT
