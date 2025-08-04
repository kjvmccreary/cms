using System.ComponentModel;

namespace ContractService.Models
{
    /// <summary>
    /// Represents the lifecycle status of a contract
    /// </summary>
    public enum ContractStatus
    {
        /// <summary>
        /// Contract is being drafted and is not yet active
        /// </summary>
        [Description("Draft")]
        Draft = 0,

        /// <summary>
        /// Contract is under review by stakeholders
        /// </summary>
        [Description("Under Review")]
        UnderReview = 1,

        /// <summary>
        /// Contract is pending approval from authorized parties
        /// </summary>
        [Description("Pending Approval")]
        PendingApproval = 2,

        /// <summary>
        /// Contract has been approved and is awaiting signatures
        /// </summary>
        [Description("Approved")]
        Approved = 3,

        /// <summary>
        /// Contract is active and in effect
        /// </summary>
        [Description("Active")]
        Active = 4,

        /// <summary>
        /// Contract has been suspended temporarily
        /// </summary>
        [Description("Suspended")]
        Suspended = 5,

        /// <summary>
        /// Contract has expired naturally
        /// </summary>
        [Description("Expired")]
        Expired = 6,

        /// <summary>
        /// Contract has been terminated before expiration
        /// </summary>
        [Description("Terminated")]
        Terminated = 7,

        /// <summary>
        /// Contract has been renewed for another term
        /// </summary>
        [Description("Renewed")]
        Renewed = 8,

        /// <summary>
        /// Contract has been cancelled
        /// </summary>
        [Description("Cancelled")]
        Cancelled = 9
    }

    /// <summary>
    /// Helper class for ContractStatus operations
    /// </summary>
    public static class ContractStatusExtensions
    {
        /// <summary>
        /// Get the description attribute value for the status
        /// </summary>
        public static string GetDescription(this ContractStatus status)
        {
            var field = status.GetType().GetField(status.ToString());
            var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute));
            return attribute?.Description ?? status.ToString();
        }

        /// <summary>
        /// Check if the status represents an active contract
        /// </summary>
        public static bool IsActive(this ContractStatus status)
        {
            return status == ContractStatus.Active;
        }

        /// <summary>
        /// Check if the status represents a finalized contract (cannot be modified)
        /// </summary>
        public static bool IsFinalized(this ContractStatus status)
        {
            return status is ContractStatus.Expired or 
                           ContractStatus.Terminated or 
                           ContractStatus.Cancelled or 
                           ContractStatus.Renewed;
        }

        /// <summary>
        /// Get valid next statuses for the current status
        /// </summary>
        public static List<ContractStatus> GetValidNextStatuses(this ContractStatus currentStatus)
        {
            return currentStatus switch
            {
                ContractStatus.Draft => new() { ContractStatus.UnderReview, ContractStatus.Cancelled },
                ContractStatus.UnderReview => new() { ContractStatus.Draft, ContractStatus.PendingApproval, ContractStatus.Cancelled },
                ContractStatus.PendingApproval => new() { ContractStatus.UnderReview, ContractStatus.Approved, ContractStatus.Cancelled },
                ContractStatus.Approved => new() { ContractStatus.Active, ContractStatus.Cancelled },
                ContractStatus.Active => new() { ContractStatus.Suspended, ContractStatus.Expired, ContractStatus.Terminated, ContractStatus.Renewed },
                ContractStatus.Suspended => new() { ContractStatus.Active, ContractStatus.Terminated, ContractStatus.Cancelled },
                ContractStatus.Expired => new() { ContractStatus.Renewed },
                ContractStatus.Terminated => new(),
                ContractStatus.Renewed => new(),
                ContractStatus.Cancelled => new(),
                _ => new()
            };
        }
    }
}