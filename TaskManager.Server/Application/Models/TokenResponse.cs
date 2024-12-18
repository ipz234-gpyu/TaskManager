namespace TaskManager.Server.Application.Models
{
    public record TokenResponse
    (
        string Token,
        long ExpiresAt
    );
}
