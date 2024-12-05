namespace WeatherHeroesApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register navigation routes
        RegisterRoutes();

        // Ensure the user is redirected appropriately
        CheckAuthentication();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute("WeatherListPage", typeof(Pages.WeatherListPage));
        Routing.RegisterRoute("ManageCitiesPage", typeof(Pages.ManageCitiesPage));
        Routing.RegisterRoute("ForecastPage", typeof(Pages.ForecastPage));
        Routing.RegisterRoute("CreditsPage", typeof(Pages.CreditsPage));
        Routing.RegisterRoute("LoginPage", typeof(Pages.LoginPage));
        Routing.RegisterRoute("RegisterPage", typeof(Pages.RegisterPage));

    }

    public void CheckAuthentication()
    {
        // Redirect based on authentication status
        bool isLoggedIn = !string.IsNullOrEmpty(Preferences.Get("AuthToken", null));

        if (!isLoggedIn)
        {
            CurrentItem = Items.FirstOrDefault(i => i.Route == "LoginPage");
        }
        else
        {
            CurrentItem = Items.FirstOrDefault(i => i.Route == "WeatherListPage");
        }
    }
}
