using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities
{
    [TableName("GroupsFromTeam")]
    [ColumnIdName("GroupsFromTeamId")]
    public class GroupFromTeam
    {
        public Guid GroupsFromTeamId { get; set; }
        public Guid TeamId { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
    }
}
