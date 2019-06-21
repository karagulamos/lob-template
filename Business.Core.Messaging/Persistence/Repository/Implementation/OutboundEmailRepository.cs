using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Business.Core.Messaging.Common.Enums;
using Business.Core.Messaging.Common.Helpers;
using Business.Core.Messaging.Config;
using Business.Core.Messaging.Entities;
using Business.Core.Messaging.Entities.Proxies;
using Business.Core.Messaging.Persistence.Context;

namespace Business.Core.Messaging.Persistence.Repository.Implementation
{
    internal class OutboundEmailRepository : Repository<OutboundEmail, BusinessMessagingContext>, IOutboundEmailRepository
    {
        public OutboundEmailRepository(BusinessMessagingContext context) : base(context)
        {
        }

        public Task<List<OutboundSmtpProxy>> GetOutboundEmailDetailsAsync()
        {
            var smtp = SmtpConfig.Get().SmtpNode;

            var maxAttempts = smtp.MaxRetry;

            var details = from email in Context.OutboundEmails
                                               .Where(CanSend().And(AttemptsNotExceeded(maxAttempts)))
                                               .Include(e => e.OutboundAttachments)
                                               .Include(e => e.OutboundRecipients)
                                               .Include(e => e.OutboundImages)

                          join config in Context.OutboundSmtpDetails on email.DescriptorId equals config.DescriptorId into configs
                          from config in configs.DefaultIfEmpty()

                          select new OutboundSmtpProxy
                          {
                              DescriptorId = email.DescriptorId,

                              Sender = email.Sender,
                              Subject = email.Subject,
                              Body = email.Body,
                              OutboundEmail = email,
                              OutboundAttachments = email.OutboundAttachments,
                              OutboundImages = email.OutboundImages,
                              OutboundRecipients = email.OutboundRecipients,
                              IsHtml = email.IsHtml,

                              UserName = config == null ? smtp.UserName : config.UserName,
                              Password = config == null ? smtp.Password : config.Password,
                              UseSsl = config == null ? smtp.UseSsl : config.UseSsl,
                              OutboundHost = config == null ? smtp.Host : config.Host,
                              OutboundPort = config == null ? smtp.Port : config.Port,
                          };

            return details.ToListAsync();
        }

        #region Query Helpers

        private static Expression<Func<OutboundEmail, bool>> CanSend()
        {
            return email => email.EmailStatus != EmailStatus.Sent;
        }

        private static Expression<Func<OutboundEmail, bool>> AttemptsNotExceeded(int maxAttempts)
        {
            return email => email.Attempts < maxAttempts;
        }


        #endregion
    }
}
