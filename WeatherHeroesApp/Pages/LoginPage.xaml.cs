using WeatherHeroesApp.Services;
using WeatherHeroesApp.Validators;

namespace WeatherHeroesApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly IValidator _validator;

    public LoginPage()
    {
        InitializeComponent();
        _authService = new AuthService();
        _validator = new UserValidator();

        ConfigureNavigationEvents();
    }

    /// <summary>
    /// Configures navigation to the registration page.
    /// </summary>
    private void ConfigureNavigationEvents()
    {
        var registerTapGesture = new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                try
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.GoToAsync("/RegisterPage");
                        Console.WriteLine("Navigated to RegisterPage successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Shell.Current is null. Navigation failed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error navigating to RegisterPage: {ex.Message}");
                }
            })
        };
        RegisterLink.GestureRecognizers.Add(registerTapGesture);
    }

    /// <summary>
    /// Handles the login process.
    /// </summary>
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        // Validate email
        string emailError = _validator.ValidateEmail(email);
        if (!string.IsNullOrEmpty(emailError))
        {
            DisplayErrorMessage(emailError);
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            DisplayErrorMessage("Please enter your password.");
            return;
        }

        try
        {
            // Show LoadingPage
            await Navigation.PushModalAsync(new LoadingPage());

            string result = await _authService.LoginAsync(email, password);

            if (result == "success")
            {
                string userId = Preferences.Get("UserId", null);
                if (string.IsNullOrEmpty(userId))
                {
                    DisplayErrorMessage("Error retrieving user ID. Please try again.");
                    return;
                }

                string userName = await _authService.GetUserNameAsync(userId);

                // Update menu after login
                if (Shell.Current is AppShell appShell)
                {
                    appShell.CheckAuthentication();
                }

                await DisplayAlert("Success", $"Welcome, {userName}!", "OK");

                // Navigate to the home page
                await Navigation.PopModalAsync(); // Close the LoadingPage
                await Shell.Current.GoToAsync("//WeatherListPage");
            }
            else
            {
                await Navigation.PopModalAsync(); // Close the LoadingPage
                DisplayErrorMessage(result);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error during login: {ex.Message}");
            await Navigation.PopModalAsync(); // Close the LoadingPage
            DisplayErrorMessage("An error occurred during login. Please try again.");
        }
    }


    /// <summary>
    /// Displays an error message.
    /// </summary>
    private void DisplayErrorMessage(string message)
    {
        ErrorMessageLabel.Text = message;
        ErrorMessageLabel.IsVisible = true;
    }

    /// <summary>
    /// Validates email in real-time as the user types.
    /// </summary>
    private void OnEmailTextChanged(object sender, TextChangedEventArgs e)
    {
        string email = e.NewTextValue;
        string emailError = _validator.ValidateEmail(email);

        ErrorMessageLabel.Text = emailError;
        ErrorMessageLabel.IsVisible = !string.IsNullOrEmpty(emailError);
    }

    /// <summary>
    /// Toggles password visibility.
    /// </summary>
    private void OnShowPasswordCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        PasswordEntry.IsPassword = !e.Value;
    }
}
