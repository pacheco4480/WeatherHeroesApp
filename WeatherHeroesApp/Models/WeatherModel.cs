namespace WeatherHeroesApp.Models
{
    public class WeatherModel
    {
        public string Name { get; set; } // City name
        public Coord Coord { get; set; } // Coordinates (latitude and longitude)
        public Sys Sys { get; set; } // System info (country)
        public Main Main { get; set; } // Temperature, pressure, humidity
        public List<WeatherCondition> Weather { get; set; } // Weather conditions
        public Wind Wind { get; set; } // Wind data
    }

    public class Coord
    {
        public double Lat { get; set; } // Latitude
        public double Lon { get; set; } // Longitude
    }

    public class Sys
    {
        public string Country { get; set; } // Country code (e.g., "PT" for Portugal)
        public long Sunrise { get; set; } // Sunrise time (Unix timestamp)
        public long Sunset { get; set; } // Sunset time (Unix timestamp)
    }

    public class Main
    {
        public double Temp { get; set; } // Current temperature
        public double TempMin { get; set; } // Minimum temperature
        public double TempMax { get; set; } // Maximum temperature
        public double FeelsLike { get; set; } // Feels-like temperature
        public int Humidity { get; set; } // Humidity percentage
        public int Pressure { get; set; } // Atmospheric pressure in hPa
    }

    public class WeatherCondition
    {
        public string Main { get; set; } // Weather type (e.g., "Rain", "Clouds")
        public string Description { get; set; } // Detailed description (e.g., "light rain")
        public string Icon { get; set; } // Icon code for the condition
    }

    public class Wind
    {
        public double Speed { get; set; } // Wind speed
        public int Deg { get; set; } // Wind direction in degrees
    }
}
