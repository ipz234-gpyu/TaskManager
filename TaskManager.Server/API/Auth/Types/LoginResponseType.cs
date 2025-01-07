using GraphQL.Types;
using TaskManager.Server.API.Auth.Models;

namespace TaskManager.Server.API.Auth.Types;

public class LoginResponseType : ObjectGraphType<LoginResponse>
{
    public LoginResponseType()
    {
        Field<LoginUserResponseType>("user").Resolve(context => context.Source.User);
        Field<TokenResponseType>("accessToken").Resolve(context => context.Source.AccessToken);
        Field<TokenResponseType>("refreshToken").Resolve(context => context.Source.RefreshToken);
    }
}
