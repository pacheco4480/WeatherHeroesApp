using System.Collections.ObjectModel;
using WeatherHeroesApp.Models;
using WeatherHeroesApp.Services;

namespace WeatherHeroesApp.Pages;

public partial class ForecastPage : ContentPage
{
    private readonly WeatherService _weatherService;
    private readonly LocationService _locationService;
    public ObservableCollection<WeatherForecast> ForecastData { get; set; }
    private const string ReferenceCityKey = "ReferenceCity";

    public ForecastPage()
    {
        InitializeComponent();
        _weatherService = new WeatherService();
        _locationService = new LocationService();
        ForecastData = new ObservableCollection<WeatherForecast>();
        ForecastList.ItemsSource = ForecastData;

        LoadForecastData();
    }

    private async void LoadForecastData()
    {
        try
        {
            ErrorLabel.IsVisible = false;
            string referenceCity = Preferences.Get(ReferenceCityKey, null);
            string cityName;

            if (!string.IsNullOrEmpty(referenceCity))
            {
                // Load data for the reference city
                cityName = referenceCity;
            }
            else
            {
                // Use current location
                var currentCity = await _locationService.GetCurrentCityAsync();
                if (currentCity == null)
                {
                    DisplayError("Unable to retrieve the current location.");
                    return;
                }

                cityName = currentCity.Name;
            }

            await LoadForecast(cityName);
        }
        catch (Exception ex)
        {
            DisplayError($"Error loading forecast data: {ex.Message}");
        }
    }

    private async Task LoadForecast(string cityName)
    {
        try
        {
            var forecast = await _weatherService.Get5DayForecastAsync(cityName);

            if (forecast != null && forecast.Any())
            {
                ForecastData.Clear();
                foreach (var item in forecast)
                {
                    ForecastData.Add(item);
                }

                CityNameLabel.Text = $"Forecast for {cityName}";
            }
            else
            {
                DisplayError("No forecast data available.");
            }
        }
        catch (Exception ex)
        {
            DisplayError($"Error loading forecast: {ex.Message}");
        }
    }

    private void DisplayError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Ensure data is always reloaded when it appears
        LoadForecastData();
    }
}
