namespace WeatherHeroesApp.Validators
{
    public interface IValidator
    {
        string ValidateEmail(string email);
        string ValidatePassword(string password);
        string ValidatePasswordMatch(string password, string confirmPassword);
    }
}
