using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherHeroesApp.Models;

namespace WeatherHeroesApp.Services
{
    public class WeatherService
    {
        private readonly string _apiKey = "b566a00e71852d092b7ac5bdde37aa83";
        private readonly string _baseUrl = "https://api.openweathermap.org/data/2.5/";

        /// <summary>
        /// Gets the current weather for a given city.
        /// </summary>
        /// <param name="cityName">The name of the city.</param>
        /// <returns>A WeatherModel object containing the current weather data.</returns>
        public async Task<WeatherModel> GetCurrentWeatherAsync(string cityName)
        {
            string url = $"{_baseUrl}weather?q={cityName}&appid={_apiKey}&units=metric";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);

            return JsonSerializer.Deserialize<WeatherModel>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private const string ApiKey = "b566a00e71852d092b7ac5bdde37aa83"; 
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/forecast";

        public async Task<List<WeatherForecast>> Get5DayForecastAsync(string city)
        {
            using var client = new HttpClient();
            var url = $"{BaseUrl}?q={city}&appid={ApiKey}&units=metric";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to fetch weather data.");

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json);

            return data.RootElement.GetProperty("list")
                .EnumerateArray()
                .Where((_, index) => index % 8 == 0) // 8 intervals of 3 hours = 1 forecast per day
                .Select(item => new WeatherForecast
                {
                    Date = DateTime.Parse(item.GetProperty("dt_txt").GetString()!),
                    Description = item.GetProperty("weather")[0].GetProperty("description").GetString()!,
                    Temperature = item.GetProperty("main").GetProperty("temp").GetDouble(),
                    Icon = item.GetProperty("weather")[0].GetProperty("icon").GetString()!
                })
                .ToList();
        }
    }
}
