using TaskManager.Server.Application.Models;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.Auth.Models;

public record LoginResponse(
     User User,
     TokenResponse AccessToken,
     TokenResponse RefreshToken
);
