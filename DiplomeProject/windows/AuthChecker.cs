using System.Linq;

namespace DiplomeProject
{
    public class AuthChecker
    {
        // Проверка логина (длина от 3 до 14 символов, без спецсимволов)
        public static bool ValidateLogin(string login)
        {
            if (string.IsNullOrEmpty(login)) return false;
            if (login.Length < 3 || login.Length > 14) return false;
            if (!login.All(char.IsLetterOrDigit)) return false; // Только буквы и цифры
            return true;
        }

        // Проверка пароля (длина 8-28 символов, обязательно буквы и цифры)
        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (password.Length < 8 || password.Length > 28) return false;

            bool hasLetter = password.Any(char.IsLetter);
            bool hasDigit = password.Any(char.IsDigit);

            if (!hasLetter || !hasDigit) return false;

            return true;
        }
    }
}