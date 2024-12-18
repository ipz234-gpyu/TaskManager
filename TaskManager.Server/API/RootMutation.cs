using GraphQL.Types;
using TaskManager.Server.API.Users;

namespace TaskManager.Server.API
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation()
        {
            Field<UserMutation>("users").Resolve(_ => new { });
        }
    }
}
