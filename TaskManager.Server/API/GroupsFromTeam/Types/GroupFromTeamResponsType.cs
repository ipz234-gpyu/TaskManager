using GraphQL.Types;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.GroupsFromTeam.Types
{
    public class GroupFromTeamResponsType : ObjectGraphType<GroupFromTeam>
    {
        public GroupFromTeamResponsType()
        {
            Field(t => t.GroupsFromTeamId);
            Field(t => t.Name);
            Field(t => t.Priority);
        }
    }
}
