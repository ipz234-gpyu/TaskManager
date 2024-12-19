using TaskManager.Server.Application.Models;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.Auth.Models
{
    public record AuthorizeResponse(
         User User,
         TokenResponse AccessToken
    );
}
