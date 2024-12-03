using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Storage;

namespace WeatherHeroesApp.Services;

public class AuthService
{
    private readonly FirebaseAuthProvider _authProvider;
    private readonly string _firebaseDatabaseUrl = "https://weatherheroesapp-default-rtdb.europe-west1.firebasedatabase.app/";

    public AuthService()
    {
        _authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAP2jjMW_NSTAni2Spesz1bEWA5dAlhHcE"));
    }

    /// <summary>
    /// Logs in the user.
    /// </summary>
    public async Task<string> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Email is required.";

        if (string.IsNullOrWhiteSpace(password))
            return "Password is required.";

        try
        {
            var auth = await _authProvider.SignInWithEmailAndPasswordAsync(email, password);

            SavePreferences(auth.FirebaseToken, auth.User.LocalId);
            return "success";
        }
        catch (FirebaseAuthException ex)
        {
            return HandleFirebaseAuthException(ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error during login: {ex.Message}");
            return "An error occurred during login. Please try again.";
        }
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    public async Task<string> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return "All fields are required.";
        }

        try
        {
            var auth = await _authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

            Console.WriteLine($"User successfully authenticated: {auth.User.LocalId}");

            var firebase = CreateFirebaseClient(auth.FirebaseToken);

            await firebase
                .Child("users")
                .Child(auth.User.LocalId)
                .PutAsync(new
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                });

            Console.WriteLine($"User data saved in Realtime Database: {firstName} {lastName}");
            SavePreferences(auth.FirebaseToken, auth.User.LocalId);
            return "success";
        }
        catch (FirebaseAuthException ex)
        {
            return HandleFirebaseAuthException(ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error during registration: {ex.Message}");
            return "An error occurred during registration. Please try again.";
        }
    }

    /// <summary>
    /// Logs out the user.
    /// </summary>
    public void Logout()
    {
        try
        {
            Preferences.Remove("AuthToken");
            Preferences.Remove("UserId");

            Console.WriteLine("User successfully logged out.");
            Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during logout: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the full name of the user.
    /// </summary>
    public async Task<string> GetUserNameAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return "Invalid user ID.";

        try
        {
            var firebase = CreateFirebaseClient(Preferences.Get("AuthToken", null));

            var user = await firebase
                .Child("users")
                .Child(userId)
                .OnceSingleAsync<dynamic>();

            return $"{user.FirstName} {user.LastName}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user name: {ex.Message}");
            return "User";
        }
    }

    /// <summary>
    /// Saves the user's token and ID in local preferences.
    /// </summary>
    private void SavePreferences(string authToken, string userId)
    {
        if (string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(userId))
        {
            Console.WriteLine("Invalid token or user ID. Preferences were not saved.");
            return;
        }

        Preferences.Set("AuthToken", authToken);
        Preferences.Set("UserId", userId);

        Console.WriteLine("Preferences saved successfully.");
    }

    /// <summary>
    /// Handles FirebaseAuth exceptions and returns user-friendly messages.
    /// </summary>
    private string HandleFirebaseAuthException(FirebaseAuthException ex)
    {
        Console.WriteLine($"FirebaseAuth error: {ex.Message}");

        if (ex.Message.Contains("EMAIL_NOT_FOUND"))
            return "User not found.";
        if (ex.Message.Contains("INVALID_PASSWORD"))
            return "Incorrect password.";
        if (ex.Message.Contains("EMAIL_EXISTS"))
            return "This email is already in use.";
        if (ex.Message.Contains("WEAK_PASSWORD"))
            return "The password is too weak.";

        return "An error occurred while processing your request. Please try again.";
    }

    /// <summary>
    /// Creates a Firebase client with the given token.
    /// </summary>
    private FirebaseClient CreateFirebaseClient(string authToken)
    {
        return new FirebaseClient(
            _firebaseDatabaseUrl,
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(authToken)
            });
    }
}
