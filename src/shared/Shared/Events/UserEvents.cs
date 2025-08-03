using System;

namespace Shared.Events
{
    public class UserCreatedEvent : BaseEvent
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
        
        public UserCreatedEvent()
        {
            EventType = nameof(UserCreatedEvent);
            Source = "IdentityService";
        }
    }

    public class UserLoginEvent : BaseEvent
    {
        public string Email { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? FailureReason { get; set; }
        
        public UserLoginEvent()
        {
            EventType = nameof(UserLoginEvent);
            Source = "IdentityService";
        }
    }

    public class TenantCreatedEvent : BaseEvent
    {
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        
        public TenantCreatedEvent()
        {
            EventType = nameof(TenantCreatedEvent);
            Source = "IdentityService";
        }
    }
}