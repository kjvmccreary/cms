using System;

namespace Shared.Events
{
    public class ContractCreatedEvent : BaseEvent
    {
        public Guid ContractId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContractType { get; set; } = string.Empty;
        public string Counterparty { get; set; } = string.Empty;
        public decimal? Value { get; set; }
        public string? Currency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        public ContractCreatedEvent()
        {
            EventType = nameof(ContractCreatedEvent);
            Source = "ContractService";
        }
    }

    public class ContractStatusChangedEvent : BaseEvent
    {
        public Guid ContractId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PreviousStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string? Reason { get; set; }
        
        public ContractStatusChangedEvent()
        {
            EventType = nameof(ContractStatusChangedEvent);
            Source = "ContractService";
        }
    }

    public class ContractExpirationWarningEvent : BaseEvent
    {
        public Guid ContractId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public int DaysUntilExpiration { get; set; }
        
        public ContractExpirationWarningEvent()
        {
            EventType = nameof(ContractExpirationWarningEvent);
            Source = "ContractService";
        }
    }
}