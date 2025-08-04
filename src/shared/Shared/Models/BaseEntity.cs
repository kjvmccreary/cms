using System;

namespace Shared.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        // Multi-tenancy
        public Guid TenantId { get; set; }
    }

    public abstract class BaseAuditEntity : BaseEntity
    {
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}