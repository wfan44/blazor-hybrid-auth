namespace YourApp.Features.Weather.Models;

using System.Text.Json.Serialization;

public class WeatherResponse
{
    [JsonPropertyName("coord")]
    public Coordinates Coordinates { get; set; }

    [JsonPropertyName("weather")]
    public List<Weather> Weather { get; set; } = new();

    [JsonPropertyName("main")]
    public MainWeatherData Main { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("wind")]
    public Wind Wind { get; set; }

    [JsonPropertyName("clouds")]
    public Clouds Clouds { get; set; }

    [JsonPropertyName("dt")]
    public long DateTime { get; set; }

    [JsonPropertyName("sys")]
    public SystemData System { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("id")]
    public int CityId { get; set; }

    [JsonPropertyName("name")]
    public string CityName { get; set; }

    [JsonPropertyName("cod")]
    public int Code { get; set; }
}

public class Coordinates
{
    [JsonPropertyName("lon")]
    public double Longitude { get; set; }

    [JsonPropertyName("lat")]
    public double Latitude { get; set; }
}

public class Weather
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string Main { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }
}

public class MainWeatherData
{
    [JsonPropertyName("temp")]
    public double Temperature { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }

    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class Wind
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("deg")]
    public int Degree { get; set; }

    [JsonPropertyName("gust")]
    public double? Gust { get; set; }
}

public class Clouds
{
    [JsonPropertyName("all")]
    public int CloudPercentage { get; set; }
}

public class SystemData
{
    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }
}

public class ForecastResponse
{
    [JsonPropertyName("list")]
    public List<ForecastItem> ForecastItems { get; set; } = new();

    [JsonPropertyName("city")]
    public CityInfo City { get; set; }
}

public class ForecastItem
{
    [JsonPropertyName("dt")]
    public long DateTime { get; set; }

    [JsonPropertyName("main")]
    public MainWeatherData Main { get; set; }

    [JsonPropertyName("weather")]
    public List<Weather> Weather { get; set; } = new();

    [JsonPropertyName("clouds")]
    public Clouds Clouds { get; set; }

    [JsonPropertyName("wind")]
    public Wind Wind { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("pop")]
    public double ProbabilityOfPrecipitation { get; set; }

    [JsonPropertyName("rain")]
    public RainData Rain { get; set; }
}

public class RainData
{
    [JsonPropertyName("3h")]
    public double Amount { get; set; }
}

public class CityInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("coord")]
    public Coordinates Coordinates { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }
}

public class SavedLocation
{
    public int Id { get; set; }
    public string CityName { get; set; }
    public string Country { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class WeatherPreference
{
    public int Id { get; set; }
    public string TemperatureUnit { get; set; } = "C"; // C or F
    public string WindSpeedUnit { get; set; } = "m/s"; // m/s or km/h or mph
    public bool Use24HourFormat { get; set; } = true;
    public bool EnableNotifications { get; set; } = true;
    public int RefreshIntervalMinutes { get; set; } = 30;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}