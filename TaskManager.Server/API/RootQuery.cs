using GraphQL.Types;
using TaskManager.Server.API.GroupsFromUser;
using TaskManager.Server.API.GroupsFromTeam;
using TaskManager.Server.API.Lists;
using TaskManager.Server.API.Teams;

namespace TaskManager.Server.API
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            Field<GroupsFromUserQuery>("groupsFromUser").Resolve(_ => new { });
            Field<GroupsFromTeamQuery>("groupsFromTeam").Resolve(_ => new { });
            Field<ListQuery>("lists").Resolve(_ => new { });
            Field<TeamQuery>("teams").Resolve(_ => new { });
        }
    }
}
