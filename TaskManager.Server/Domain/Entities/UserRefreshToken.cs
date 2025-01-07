using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities;

[TableName("UserRefreshTokens")]
[ColumnIdName("TokenId")]
public class UserRefreshToken
{
    public Guid TokenId { get; set; }
    public Guid UserId { get; set; }
    public string RefreshTokenHash { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? DeviceInfo {  get; set; }

}
