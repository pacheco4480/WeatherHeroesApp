using System.Text.RegularExpressions;

namespace WeatherHeroesApp.Validators
{
    public class UserValidator : IValidator
    {
        public string ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "O email é obrigatório.";

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email) ? string.Empty : "Formato de email inválido.";
        }

        public string ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "A senha é obrigatória.";

            var errors = new List<string>();

            if (password.Length < 6)
                errors.Add("Pelo menos 6 caracteres.");
            if (!password.Any(char.IsUpper))
                errors.Add("Pelo menos uma letra maiúscula.");
            if (!password.Any(char.IsDigit))
                errors.Add("Pelo menos um número.");
            if (!password.Any(ch => "!@#$%^&*()".Contains(ch)))
                errors.Add("Pelo menos um símbolo especial (!@#$%^&*()).");

            return errors.Any() ? $"A senha deve conter: {string.Join(", ", errors)}" : string.Empty;
        }


        public string ValidatePasswordMatch(string password, string confirmPassword)
        {
            return password == confirmPassword ? string.Empty : "As senhas não coincidem.";
        }
    }
}
