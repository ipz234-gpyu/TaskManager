using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities
{
    [TableName("UserTeams")]
    [ColumnIdName("UserTeamId")]
    public class UserTeam
    {
        public Guid UserTeamId { get; set; }
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
    }
}
