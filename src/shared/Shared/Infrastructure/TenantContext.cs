using System;

namespace Shared.Infrastructure
{
    public interface ITenantContext
    {
        Guid TenantId { get; }
        Guid UserId { get; }
        string TenantDomain { get; }
        string UserEmail { get; }
        string[] UserRoles { get; }
        bool IsAuthenticated { get; }
    }

    public class TenantContext : ITenantContext
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string TenantDomain { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string[] UserRoles { get; set; } = Array.Empty<string>();
        public bool IsAuthenticated { get; set; }
    }
}