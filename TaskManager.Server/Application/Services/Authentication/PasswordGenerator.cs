using System.Text;

namespace TaskManager.Server.Application.Services.Authentication
{
    public class PasswordGenerator
    {
        public static string GeneratePassword(int length = 8)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var password = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(characters.Length);
                password.Append(characters[randomIndex]);
            }

            return password.ToString();
        }
    }
}
