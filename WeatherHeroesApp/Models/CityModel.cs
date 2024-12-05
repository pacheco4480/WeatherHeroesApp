namespace WeatherHeroesApp.Models
{
    public class CityModel
    {
        public string Name { get; set; } // City name
        public double Latitude { get; set; } // Latitude coordinate
        public double Longitude { get; set; } // Longitude coordinate
        public string Country { get; set; } // Country code

        public string Temperature { get; set; } 
        public string WeatherIcon { get; set; } 
    }
}
