using GraphQL.Types;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.Users.Types
{
    public class UserResponseType : ObjectGraphType<User>
    {
        public UserResponseType()
        {
            Field(x => x.Name).Description("The name of the user.");
            Field(x => x.Surname).Description("The surname of the user.");
            Field(x => x.ProfilePhotoUrl).Description("This is a link to the user profile photo.");
        }
    }
}
