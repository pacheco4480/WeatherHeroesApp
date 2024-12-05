using WeatherHeroesApp.Models;
using WeatherHeroesApp.Services;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace WeatherHeroesApp.Pages;

public partial class ManageCitiesPage : ContentPage
{
    private const string CitiesKey = "SavedCities";
    private const string ReferenceCityKey = "ReferenceCity";
    private readonly WeatherService _weatherService;

    public ObservableCollection<SuggestionModel> Suggestions { get; set; } = new();
    public ObservableCollection<CityModel> Cities { get; set; }

    public ManageCitiesPage()
    {
        InitializeComponent();

        _weatherService = new WeatherService();
        Cities = new ObservableCollection<CityModel>(LoadCities());
        SuggestionsCollectionView.ItemsSource = Suggestions;
        CitiesCollectionView.ItemsSource = Cities;

        LoadWeatherForCities(); // Update cities' weather data
        UpdateEmptyState();
    }

    private void SaveCities()
    {
        var serializedCities = JsonConvert.SerializeObject(Cities);
        Preferences.Set(CitiesKey, serializedCities);
    }

    private List<CityModel> LoadCities()
    {
        var serializedCities = Preferences.Get(CitiesKey, string.Empty);
        return string.IsNullOrEmpty(serializedCities)
            ? new List<CityModel>()
            : JsonConvert.DeserializeObject<List<CityModel>>(serializedCities);
    }

    private async void LoadWeatherForCities()
    {
        foreach (var city in Cities)
        {
            try
            {
                var weather = await _weatherService.GetCurrentWeatherAsync(city.Name);
                city.Temperature = $"{weather.Main.Temp}°C";
                city.WeatherIcon = $"https://openweathermap.org/img/wn/{weather.Weather.FirstOrDefault()?.Icon}@2x.png";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating weather for {city.Name}: {ex.Message}");
            }
        }
        SaveCities(); // Save updated cities
        RefreshCitiesCollectionView(); // Update visually
    }

    private void RefreshCitiesCollectionView()
    {
        CitiesCollectionView.ItemsSource = null; // Force visual update
        CitiesCollectionView.ItemsSource = Cities;
    }

    private async void OnSetAsReferenceClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.CommandParameter is CityModel city)
        {
            Preferences.Set(ReferenceCityKey, city.Name);
            await DisplayAlert("Success", $"{city.Name} is now the reference city.", "OK");
        }
    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CitySearchBar.Text)) return;

        try
        {
            var weather = await _weatherService.GetCurrentWeatherAsync(CitySearchBar.Text.Trim());

            if (weather != null)
            {
                var city = new CityModel
                {
                    Name = weather.Name,
                    Latitude = weather.Coord.Lat,
                    Longitude = weather.Coord.Lon,
                    Country = weather.Sys.Country,
                    Temperature = $"{weather.Main.Temp}°C",
                    WeatherIcon = $"https://openweathermap.org/img/wn/{weather.Weather.FirstOrDefault()?.Icon}@2x.png"
                };

                // Add the new city to the list if it doesn't already exist
                if (!Cities.Any(c => c.Name.Equals(city.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    Cities.Add(city);
                    SaveCities();
                    UpdateEmptyState();

                    // Updates weather data to ensure immediate display
                    await UpdateWeatherForCity(city);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching weather data: {ex.Message}");
            await DisplayAlert("Error", "City not found or failed to fetch data.", "OK");
        }

        CitySearchBar.Text = string.Empty;
        RefreshCitiesCollectionView(); // Update the interface
    }


    private async Task UpdateWeatherForCity(CityModel city)
{
    try
    {
        var weather = await _weatherService.GetCurrentWeatherAsync(city.Name);
        city.Temperature = $"{weather.Main.Temp}°C";
        city.WeatherIcon = $"https://openweathermap.org/img/wn/{weather.Weather.FirstOrDefault()?.Icon}@2x.png";

        SaveCities();
        RefreshCitiesCollectionView();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating weather for {city.Name}: {ex.Message}");
    }
}


    private void OnRemoveCity(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.CommandParameter is CityModel city)
        {
            Cities.Remove(city);
            SaveCities();
            UpdateEmptyState();
            RefreshCitiesCollectionView();

            // Clear reference if removed
            string referenceCity = Preferences.Get(ReferenceCityKey, null);
            if (referenceCity == city.Name)
            {
                Preferences.Remove(ReferenceCityKey);
            }
        }
    }

    private void UpdateEmptyState()
    {
        EmptyStateLabel.IsVisible = !Cities.Any();
    }

    private async void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            Suggestions.Clear();
            SuggestionsCollectionView.IsVisible = false;
            return;
        }

        try
        {
            var searchText = e.NewTextValue.Trim();
            var cities = await GetCitySuggestionsAsync(searchText);

            if (cities != null && cities.Any())
            {
                Suggestions.Clear();
                foreach (var city in cities)
                {
                    Suggestions.Add(city);
                }
                SuggestionsCollectionView.IsVisible = true;
            }
            else
            {
                Suggestions.Clear();
                SuggestionsCollectionView.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching suggestions: {ex.Message}");
            Suggestions.Clear();
            SuggestionsCollectionView.IsVisible = false;
        }
    }

    private async Task<List<SuggestionModel>> GetCitySuggestionsAsync(string query)
    {
        var url = $"https://api.openweathermap.org/geo/1.0/direct?q={query}&limit=5&appid=b566a00e71852d092b7ac5bdde37aa83";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(url);
        var cities = JsonConvert.DeserializeObject<List<SuggestionModel>>(response);

        return cities?.Select(city => new SuggestionModel
        {
            Name = city.Name,
            Country = city.Country,
            DisplayName = $"{city.Name}, {city.Country}",
            Latitude = city.Latitude,
            Longitude = city.Longitude
        }).ToList() ?? new List<SuggestionModel>();
    }

    private async void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SuggestionModel selectedCity)
        {
            CitySearchBar.Text = selectedCity.Name;
            Suggestions.Clear();
            SuggestionsCollectionView.IsVisible = false;

            var city = new CityModel
            {
                Name = selectedCity.Name,
                Latitude = selectedCity.Latitude,
                Longitude = selectedCity.Longitude,
                Country = selectedCity.Country
            };

            // Add the city if it is not already in the list
            if (!Cities.Any(c => c.Name.Equals(city.Name, StringComparison.OrdinalIgnoreCase)))
            {
                Cities.Add(city);
                SaveCities();
                UpdateEmptyState();

                // Updates weather data to ensure immediate display
                await UpdateWeatherForCity(city);
            }
        }

        RefreshCitiesCollectionView(); 
    }

}
