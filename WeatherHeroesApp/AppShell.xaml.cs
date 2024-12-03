namespace WeatherHeroesApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register navigation routes
        RegisterRoutes();

        // Update menu items based on authentication status
        UpdateMenuItems();
    }

    private void RegisterRoutes()
    {
        // Register necessary routes
        Routing.RegisterRoute("RegisterPage", typeof(Pages.RegisterPage));
        Routing.RegisterRoute("LoginPage", typeof(Pages.LoginPage));
        Routing.RegisterRoute("WeatherListPage", typeof(Pages.WeatherListPage));

        Console.WriteLine("Routes successfully registered.");
    }

    public void UpdateMenuItems()
    {
        try
        {
            // Check authentication status
            bool isLoggedIn = !string.IsNullOrEmpty(Preferences.Get("AuthToken", null));

            // Dynamically update menu items
            if (isLoggedIn)
            {
                RemoveMenuItem("LoginPage");
                RemoveMenuItem("RegisterPage");
                AddMenuItem("LogoutPage", new Pages.LoginPage(), "Logout");
            }
            else
            {
                RemoveMenuItem("LogoutPage");
                AddMenuItem("LoginPage", new Pages.LoginPage(), "Login");
                AddMenuItem("RegisterPage", new Pages.RegisterPage(), "Register");

                // Set the default item to LoginPage
                CurrentItem = Items.FirstOrDefault(i => i.Route == "LoginPage");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating menu items: {ex.Message}");
        }
    }

    private void AddMenuItem(string route, Page page, string title)
    {
        if (!Items.Any(i => i.Route == route))
        {
            Items.Add(new FlyoutItem
            {
                Title = title,
                Route = route,
                Items =
                {
                    new ShellContent
                    {
                        Content = page
                    }
                }
            });
        }
    }

    private void RemoveMenuItem(string route)
    {
        var menuItem = Items.FirstOrDefault(i => i.Route == route);
        if (menuItem != null)
        {
            Items.Remove(menuItem);
        }
    }
}
