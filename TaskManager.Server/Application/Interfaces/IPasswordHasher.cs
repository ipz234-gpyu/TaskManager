using TaskManager.Server.Application.Models;

namespace TaskManager.Server.Application.Interfaces;

public interface IPasswordHasher
{
    HashPasswordResponse HashPassword(string password);
    bool VerifyHash(string password, string hashedPassword, string salt);
}
