using GraphQL.Types;
using TaskManager.Server.API.Auth;
using TaskManager.Server.API.GroupsFromUser;
using TaskManager.Server.API.GroupsFromTeam;
using TaskManager.Server.API.Lists;
using TaskManager.Server.API.Teams;
using TaskManager.Server.API.Users;
using TaskManager.Server.API.Tasks;
using TaskManager.Server.API.Tags;

namespace TaskManager.Server.API;

public class RootMutation : ObjectGraphType
{
    public RootMutation()
    {
        Field<UserMutation>("users").Resolve(_ => new { });
        Field<AuthMutation>("auth").Resolve(_ => new { });
        Field<GroupsFromUserMutation>("groupsFromUser").Resolve(_ => new { });
        Field<GroupsFromTeamMutation>("groupsFromTeam").Resolve(_ => new { });
        Field<ListMutation>("lists").Resolve(_ => new { });
        Field<TeamMutation>("teams").Resolve(_ => new { });
        Field<TaskMutation>("tasks").Resolve(_ => new { });
        Field<TagMutation>("tags").Resolve(_ => new { });
    }
}
