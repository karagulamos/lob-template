using System.Collections.Generic;
using System.Text;

namespace Business.Core.Messaging.Entities.Proxies
{
    public class OutboundSmtpProxy
    {
        public OutboundSmtpProxy()
        {
            OutboundImages = new HashSet<OutboundImage>();
            OutboundAttachments = new HashSet<OutboundAttachment>();
            OutboundRecipients = new HashSet<OutboundEmailRecipient>();
        }

        public string OutboundHost { get; set; }
        public int OutboundPort { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Body { get; set; }
        public bool UseSsl { get; set; }
        public bool IsHtml { get; set; }
        public int DescriptorId { get; set; }
        public OutboundEmail OutboundEmail { get; set; }

        public ICollection<OutboundAttachment> OutboundAttachments { get; set; }
        public ICollection<OutboundEmailRecipient> OutboundRecipients { get; set; }
        public ICollection<OutboundImage> OutboundImages { get; set; }

        public string ToLongString()
        {
            var sb = new StringBuilder();
            foreach (var recipient in OutboundRecipients)
            {
                sb.Append(recipient.RecipientEmail + "; ");
            }

            return "Email with Subject (" + Subject + ") to Recipients (" + sb + ")";
        }
    }
}