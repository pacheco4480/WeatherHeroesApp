using WeatherHeroesApp.Services;
using WeatherHeroesApp.Validators;

namespace WeatherHeroesApp.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly IValidator _validator;

    public RegisterPage()
    {
        InitializeComponent();
        _authService = new AuthService();
        _validator = new UserValidator();
    }

    /// <summary>
    /// Validates email in real-time.
    /// </summary>
    private void OnEmailTextChanged(object sender, TextChangedEventArgs e)
    {
        string email = e.NewTextValue;
        string emailError = _validator.ValidateEmail(email);

        if (!string.IsNullOrEmpty(emailError))
        {
            DisplayErrorMessage(emailError);
        }
        else
        {
            ErrorMessageLabel.IsVisible = false;
        }
    }

    /// <summary>
    /// Visually validates the password as the user types.
    /// </summary>
    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        string password = e.NewTextValue;

        // Update validation rule labels
        LengthRuleLabel.TextColor = password.Length >= 6 ? Colors.Green : Colors.Gray;
        UppercaseRuleLabel.TextColor = password.Any(char.IsUpper) ? Colors.Green : Colors.Gray;
        DigitRuleLabel.TextColor = password.Any(char.IsDigit) ? Colors.Green : Colors.Gray;
        SymbolRuleLabel.TextColor = password.Any(ch => "!@#$%^&*()".Contains(ch)) ? Colors.Green : Colors.Gray;
    }

    /// <summary>
    /// Toggles password visibility.
    /// </summary>
    private void OnShowPasswordCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        bool isChecked = e.Value;

        // Toggle password fields visibility
        PasswordEntry.IsPassword = !isChecked;
        ConfirmPasswordEntry.IsPassword = !isChecked;
    }

    /// <summary>
    /// Event triggered when the "Register" button is clicked.
    /// </summary>
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        // Clear error messages
        ErrorMessageLabel.IsVisible = false;

        string firstName = FirstNameEntry.Text?.Trim();
        string lastName = LastNameEntry.Text?.Trim();
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        // Validations
        if (string.IsNullOrWhiteSpace(firstName))
        {
            DisplayErrorMessage("First name is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            DisplayErrorMessage("Last name is required.");
            return;
        }

        string emailError = _validator.ValidateEmail(email);
        string passwordError = _validator.ValidatePassword(password);
        string matchError = _validator.ValidatePasswordMatch(password, confirmPassword);

        if (!string.IsNullOrEmpty(emailError))
        {
            DisplayErrorMessage(emailError);
            return;
        }

        if (!string.IsNullOrEmpty(passwordError))
        {
            DisplayErrorMessage(passwordError);
            return;
        }

        if (!string.IsNullOrEmpty(matchError))
        {
            DisplayErrorMessage(matchError);
            return;
        }

        // Show loading indicator
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            string result = await _authService.RegisterAsync(email, password, firstName, lastName);

            if (result == "success")
            {
                await DisplayAlert("Success", "Registration completed successfully! Please login.", "OK");
                await Shell.Current.GoToAsync("/LoginPage");
            }
            else
            {
                DisplayErrorMessage(result);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error during registration: {ex.Message}");
            DisplayErrorMessage("An error occurred during registration. Please try again.");
        }
        finally
        {
            // Hide loading indicator
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
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
}
