using GraphQL.Types;
using TaskManager.Server.API.Auth;
using TaskManager.Server.API.GroupsFromUser;
using TaskManager.Server.API.Lists;
using TaskManager.Server.API.Users;

namespace TaskManager.Server.API
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation()
        {
            Field<UserMutation>("users").Resolve(_ => new { });
            Field<AuthMutation>("auth").Resolve(_ => new { });
            Field<GroupsFromUserMutation>("groupsFromUser").Resolve(_ => new { });
            Field<ListMutation>("lists").Resolve(_ => new { });
        }
    }
}
