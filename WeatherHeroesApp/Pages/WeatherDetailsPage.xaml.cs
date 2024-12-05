using WeatherHeroesApp.Models;
using WeatherHeroesApp.Services;

namespace WeatherHeroesApp.Pages;

public partial class WeatherDetailsPage : ContentPage
{
    private readonly WeatherService _weatherService;
    private const string ReferenceCityKey = "ReferenceCity";

    public WeatherDetailsPage(string referenceCity)
    {
        InitializeComponent();
        _weatherService = new WeatherService();

        // Load weather details for the reference city
        LoadWeatherDetails(referenceCity);
    }

    /// <summary>
    /// Loads detailed weather information for a given city.
    /// </summary>
    /// <param name="cityName">The name of the city.</param>
    private async void LoadWeatherDetails(string cityName)
    {
        try
        {
            ErrorLabel.IsVisible = false;

            var weather = await _weatherService.GetCurrentWeatherAsync(cityName);

            if (weather != null)
            {
                CityNameLabel.Text = string.IsNullOrEmpty(weather.Name) ? "City: Data unavailable" : weather.Name;
                WeatherIcon.Source = string.IsNullOrEmpty(weather.Weather.FirstOrDefault()?.Icon)
                    ? "default_icon.png" // A default icon in case of missing data
                    : $"https://openweathermap.org/img/wn/{weather.Weather.FirstOrDefault()?.Icon}@2x.png";

                TemperatureLabel.Text = weather.Main.Temp == 0
                    ? "Temperature: Data unavailable"
                    : $"{weather.Main.Temp}°C";

                FeelsLikeLabel.Text = weather.Main.FeelsLike == 0
                    ? "Feels Like: Data unavailable"
                    : $"Feels Like: {weather.Main.FeelsLike}°C";

                HumidityLabel.Text = weather.Main.Humidity == 0
                    ? "Humidity: Data unavailable"
                    : $"Humidity: {weather.Main.Humidity}%";

                WindSpeedLabel.Text = weather.Wind.Speed == 0
                    ? "Wind Speed: Data unavailable"
                    : $"Wind Speed: {weather.Wind.Speed} m/s";

                PressureLabel.Text = weather.Main.Pressure == 0
                    ? "Pressure: Data unavailable"
                    : $"Pressure: {weather.Main.Pressure} hPa";

                SunriseLabel.Text = weather.Sys.Sunrise == 0
                    ? "Sunrise: Data unavailable"
                    : $"Sunrise: {UnixTimeToLocalTime(weather.Sys.Sunrise)}";

                SunsetLabel.Text = weather.Sys.Sunset == 0
                    ? "Sunset: Data unavailable"
                    : $"Sunset: {UnixTimeToLocalTime(weather.Sys.Sunset)}";

                WeatherConditionLabel.Text = string.IsNullOrEmpty(weather.Weather.FirstOrDefault()?.Description)
                    ? "Condition: Data unavailable"
                    : $"Condition: {weather.Weather.FirstOrDefault()?.Description}";
            }
            else
            {
                DisplayError("No weather details available.");
            }
        }
        catch (Exception ex)
        {
            DisplayError($"Error loading weather details: {ex.Message}");
        }
    }


    /// <summary>
    /// Converts Unix timestamp to local time.
    /// </summary>
    /// <param name="unixTime">Unix timestamp.</param>
    /// <returns>Formatted local time.</returns>
    private string UnixTimeToLocalTime(long unixTime)
    {
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
        return dateTime.ToString("HH:mm");
    }

    /// <summary>
    /// Displays an error message.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    private void DisplayError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}
