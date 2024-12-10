using WeatherHeroesApp.Models;
using WeatherHeroesApp.Services;
using WeatherHeroesApp.Helpers;
using SkiaSharp.Extended.UI.Controls;

namespace WeatherHeroesApp.Pages;

public partial class WeatherDetailsPage : ContentPage
{
    private readonly WeatherService _weatherService;

    public WeatherDetailsPage(string cityName)
    {
        InitializeComponent();
        _weatherService = new WeatherService();

        // Load weather details for the specified city
        LoadWeatherDetails(cityName);
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
                // City Name
                CityNameLabel.Text = string.IsNullOrEmpty(weather.Name)
                    ? "City: Data unavailable"
                    : weather.Name;

                // Weather Icon
                WeatherIcon.Source = string.IsNullOrEmpty(weather.Weather.FirstOrDefault()?.Icon)
                    ? "default_icon.png"
                    : $"https://openweathermap.org/img/wn/{weather.Weather.FirstOrDefault()?.Icon}@2x.png";

                // Temperature
                TemperatureLabel.Text = weather.Main.Temp == 0
                    ? "Temperature: Data unavailable"
                    : $"{weather.Main.Temp}°C";

                // Weather Description
                WeatherDescriptionLabel.Text = !string.IsNullOrEmpty(weather.Weather.FirstOrDefault()?.Main)
                    ? AnimationHelper.GetMessage(weather.Weather.FirstOrDefault()?.Main)
                    : "Condition: Data unavailable";

                // Description
                var description = weather.Weather.FirstOrDefault()?.Description;
                DescriptionLabel.Text = string.IsNullOrEmpty(description)
                    ? "Description: Data unavailable"
                    : $"Description: {char.ToUpper(description[0])}{description.Substring(1)}";

                // Feels Like
                FeelsLikeLabel.Text = weather.Main.FeelsLike == 0
                    ? "Feels Like: Data unavailable"
                    : $"Feels Like: {weather.Main.FeelsLike}°C";

                // Humidity
                HumidityLabel.Text = weather.Main.Humidity == 0
                    ? "Humidity: Data unavailable"
                    : $"Humidity: {weather.Main.Humidity}%";

                // Wind Speed
                WindSpeedLabel.Text = weather.Wind.Speed == 0
                    ? "Wind Speed: Data unavailable"
                    : $"Wind Speed: {weather.Wind.Speed} m/s";

                // Pressure
                PressureLabel.Text = weather.Main.Pressure == 0
                    ? "Pressure: Data unavailable"
                    : $"Pressure: {weather.Main.Pressure} hPa";

                // Sunrise
                SunriseLabel.Text = weather.Sys.Sunrise == 0
                    ? "Sunrise: Data unavailable"
                    : $"Sunrise: {UnixTimeToLocalTime(weather.Sys.Sunrise)}";

                // Sunset
                SunsetLabel.Text = weather.Sys.Sunset == 0
                    ? "Sunset: Data unavailable"
                    : $"Sunset: {UnixTimeToLocalTime(weather.Sys.Sunset)}";

                // Update animation and background color
                WeatherAnimationView.Source = new SKFileLottieImageSource
                {
                    File = AnimationHelper.GetAnimation(weather.Weather.FirstOrDefault()?.Main)
                };

                HeroIcon.Source = AnimationHelper.GetIcon(weather.Weather.FirstOrDefault()?.Main);
                MainGrid.BackgroundColor = Color.FromHex(AnimationHelper.GetBackgroundColor(weather.Weather.FirstOrDefault()?.Main));
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
