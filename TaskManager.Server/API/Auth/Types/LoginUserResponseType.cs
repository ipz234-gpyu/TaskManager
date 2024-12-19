using GraphQL.Types;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.Auth.Types
{
    public class LoginUserResponseType : ObjectGraphType<User>
    {
        public LoginUserResponseType()
        {
            Field(t => t.Name);
            Field(t => t.Surname);
            Field(t => t.ProfilePhotoUrl);
        }
    }
}
