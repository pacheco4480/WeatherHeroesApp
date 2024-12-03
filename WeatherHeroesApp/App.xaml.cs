using Firebase.Auth;

namespace WeatherHeroesApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }



    private async Task<bool> IsUserLoggedInAsync()
    {
        var authToken = Preferences.Get("AuthToken", null);

        if (string.IsNullOrEmpty(authToken))
        {
            Console.WriteLine("Authentication token not found. User is not logged in.");
            return false;
        }

        try
        {
            // Validate the token using FirebaseAuthProvider
            var auth = await new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAP2jjMW_NSTAni2Spesz1bEWA5dAlhHcE"))
                .GetUserAsync(authToken);

            return auth != null; // Return true if the token is valid
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating token: {ex.Message}");
            return false;
        }
    }
}
