using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        
        // Tenant relationship
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        
        // Roles
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
    
    public class Role
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
    
    public class UserRole
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
        
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public Guid AssignedBy { get; set; }
    }
}