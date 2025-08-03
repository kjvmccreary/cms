using System;

namespace Shared.Events
{
    public class DocumentUploadedEvent : BaseEvent
    {
        public Guid DocumentId { get; set; }
        public Guid ContractId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;
        
        public DocumentUploadedEvent()
        {
            EventType = nameof(DocumentUploadedEvent);
            Source = "DocumentService";
        }
    }

    public class DocumentDeletedEvent : BaseEvent
    {
        public Guid DocumentId { get; set; }
        public Guid ContractId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        
        public DocumentDeletedEvent()
        {
            EventType = nameof(DocumentDeletedEvent);
            Source = "DocumentService";
        }
    }
}