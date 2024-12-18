using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManager.Server.Application.Interfaces;
using TaskManager.Server.Application.Models;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Application.Services.Authentication
{
    public class JwtTokenUtils : IJwtTokenUtils
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRefreshTokenRepository _userRefreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTokenUtils(
            IOptions<JwtSettings> jwtOptions,
            IUserRefreshTokenRepository userRefreshTokenRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _jwtSettings = jwtOptions.Value;
            _userRefreshTokenRepository = userRefreshTokenRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public TokenResponse GenerateToken(Claim[] claims, int expireMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddMinutes(expireMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var responseToken = tokenHandler.WriteToken(token);

            var expiresAtTimeStamp = new DateTimeOffset(expires).ToUnixTimeMilliseconds();

            return new TokenResponse(responseToken, expiresAtTimeStamp);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            }, out _);
        }

        public TokenResponse GenerateAccessToken(Guid UserId)
        {
            var accessToken = GenerateToken(
                [new(ClaimTypes.NameIdentifier, UserId.ToString())],
                _jwtSettings.AccessTokenExpiryMinutes
            );

            return accessToken;
        }

        public async Task<TokenResponse> GenerateRefreshToken(Guid UserId, string deviceInfo)
        {
            var user = await _userRepository.GetByIdAsync(UserId);

            if (user == null)
                throw new Exception();

            var userRefreshToken = new UserRefreshToken()
            {
                UserId = user.UserId,
                DeviceInfo = deviceInfo,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpiryMinutes),
                RefreshTokenHash = Convert.ToHexString(RandomNumberGenerator.GetBytes(16))
            };

            userRefreshToken = await _userRefreshTokenRepository.CreateAsync(userRefreshToken);

            var refreshToken = GenerateToken(
                [
                    new(ClaimTypes.NameIdentifier, userRefreshToken.ToString()),
                    new("hash", userRefreshToken.RefreshTokenHash)
                ],
                _jwtSettings.RefreshTokenExpiryMinutes
            );

            return refreshToken;
        }

        public async Task RevokeRefreshToken(Guid TokenId, string tokenHash)
        {
            var userRefreshToken = await _userRefreshTokenRepository.GetByIdAsync(TokenId);
            if (userRefreshToken == null)
            {
                throw new Exception();
            }

            if (userRefreshToken.RefreshTokenHash != tokenHash)
            {
                throw new Exception();
            }

            await _userRefreshTokenRepository.DeleteAsync(TokenId);
        }
    }
}
