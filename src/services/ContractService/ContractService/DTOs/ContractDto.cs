namespace ContractService.DTOs
{
    public class ContractDto
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ContractTypeId { get; set; }
        public string ContractTypeName { get; set; } = string.Empty;
        public string ContractTypeColor { get; set; } = string.Empty;
        public string? ContractTypeIcon { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusDisplay { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string PriorityDisplay { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public decimal? Value { get; set; }
        public string? Currency { get; set; }
        public string? FormattedValue { get; set; }
        public string? BillingFrequency { get; set; }
        public bool AutoRenewal { get; set; }
        public int RenewalReminderDays { get; set; }
        public int? AutoRenewalDurationDays { get; set; }
        public string? Terms { get; set; }
        public string? InternalNotes { get; set; }
        public string? Tags { get; set; }
        public List<string> TagsList { get; set; } = new();
        public string? CustomFields { get; set; }
        public Guid? ParentContractId { get; set; }
        public string? Department { get; set; }
        public string? ProjectCode { get; set; }
        public Guid? OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public Guid? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovalNotes { get; set; }
        public DateTime LastStatusChangeDate { get; set; }
        public Guid? LastStatusChangedById { get; set; }
        public string? LastStatusChangedByName { get; set; }
        public string? StatusChangeReason { get; set; }
        public bool NotificationsEnabled { get; set; }
        public string? DocumentPath { get; set; }
        public string? DocumentFileName { get; set; }
        public string? DocumentContentType { get; set; }
        public long? DocumentSize { get; set; }
        public string? DocumentSizeFormatted { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; }
        public int? DaysUntilExpiration { get; set; }
        public int? DurationDays { get; set; }
        public bool IsFullySigned { get; set; }
        public List<ContractPartyDto> Parties { get; set; } = new();
        public ContractPartyDto? PrimaryCounterparty { get; set; }
        public int PartiesCount { get; set; }
        public int SignedPartiesCount { get; set; }
        public bool HasChildContracts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }
    }

    public class ContractPartyDto
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public string PartyType { get; set; } = string.Empty;
        public string PartyTypeDisplay { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? LegalName { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? ContactPersonName { get; set; }
        public string? ContactPersonTitle { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string FormattedAddress { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? Website { get; set; }
        public bool RequiresSignature { get; set; }
        public DateTime? SignedDate { get; set; }
        public string? SignedByName { get; set; }
        public string? SignedByTitle { get; set; }
        public string? SignatureData { get; set; }
        public string? SignatureIpAddress { get; set; }
        public string? Notes { get; set; }
        public int SortOrder { get; set; }
        public bool IsSigned { get; set; }
    }

    public class ContractTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool IsActive { get; set; }
        public int? DefaultDurationDays { get; set; }
        public bool RequiresApproval { get; set; }
        public bool SupportsRenewal { get; set; }
        public int DefaultReminderDays { get; set; }
        public string? Template { get; set; }
        public string? CustomFields { get; set; }
        public int SortOrder { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int ContractsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}