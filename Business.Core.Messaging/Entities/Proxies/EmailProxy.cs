using System;
using System.Collections.Generic;
using Business.Core.Messaging.Common.Enums;

namespace Business.Core.Messaging.Entities.Proxies
{
    public class EmailProxy
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public int DescriptorId { get; set; }

        public List<EmailRecipientProxy> Recipients { get; set; }
        public List<AttachmentProxy> Attachments { get; set; }

        public static explicit operator OutboundEmail(EmailProxy email)
        {
            var dbemail = new OutboundEmail
            {
                Sender = email.From,
                Body = email.Body,
                IsHtml = email.IsHtml,
                Subject = email.Subject,
                DescriptorId = email.DescriptorId,
                DateCreated = DateTime.Now,
                DateSent = DateTime.Now,
                EmailStatus = EmailStatus.New,
                Attempts = 0,
                OutboundAttachments = new List<OutboundAttachment>(),
                OutboundRecipients = new List<OutboundEmailRecipient>()
            };

            email.Attachments.ForEach(att => dbemail.OutboundAttachments.Add((OutboundAttachment)att));
            email.Recipients.ForEach(rec => dbemail.OutboundRecipients.Add((OutboundEmailRecipient)rec));

            return dbemail;
        }
    }
}
