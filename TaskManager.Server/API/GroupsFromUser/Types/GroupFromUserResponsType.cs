using GraphQL.Types;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.GroupsFromUser.Types
{
    public class GroupFromUserResponsType : ObjectGraphType<GroupFromUser>
    {
        public GroupFromUserResponsType()
        {
            Field(t => t.GroupsFromUserId);
            Field(t => t.Name);
            Field(t => t.Priority);
        }
    }
}
