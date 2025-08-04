using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace ContractService.Models
{
    /// <summary>
    /// Represents a contract type/category for organizing contracts
    /// </summary>
    public class ContractType : BaseEntity
    {
        /// <summary>
        /// The name of the contract type (e.g., "Employment", "Service Agreement", "NDA")
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of this contract type
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Color code for UI display (hex format)
        /// </summary>
        [StringLength(7)]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be a valid hex color code")]
        public string Color { get; set; } = "#007bff";

        /// <summary>
        /// Icon name for UI display
        /// </summary>
        [StringLength(50)]
        public string? Icon { get; set; }

        /// <summary>
        /// Whether this contract type is active and available for selection
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Default duration in days for contracts of this type
        /// </summary>
        public int? DefaultDurationDays { get; set; }

        /// <summary>
        /// Whether contracts of this type require approval workflow
        /// </summary>
        public bool RequiresApproval { get; set; } = true;

        /// <summary>
        /// Whether contracts of this type support renewal
        /// </summary>
        public bool SupportsRenewal { get; set; } = true;

        /// <summary>
        /// Default reminder days before expiration
        /// </summary>
        public int DefaultReminderDays { get; set; } = 30;

        /// <summary>
        /// Template content or instructions for this contract type
        /// </summary>
        public string? Template { get; set; }

        /// <summary>
        /// Custom fields configuration as JSON
        /// </summary>
        public string? CustomFields { get; set; }

        /// <summary>
        /// Sort order for displaying contract types
        /// </summary>
        public int SortOrder { get; set; } = 0;

        // Navigation Properties
        /// <summary>
        /// Contracts using this contract type
        /// </summary>
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

        /// <summary>
        /// Get display name with active status indicator
        /// </summary>
        public string DisplayName => IsActive ? Name : $"{Name} (Inactive)";

        /// <summary>
        /// Get contracts count for this type within tenant
        /// </summary>
        public int ContractsCount => Contracts?.Count ?? 0;
    }
}