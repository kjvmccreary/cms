using System.ComponentModel.DataAnnotations;

namespace ContractService.DTOs
{
    public class CreateContractDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string? Description { get; set; }
        
        [Required]
        public Guid ContractTypeId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Range(1, 4)]
        public int Priority { get; set; } = 3;
        
        [Range(0, double.MaxValue)]
        public decimal? Value { get; set; }
        
        [StringLength(3)]
        public string? Currency { get; set; }
        
        [StringLength(20)]
        public string? BillingFrequency { get; set; }
        
        public bool AutoRenewal { get; set; } = false;
        
        [Range(1, 365)]
        public int RenewalReminderDays { get; set; } = 30;
        
        [Range(1, 3650)]
        public int? AutoRenewalDurationDays { get; set; }
        
        public string? Terms { get; set; }
        
        [StringLength(2000)]
        public string? InternalNotes { get; set; }
        
        [StringLength(500)]
        public string? Tags { get; set; }
        
        public string? CustomFields { get; set; }
        
        public Guid? ParentContractId { get; set; }
        
        [StringLength(100)]
        public string? Department { get; set; }
        
        [StringLength(50)]
        public string? ProjectCode { get; set; }
        
        public Guid? OwnerId { get; set; }
        
        public bool NotificationsEnabled { get; set; } = true;
        
        public List<CreateContractPartyDto> Parties { get; set; } = new();
    }

    public class UpdateContractDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string? Description { get; set; }
        
        [Required]
        public Guid ContractTypeId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Range(1, 4)]
        public int Priority { get; set; } = 3;
        
        [Range(0, double.MaxValue)]
        public decimal? Value { get; set; }
        
        [StringLength(3)]
        public string? Currency { get; set; }
        
        [StringLength(20)]
        public string? BillingFrequency { get; set; }
        
        public bool AutoRenewal { get; set; } = false;
        
        [Range(1, 365)]
        public int RenewalReminderDays { get; set; } = 30;
        
        [Range(1, 3650)]
        public int? AutoRenewalDurationDays { get; set; }
        
        public string? Terms { get; set; }
        
        [StringLength(2000)]
        public string? InternalNotes { get; set; }
        
        [StringLength(500)]
        public string? Tags { get; set; }
        
        public string? CustomFields { get; set; }
        
        [StringLength(100)]
        public string? Department { get; set; }
        
        [StringLength(50)]
        public string? ProjectCode { get; set; }
        
        public Guid? OwnerId { get; set; }
        
        public bool NotificationsEnabled { get; set; } = true;
    }

    public class CreateContractPartyDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? LegalName { get; set; }
        
        [Required]
        public int PartyType { get; set; } // 0=Internal, 1=External, 2=Witness, 3=LegalRepresentative
        
        [StringLength(100)]
        public string? ContactPersonName { get; set; }
        
        [StringLength(100)]
        public string? ContactPersonTitle { get; set; }
        
        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }
        
        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(200)]
        public string? AddressLine1 { get; set; }
        
        [StringLength(200)]
        public string? AddressLine2 { get; set; }
        
        [StringLength(100)]
        public string? City { get; set; }
        
        [StringLength(100)]
        public string? State { get; set; }
        
        [StringLength(20)]
        public string? PostalCode { get; set; }
        
        [StringLength(100)]
        public string? Country { get; set; }
        
        [StringLength(50)]
        public string? TaxId { get; set; }
        
        [StringLength(50)]
        public string? RegistrationNumber { get; set; }
        
        [Url]
        [StringLength(255)]
        public string? Website { get; set; }
        
        public bool RequiresSignature { get; set; } = true;
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public int SortOrder { get; set; } = 0;
    }

    public class CreateContractTypeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(7)]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be a valid hex color code")]
        public string Color { get; set; } = "#007bff";
        
        [StringLength(50)]
        public string? Icon { get; set; }
        
        [Range(1, 3650)]
        public int? DefaultDurationDays { get; set; }
        
        public bool RequiresApproval { get; set; } = true;
        
        public bool SupportsRenewal { get; set; } = true;
        
        [Range(1, 365)]
        public int DefaultReminderDays { get; set; } = 30;
        
        public string? Template { get; set; }
        
        public string? CustomFields { get; set; }
        
        public int SortOrder { get; set; } = 0;
    }

    public class ContractStatusChangeDto
    {
        [Required]
        [StringLength(50)]
        public string NewStatus { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Reason { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class ContractSignatureDto
    {
        [Required]
        public Guid PartyId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string SignedByName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? SignedByTitle { get; set; }
        
        public string? SignatureData { get; set; }
    }
}