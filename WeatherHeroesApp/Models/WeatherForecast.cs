namespace WeatherHeroesApp.Models;

public class WeatherForecast
{
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public double Temperature { get; set; }
    public string Icon { get; set; }

    public string IconUrl => $"https://openweathermap.org/img/wn/{Icon}@2x.png";
    public string FormattedDate => Date.ToString("dd/MM/yyyy");
}
