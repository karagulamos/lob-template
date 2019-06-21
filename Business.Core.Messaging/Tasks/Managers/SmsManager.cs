using System.Collections.Generic;
using System.Linq;
using Business.Core.Messaging.Common.Exceptions;
using Business.Core.Messaging.Entities;
using Business.Core.Messaging.Entities.Proxies;
using Business.Core.Messaging.Persistence;
using Business.Core.Messaging.Persistence.Repository;
using Business.Core.Messaging.Tasks.Config;

namespace Business.Core.Messaging.Tasks.Managers
{

    internal class SmsManager : ISmsManager
    {
        private readonly IOutboundSmsRepository _smsRepository;

        public SmsManager() : this(DataFactory.Get<IOutboundSmsRepository>())
        {
        }

        public SmsManager(IOutboundSmsRepository smsRepository)
        {
            _smsRepository = smsRepository;
        }

        public void Send(string from, string phoneNumber, string body)
        {
            Send(from, new List<string> { phoneNumber }, body);
        }

        public void Send(string from, List<string> phoneNumbers, string body)
        {
            var sms = new SmsProxy
            {
                Body = body,
                From = from,
                Recipients = new List<SmsRecipientProxy>()
            };

            foreach (var phoneNumber in phoneNumbers.Where(phoneNumber => !string.IsNullOrEmpty(phoneNumber)))
            {
                sms.Recipients.Add(new SmsRecipientProxy
                {
                    PhoneNumber = phoneNumber
                });
            }

            Send(sms);
        }

        private void Send(SmsProxy sms)
        {
            if (string.IsNullOrEmpty(sms.Body))
                throw new SmsFormatException("Body of message cannot be empty");

            if (sms.Recipients.Count == 0)
                throw new SmsFormatException("A minimum of one recipient is required");

            if (string.IsNullOrEmpty(sms.From))
                throw new SmsFormatException("From field cannot be empty");

            //foreach (var recipient in sms.Recipients)
            //{
            //    if (!recipient.PhoneNumber.IsValidNGNMobile())
            //        throw new SmsFormatException("Invalid Mobile number - " + recipient.PhoneNumber);
            //}

            _smsRepository.Add((OutboundSms)sms);

            _smsRepository.CommitChanges();
        }
    }
}
