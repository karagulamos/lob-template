namespace Business.Core.Messaging.Entities.Proxies
{
    public class EmailRecipientProxy
    {
        public string EmailAddress { get; set; }

        public static explicit operator OutboundEmailRecipient(EmailRecipientProxy recipient)
        {
            return new OutboundEmailRecipient
            {
                RecipientEmail = recipient.EmailAddress
            };
        }
    }
}
