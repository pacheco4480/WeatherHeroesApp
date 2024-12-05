using System.Collections.Generic;

namespace WeatherHeroesApp.Models
{
    public class WeatherForecastResponse
    {
        public List<WeatherForecast> List { get; set; } // List of predictions for each period
        public CityInfo City { get; set; } // City information
    }

    public class CityInfo
    {
        public string Name { get; set; } // City name
        public string Country { get; set; } // Country code
    }
}
