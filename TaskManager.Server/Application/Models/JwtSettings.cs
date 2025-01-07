namespace TaskManager.Server.Application.Models;

public class JwtSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpiryMinutes { get; set; }
    public int RefreshTokenExpiryMinutes { get; set; }
    public int InviteTokenExpiryMinutes { get; set; }
}
