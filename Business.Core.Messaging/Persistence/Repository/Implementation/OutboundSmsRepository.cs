using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Business.Core.Messaging.Common.Helpers;
using Business.Core.Messaging.Entities;
using Business.Core.Messaging.Persistence.Context;

namespace Business.Core.Messaging.Persistence.Repository.Implementation
{
    internal class OutboundSmsRepository : Repository<OutboundSms, BusinessMessagingContext>, IOutboundSmsRepository
    {
        public OutboundSmsRepository(BusinessMessagingContext context) : base(context)
        {
        }

        public Task<List<OutboundSms>> GetUnprocessedSmsesAsync()
        {
            var maxAttempts = Convert.ToInt32(ConfigurationManager.AppSettings["MaxSmsAttempts"]);

            Context.Configuration.ProxyCreationEnabled = false;
            Context.Configuration.LazyLoadingEnabled = false;
            return Context.OutboundSmses.Where(CanSend().And(AttemptsNotExceeded(maxAttempts))).Include(s => s.SmsRecipients).ToListAsync();
        }

        #region Query Helpers

        private static Expression<Func<OutboundSms, bool>> CanSend()
        {
            return e => e.Sent == 0;
        }

        private static Expression<Func<OutboundSms, bool>> AttemptsNotExceeded(int maxAttempts)
        {
            return email => email.Attempts < maxAttempts;
        }
        #endregion
    }
}
