namespace TaskManager.Server.Application.Models
{
    public class PasswordHasherSettings
    {
        public int SaltBytesSize { get; set; }
        public int KeySize { get; set; }
        public int Iterations { get; set; }
    }
}
