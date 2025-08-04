using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class Tenant
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Domain { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}