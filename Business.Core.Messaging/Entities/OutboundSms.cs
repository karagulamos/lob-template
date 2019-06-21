using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Core.Messaging.Entities
{
    public class OutboundSms : ITrackable
    {
        public OutboundSms()
        {
            SmsRecipients = new HashSet<OutboundSmsRecipient>();
        }

        public long OutboundSmsId { get; set; }

        public DateTime? DateSent { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public int Sent { get; set; }
        public int Attempts { get; set; }

        public virtual ICollection<OutboundSmsRecipient> SmsRecipients { get; set; }

        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }

        public string ToLongString()
        {
            var sb = new StringBuilder();
            foreach (var recipient in SmsRecipients)
            {
                sb.Append(recipient.PhoneNumber + "; ");
            }

            return "SMS Message to Recipients (" + sb + ")";
        }
    }
}
