using System.Text.RegularExpressions;

namespace TaskManager.Server.Application.Services
{
    public static class DataValidator
    {
        public static bool IsEmailValid(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        public static bool IsPasswordValid(string password)
        {
            return !(password is null || password.Length < 8 || password.Length > 20);
        }
    }
}
