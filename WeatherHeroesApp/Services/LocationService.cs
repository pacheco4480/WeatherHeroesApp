using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherHeroesApp.Models;
using Microsoft.Maui.ApplicationModel;

namespace WeatherHeroesApp.Services
{
    public class LocationService
    {
        private readonly string _apiKey = "b566a00e71852d092b7ac5bdde37aa83";
        private readonly string _geoBaseUrl = "https://api.openweathermap.org/geo/1.0/reverse";

        /// <summary>
        /// Requests location permission if not already granted.
        /// </summary>
        private async Task<bool> RequestLocationPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }

        /// <summary>
        /// Gets the current location of the user using the device's geolocation.
        /// </summary>
        /// <returns>A CityModel object representing the user's current city.</returns>
        public async Task<CityModel> GetCurrentCityAsync()
        {
            try
            {
                // Request permissions
                if (!await RequestLocationPermissionAsync())
                {
                    throw new UnauthorizedAccessException("Location permission not granted.");
                }

                // Retrieve the current geographic location
                var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (location == null)
                {
                    throw new Exception("Unable to retrieve the current location.");
                }

                // Fetch city name using reverse geocoding
                var cityName = await GetCityNameAsync(location.Latitude, location.Longitude);

                return new CityModel
                {
                    Name = cityName.Name,
                    Latitude = cityName.Latitude,
                    Longitude = cityName.Longitude,
                    Country = cityName.Country
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting current city: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the city name based on latitude and longitude using the OpenWeatherMap reverse geocoding API.
        /// </summary>
        /// <param name="latitude">The latitude of the location.</param>
        /// <param name="longitude">The longitude of the location.</param>
        /// <returns>A SuggestionModel object containing city and country information.</returns>
        public async Task<SuggestionModel> GetCityNameAsync(double latitude, double longitude)
        {
            try
            {
                string url = $"{_geoBaseUrl}?lat={latitude}&lon={longitude}&limit=1&appid={_apiKey}";

                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);

                var locationData = JsonSerializer.Deserialize<List<SuggestionModel>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (locationData != null && locationData.Any())
                {
                    var cityData = locationData.First();
                    cityData.DisplayName = $"{cityData.Name}, {cityData.Country}";
                    return cityData;
                }
                else
                {
                    throw new Exception("City not found for the given coordinates.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting city name: {ex.Message}");
                throw;
            }
        }
    }
}
