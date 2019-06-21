namespace Business.Core.Messaging.Entities
{
    public class OutboundAttachment
    {
        public long OutboundAttachmentId { get; set; }

        public string FilePath { get; set; }

        public long OutboundEmailId { get; set; }
        public virtual OutboundEmail OutboundEmail { get; set; }
    }
}
