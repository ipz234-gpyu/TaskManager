using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities
{
    [TableName("Users")]
    [ColumnIdName("UserId")]
    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string? ProfilePhotoUrl { get; set; }
    }
}
