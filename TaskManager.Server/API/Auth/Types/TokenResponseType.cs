using GraphQL.Types;
using TaskManager.Server.Application.Models;

namespace TaskManager.Server.API.Auth.Types;

public class TokenResponseType : ObjectGraphType<TokenResponse>
{
    public TokenResponseType()
    {
        Field(t => t.Token);
        Field(t => t.ExpiresAt);
    }
}
