namespace Business.Core.Messaging.Entities
{
    public class OutboundSmsRecipient
    {
        public long OutboundSmsRecipientId { get; set; }

        public string PhoneNumber { get; set; }

        public long OutboundSmsId { get; set; }
        public virtual OutboundSms OutboundSms { get; set; }
    }
}
