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

        // Configure the event to navigate to the registration page
        var registerTapGesture = new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                try
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.GoToAsync("//RegisterPage");
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
    /// Event triggered when the "Login" button is clicked.
    /// </summary>
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        // Validations
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

        // Show loading indicator
        LoadingIndicator.IsVisible = true;

        try
        {
            string result = await _authService.LoginAsync(email, password);

            // Hide loading indicator
            LoadingIndicator.IsVisible = false;

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
                    appShell.UpdateMenuItems();
                }

                await DisplayAlert("Success", $"Welcome, {userName}!", "OK");

                // Navigate to the home page
                await Shell.Current.GoToAsync("//WeatherListPage");
            }
            else
            {
                DisplayErrorMessage(result);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error during login: {ex.Message}");
            DisplayErrorMessage("An error occurred during login. Please try again.");
        }
        finally
        {
            // Ensure the loading indicator is hidden
            LoadingIndicator.IsVisible = false;
        }
    }

    /// <summary>
    /// Displays an error message on the UI.
    /// </summary>
    private void DisplayErrorMessage(string message)
    {
        ErrorMessageLabel.Text = message;
        ErrorMessageLabel.IsVisible = true;
    }

    /// <summary>
    /// Validates the email in real-time as the user types.
    /// </summary>
    private void OnEmailTextChanged(object sender, TextChangedEventArgs e)
    {
        string email = e.NewTextValue;
        string emailError = _validator.ValidateEmail(email);

        if (!string.IsNullOrEmpty(emailError))
        {
            ErrorMessageLabel.Text = emailError;
            ErrorMessageLabel.IsVisible = true;
        }
        else
        {
            ErrorMessageLabel.IsVisible = false;
        }
    }

    /// <summary>
    /// Toggles password visibility.
    /// </summary>
    private void OnShowPasswordCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        bool isChecked = e.Value;

        PasswordEntry.IsPassword = !isChecked;
    }
}
