using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace ContractService.Models
{
    /// <summary>
    /// Represents the type of party in a contract
    /// </summary>
    public enum PartyType
    {
        /// <summary>
        /// The organization that owns the contract (our company)
        /// </summary>
        Internal = 0,

        /// <summary>
        /// External party or counterparty
        /// </summary>
        External = 1,

        /// <summary>
        /// Third-party witness or stakeholder
        /// </summary>
        Witness = 2,

        /// <summary>
        /// Legal representative or attorney
        /// </summary>
        LegalRepresentative = 3
    }

    /// <summary>
    /// Represents a party involved in a contract (individual or organization)
    /// </summary>
    public class ContractParty : BaseEntity
    {
        /// <summary>
        /// Reference to the contract this party belongs to
        /// </summary>
        [Required]
        public Guid ContractId { get; set; }

        /// <summary>
        /// Type of party in the contract relationship
        /// </summary>
        [Required]
        public PartyType PartyType { get; set; }

        /// <summary>
        /// Name of the organization or individual
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Legal name if different from display name
        /// </summary>
        [StringLength(200)]
        public string? LegalName { get; set; }

        /// <summary>
        /// Contact person's name for organizations
        /// </summary>
        [StringLength(100)]
        public string? ContactPersonName { get; set; }

        /// <summary>
        /// Job title of the contact person
        /// </summary>
        [StringLength(100)]
        public string? ContactPersonTitle { get; set; }

        /// <summary>
        /// Primary email address
        /// </summary>
        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Primary phone number
        /// </summary>
        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; }

        /// <summary>
        /// Mailing address line 1
        /// </summary>
        [StringLength(200)]
        public string? AddressLine1 { get; set; }

        /// <summary>
        /// Mailing address line 2
        /// </summary>
        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [StringLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// State or province
        /// </summary>
        [StringLength(100)]
        public string? State { get; set; }

        /// <summary>
        /// Postal or ZIP code
        /// </summary>
        [StringLength(20)]
        public string? PostalCode { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        [StringLength(100)]
        public string? Country { get; set; }

        /// <summary>
        /// Tax identification number
        /// </summary>
        [StringLength(50)]
        public string? TaxId { get; set; }

        /// <summary>
        /// Business registration number
        /// </summary>
        [StringLength(50)]
        public string? RegistrationNumber { get; set; }

        /// <summary>
        /// Website URL
        /// </summary>
        [StringLength(255)]
        [Url]
        public string? Website { get; set; }

        /// <summary>
        /// Whether this party is required to sign the contract
        /// </summary>
        public bool RequiresSignature { get; set; } = true;

        /// <summary>
        /// Date when this party signed the contract
        /// </summary>
        public DateTime? SignedDate { get; set; }

        /// <summary>
        /// Name of the person who signed on behalf of this party
        /// </summary>
        [StringLength(100)]
        public string? SignedByName { get; set; }

        /// <summary>
        /// Title of the person who signed
        /// </summary>
        [StringLength(100)]
        public string? SignedByTitle { get; set; }

        /// <summary>
        /// Digital signature data or reference
        /// </summary>
        public string? SignatureData { get; set; }

        /// <summary>
        /// IP address from which the signature was made
        /// </summary>
        [StringLength(45)]
        public string? SignatureIpAddress { get; set; }

        /// <summary>
        /// Additional notes about this party
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Sort order for displaying parties
        /// </summary>
        public int SortOrder { get; set; } = 0;

        // Navigation Properties
        /// <summary>
        /// The contract this party belongs to
        /// </summary>
        public virtual Contract Contract { get; set; } = null!;

        /// <summary>
        /// Check if this party has signed the contract
        /// </summary>
        public bool IsSigned => SignedDate.HasValue;

        /// <summary>
        /// Get full address as formatted string
        /// </summary>
        public string FormattedAddress
        {
            get
            {
                var parts = new List<string>();
                
                if (!string.IsNullOrWhiteSpace(AddressLine1))
                    parts.Add(AddressLine1);
                    
                if (!string.IsNullOrWhiteSpace(AddressLine2))
                    parts.Add(AddressLine2);
                    
                var cityStateZip = new List<string>();
                if (!string.IsNullOrWhiteSpace(City))
                    cityStateZip.Add(City);
                if (!string.IsNullOrWhiteSpace(State))
                    cityStateZip.Add(State);
                if (!string.IsNullOrWhiteSpace(PostalCode))
                    cityStateZip.Add(PostalCode);
                    
                if (cityStateZip.Any())
                    parts.Add(string.Join(", ", cityStateZip.Take(2)) + (cityStateZip.Count > 2 ? " " + cityStateZip.Last() : ""));
                    
                if (!string.IsNullOrWhiteSpace(Country))
                    parts.Add(Country);
                    
                return string.Join("\n", parts);
            }
        }

        /// <summary>
        /// Get display name for the party
        /// </summary>
        public string DisplayName => LegalName ?? Name;
    }
}