﻿using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities
{
    [TableName("TeamInvitations")]
    public class TeamInvitation
    {
        public string TokenHash { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
    }
}
