using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities
{
    [TableName("GroupListsFromTeam")]
    public class GroupListFromTeam
    {
        public Guid ListId { get; set; }
        public Guid GroupsFromTeamId { get; set; }
    }
}
