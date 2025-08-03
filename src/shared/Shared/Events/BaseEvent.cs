using System;

namespace Shared.Events
{
    public abstract class BaseEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty; // Which service published this event
    }
}