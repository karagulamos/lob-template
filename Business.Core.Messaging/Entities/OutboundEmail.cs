using System;
using System.Collections.Generic;
using Business.Core.Messaging.Common.Enums;

namespace Business.Core.Messaging.Entities
{
    public class OutboundEmail : ITrackable
    {
        public OutboundEmail()
        {
            OutboundAttachments = new HashSet<OutboundAttachment>();
            OutboundRecipients = new HashSet<OutboundEmailRecipient>();
            OutboundImages = new HashSet<OutboundImage>();
        }

        public long OutboundEmailId { get; set; }

        public DateTime? DateSent { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public EmailStatus EmailStatus { get; set; }
        public int Attempts { get; set; }
        public bool IsHtml { get; set; }
        public int DescriptorId { get; set; }

        public virtual ICollection<OutboundAttachment> OutboundAttachments { get; set; }
        public virtual ICollection<OutboundEmailRecipient> OutboundRecipients { get; set; }
        public virtual ICollection<OutboundImage> OutboundImages { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
