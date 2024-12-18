using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities
{
    [TableName("TaskTag")]
    public class TaskTag
    {
        public Guid TaskId { get; set; }
        public Guid TagId { get; set; }
    }
}
