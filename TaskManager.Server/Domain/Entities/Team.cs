using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities;

[TableName("Teams")]
[ColumnIdName("TeamId")]
public class Team
{
    public Guid TeamId { get; set; }
    public string NameTeam { get; set; }
    public Guid CreatedBy { get; set; }
}
