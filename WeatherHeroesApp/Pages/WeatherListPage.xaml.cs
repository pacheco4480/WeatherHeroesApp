using WeatherHeroesApp.Models;
using WeatherHeroesApp.Services;
using WeatherHeroesApp.Helpers;
using SkiaSharp.Extended.UI.Controls;


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
                weather = await _weatherService.GetCurrentWeatherAsync(referenceCity);
            }
            else
            {
                var city = await _locationService.GetCurrentCityAsync();
                weather = await _weatherService.GetCurrentWeatherAsync(city.Name);
            }

            if (weather != null)
            {
                CityNameLabel.Text = weather.Name;
                WeatherIcon.Source = $"https://openweathermap.org/img/wn/{weather.Weather.FirstOrDefault()?.Icon}@2x.png";
                TemperatureLabel.Text = $"{weather.Main.Temp}°C";
                WeatherDescriptionLabel.Text = AnimationHelper.GetMessage(weather.Weather.FirstOrDefault()?.Main);

                // Atualiza a animação
                WeatherAnimationView.Source = new SKFileLottieImageSource
                {
                    File = AnimationHelper.GetAnimation(weather.Weather.FirstOrDefault()?.Main)
                };

                HeroIcon.Source = AnimationHelper.GetIcon(weather.Weather.FirstOrDefault()?.Main);
                HumidityLabel.Text = $"Humidity: {weather.Main.Humidity}%";
                WindSpeedLabel.Text = $"Wind Speed: {weather.Wind.Speed} m/s";

                // Atualiza a cor de fundo dinamicamente com base na condição climática
                this.BackgroundColor = Color.FromHex(AnimationHelper.GetBackgroundColor(weather.Weather.FirstOrDefault()?.Main));
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
