using Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractService.Models
{
    /// <summary>
    /// Core contract entity representing a legal agreement
    /// </summary>
    public class Contract : BaseAuditEntity
    {
        /// <summary>
        /// Human-readable contract number/reference
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        /// <summary>
        /// Contract title or name
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the contract
        /// </summary>
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Reference to the contract type
        /// </summary>
        [Required]
        public Guid ContractTypeId { get; set; }

        /// <summary>
        /// Current status of the contract
        /// </summary>
        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        /// <summary>
        /// Priority level of the contract
        /// </summary>
        public int Priority { get; set; } = 3; // 1=High, 2=Medium, 3=Normal, 4=Low

        /// <summary>
        /// Date when the contract becomes effective
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date when the contract expires (nullable for indefinite contracts)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Date when the contract was actually signed by all parties
        /// </summary>
        public DateTime? SignedDate { get; set; }

        /// <summary>
        /// Contract value in the specified currency
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Value { get; set; }

        /// <summary>
        /// Currency code (ISO 4217 format like USD, EUR, GBP)
        /// </summary>
        [StringLength(3)]
        public string? Currency { get; set; }

        /// <summary>
        /// Billing frequency for recurring contracts
        /// </summary>
        [StringLength(20)]
        public string? BillingFrequency { get; set; } // Monthly, Quarterly, Annually, etc.

        /// <summary>
        /// Whether this contract auto-renews
        /// </summary>
        public bool AutoRenewal { get; set; } = false;

        /// <summary>
        /// Number of days before expiration to send renewal reminders
        /// </summary>
        public int RenewalReminderDays { get; set; } = 30;

        /// <summary>
        /// Duration of auto-renewal in days
        /// </summary>
        public int? AutoRenewalDurationDays { get; set; }

        /// <summary>
        /// Terms and conditions or contract content
        /// </summary>
        public string? Terms { get; set; }

        /// <summary>
        /// Internal notes about the contract (not visible to external parties)
        /// </summary>
        [StringLength(2000)]
        public string? InternalNotes { get; set; }

        /// <summary>
        /// Tags for categorization and search (comma-separated)
        /// </summary>
        [StringLength(500)]
        public string? Tags { get; set; }

        /// <summary>
        /// Custom fields as JSON for flexible data storage
        /// </summary>
        public string? CustomFields { get; set; }

        /// <summary>
        /// Reference to the parent contract (for renewals or amendments)
        /// </summary>
        public Guid? ParentContractId { get; set; }

        /// <summary>
        /// Department or team responsible for this contract
        /// </summary>
        [StringLength(100)]
        public string? Department { get; set; }

        /// <summary>
        /// Project or cost center code
        /// </summary>
        [StringLength(50)]
        public string? ProjectCode { get; set; }

        /// <summary>
        /// User ID of the contract owner/manager
        /// </summary>
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// User ID of the person who approved the contract
        /// </summary>
        public Guid? ApprovedById { get; set; }

        /// <summary>
        /// Date when the contract was approved
        /// </summary>
        public DateTime? ApprovedDate { get; set; }

        /// <summary>
        /// Reason for approval/rejection
        /// </summary>
        [StringLength(500)]
        public string? ApprovalNotes { get; set; }

        /// <summary>
        /// Date of the last status change
        /// </summary>
        public DateTime LastStatusChangeDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who made the last status change
        /// </summary>
        public Guid? LastStatusChangedById { get; set; }

        /// <summary>
        /// Reason for the last status change
        /// </summary>
        [StringLength(500)]
        public string? StatusChangeReason { get; set; }

        /// <summary>
        /// Whether notifications are enabled for this contract
        /// </summary>
        public bool NotificationsEnabled { get; set; } = true;

        /// <summary>
        /// Contract document file path or reference
        /// </summary>
        [StringLength(500)]
        public string? DocumentPath { get; set; }

        /// <summary>
        /// Original filename of the uploaded document
        /// </summary>
        [StringLength(255)]
        public string? DocumentFileName { get; set; }

        /// <summary>
        /// MIME type of the document
        /// </summary>
        [StringLength(100)]
        public string? DocumentContentType { get; set; }

        /// <summary>
        /// Size of the document in bytes
        /// </summary>
        public long? DocumentSize { get; set; }

        // Navigation Properties
        /// <summary>
        /// Contract type/category
        /// </summary>
        public virtual ContractType ContractType { get; set; } = null!;

        /// <summary>
        /// Parties involved in this contract
        /// </summary>
        public virtual ICollection<ContractParty> Parties { get; set; } = new List<ContractParty>();

        /// <summary>
        /// Parent contract (for renewals/amendments)
        /// </summary>
        public virtual Contract? ParentContract { get; set; }

        /// <summary>
        /// Child contracts (renewals/amendments)
        /// </summary>
        public virtual ICollection<Contract> ChildContracts { get; set; } = new List<Contract>();

        // Computed Properties
        /// <summary>
        /// Check if the contract is currently active
        /// </summary>
        public bool IsActive => Status == ContractStatus.Active;

        /// <summary>
        /// Check if the contract has expired
        /// </summary>
        public bool IsExpired
        {
            get
            {
                if (!EndDate.HasValue) return false;
                return DateTime.UtcNow > EndDate.Value && Status != ContractStatus.Renewed;
            }
        }

        /// <summary>
        /// Check if the contract is expiring soon
        /// </summary>
        public bool IsExpiringSoon
        {
            get
            {
                if (!EndDate.HasValue || IsExpired) return false;
                var daysUntilExpiration = (EndDate.Value - DateTime.UtcNow).Days;
                return daysUntilExpiration <= RenewalReminderDays;
            }
        }

        /// <summary>
        /// Get days until expiration (negative if expired)
        /// </summary>
        public int? DaysUntilExpiration
        {
            get
            {
                if (!EndDate.HasValue) return null;
                return (int)(EndDate.Value - DateTime.UtcNow).TotalDays;
            }
        }

        /// <summary>
        /// Check if all required parties have signed
        /// </summary>
        public bool IsFullySigned
        {
            get
            {
                var requiredSignatures = Parties.Where(p => p.RequiresSignature);
                return requiredSignatures.Any() && requiredSignatures.All(p => p.IsSigned);
            }
        }

        /// <summary>
        /// Get the primary external party (counterparty)
        /// </summary>
        public ContractParty? PrimaryCounterparty
        {
            get
            {
                return Parties
                    .Where(p => p.PartyType == PartyType.External)
                    .OrderBy(p => p.SortOrder)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Get contract duration in days
        /// </summary>
        public int? DurationDays
        {
            get
            {
                if (!EndDate.HasValue) return null;
                return (int)(EndDate.Value - StartDate).TotalDays;
            }
        }

        /// <summary>
        /// Get formatted contract value with currency
        /// </summary>
        public string FormattedValue
        {
            get
            {
                if (!Value.HasValue) return "Not specified";
                var currencySymbol = Currency switch
                {
                    "USD" => "$",
                    "EUR" => "€",
                    "GBP" => "£",
                    _ => Currency + " "
                };
                return $"{currencySymbol}{Value:N2}";
            }
        }

        /// <summary>
        /// Get priority display name
        /// </summary>
        public string PriorityDisplay => Priority switch
        {
            1 => "High",
            2 => "Medium", 
            3 => "Normal",
            4 => "Low",
            _ => "Unknown"
        };

        /// <summary>
        /// Get tags as a list
        /// </summary>
        public List<string> TagsList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Tags)) return new List<string>();
                return Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(t => t.Trim())
                          .Where(t => !string.IsNullOrEmpty(t))
                          .ToList();
            }
        }

        /// <summary>
        /// Get status display with color information
        /// </summary>
        public (string Display, string Color) StatusInfo => Status switch
        {
            ContractStatus.Draft => ("Draft", "#6c757d"),
            ContractStatus.UnderReview => ("Under Review", "#fd7e14"),
            ContractStatus.PendingApproval => ("Pending Approval", "#ffc107"),
            ContractStatus.Approved => ("Approved", "#20c997"),
            ContractStatus.Active => ("Active", "#28a745"),
            ContractStatus.Suspended => ("Suspended", "#6f42c1"),
            ContractStatus.Expired => ("Expired", "#dc3545"),
            ContractStatus.Terminated => ("Terminated", "#dc3545"),
            ContractStatus.Renewed => ("Renewed", "#17a2b8"),
            ContractStatus.Cancelled => ("Cancelled", "#6c757d"),
            _ => ("Unknown", "#6c757d")
        };
    }
}