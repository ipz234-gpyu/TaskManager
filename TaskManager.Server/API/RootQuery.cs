using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.GroupsFromUser;

namespace TaskManager.Server.API
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            Field<GroupsFromUserQuery>("groupsFromUser").Resolve(_ => new { });
        }
    }
}
