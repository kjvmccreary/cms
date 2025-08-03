using System;

namespace Shared.DTOs
{
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTenantDto
    {
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }

    public class UpdateTenantDto
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}