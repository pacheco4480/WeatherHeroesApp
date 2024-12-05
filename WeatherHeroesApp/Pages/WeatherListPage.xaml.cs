using WeatherHeroesApp.Models;
using WeatherHeroesApp.Services;

namespace WeatherHeroesApp.Pages;

public partial class WeatherListPage : ContentPage
{
    private readonly WeatherService _weatherService;
    private readonly LocationService _locationService;
    private const string ReferenceCityKey = "ReferenceCity";

    public WeatherListPage()
    {
        InitializeComponent();
        _weatherService = new WeatherService();
        _locationService = new LocationService();

        LoadWeatherData();
    }

    /// <summary>
    /// Loads weather data based on the reference city or current location.
    /// </summary>
    private async void LoadWeatherData()
    {
        try
        {
            ErrorLabel.IsVisible = false;

            string referenceCity = Preferences.Get(ReferenceCityKey, null);

            WeatherModel weather;
            if (!string.IsNullOrEmpty(referenceCity))
            {
                // Load weather for the reference city
                weather = await _weatherService.GetCurrentWeatherAsync(referenceCity);
            }
            else
            {
                // Load weather based on current location
                var city = await _locationService.GetCurrentCityAsync();
                weather = await _weatherService.GetCurrentWeatherAsync(city.Name);
            }

            if (weather != null)
            {
                CityNameLabel.Text = weather.Name;
                WeatherIcon.Source = $"https://openweathermap.org/img/wn/{weather.Weather.FirstOrDefault()?.Icon}@2x.png";
                TemperatureLabel.Text = $"{weather.Main.Temp}°C";

                // Replace Feels Like with Weather Description
                WeatherDescriptionLabel.Text = $"Weather: {weather.Weather.FirstOrDefault()?.Description}";

                // Display other weather information
                HumidityLabel.Text = $"Humidity: {weather.Main.Humidity}%";
                WindSpeedLabel.Text = $"Wind Speed: {weather.Wind.Speed} m/s";

                // Handle missing or zero values for wind speed
                if (weather.Wind.Speed == 0)
                    WindSpeedLabel.Text = "Wind Speed: Data unavailable";
            }
            else
            {
                DisplayError("No weather data available.");
            }
        }
        catch (Exception ex)
        {
            DisplayError($"Error loading weather data: {ex.Message}");
        }
    }

    /// <summary>
    /// Navigates to the WeatherDetailsPage to show more detailed weather information.
    /// </summary>
    /// <summary>
    /// Navigates to the WeatherDetailsPage to show more detailed weather information.
    /// </summary>
    private async void OnSeeMoreDetailsClicked(object sender, EventArgs e)
    {
        try
        {
            string referenceCity = Preferences.Get(ReferenceCityKey, null);
            string cityName;

            if (!string.IsNullOrEmpty(referenceCity))
            {
                // Use the reference city if available
                cityName = referenceCity;
            }
            else
            {
                // Get the city from the LocationService
                var currentCity = await _locationService.GetCurrentCityAsync();
                if (currentCity == null)
                {
                    await DisplayAlert("Error", "Unable to retrieve the current location.", "OK");
                    return;
                }
                cityName = currentCity.Name;
            }

            // Navigate to the WeatherDetailsPage with the city name
            await Navigation.PushAsync(new WeatherDetailsPage(cityName));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load details: {ex.Message}", "OK");
        }
    }



    /// <summary>
    /// Displays an error message.
    /// </summary>
    private void DisplayError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadWeatherData(); // Reload data when the page appears
    }


}
