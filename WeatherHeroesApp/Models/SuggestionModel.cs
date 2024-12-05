namespace WeatherHeroesApp.Models
{
    public class SuggestionModel
    {
        public string Name { get; set; } 
        public string Country { get; set; } 
        public double Latitude { get; set; } 
        public double Longitude { get; set; } 

        public string DisplayName { get; set; } 
    }
}
