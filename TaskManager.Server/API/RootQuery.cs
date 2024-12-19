using GraphQL;
using GraphQL.Types;
using TaskManager.Server.API.GroupsFromUser;
using TaskManager.Server.API.Lists;

namespace TaskManager.Server.API
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            Field<GroupsFromUserQuery>("groupsFromUser").Resolve(_ => new { });
            Field<ListQuery>("lists").Resolve(_ => new { });
        }
    }
}
