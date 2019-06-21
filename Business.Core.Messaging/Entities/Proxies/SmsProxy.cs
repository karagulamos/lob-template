using System;
using System.Collections.Generic;

namespace Business.Core.Messaging.Entities.Proxies
{
    public class SmsProxy
    {
        public string From { get; set; }
        public List<SmsRecipientProxy> Recipients { get; set; }
        public string Body { get; set; }

        public static explicit operator OutboundSms(SmsProxy sms)
        {
            var outboundSms = new OutboundSms
            {
                Sender = sms.From,
                Message = sms.Body,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                Sent = 0,
                Attempts = 0,
                SmsRecipients = new List<OutboundSmsRecipient>()

            };

            sms.Recipients.ForEach(recipient => outboundSms.SmsRecipients.Add(new OutboundSmsRecipient
            {
                PhoneNumber = recipient.PhoneNumber
            }));

            return outboundSms;
        }
    }
}
