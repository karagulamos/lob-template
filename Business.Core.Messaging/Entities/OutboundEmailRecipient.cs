namespace Business.Core.Messaging.Entities
{
    public class OutboundEmailRecipient
    {
        public long OutboundEmailRecipientId { get; set; }

        public string RecipientEmail { get; set; }

        public long OutboundEmailId { get; set; }
        public virtual OutboundEmail OutboundEmail { get; set; }
    }
}
