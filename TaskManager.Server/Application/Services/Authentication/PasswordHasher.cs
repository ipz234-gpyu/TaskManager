using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Models;

namespace TaskManager.Server.Application.Services.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasherSettings _settings;

    public PasswordHasher(IOptions<PasswordHasherSettings> settings)
    {
        _settings = settings.Value;
    }

    public HashPasswordResponse HashPassword(string password)
    {
        byte[] salt_bytes = RandomNumberGenerator.GetBytes(_settings.SaltBytesSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt_bytes,
            _settings.Iterations,
            HashAlgorithmName.SHA256,
            _settings.KeySize);

        return new HashPasswordResponse()
        {
            Password = Convert.ToHexString(hash),
            Salt = Convert.ToHexString(salt_bytes)
        };
    }

    public bool VerifyHash(string password, string hashedPassword, string salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            password,
            Convert.FromHexString(salt),
            _settings.Iterations,
            HashAlgorithmName.SHA256,
            _settings.KeySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hashedPassword));
    }
}
