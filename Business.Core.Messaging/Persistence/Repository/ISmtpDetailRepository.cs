using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Business.Core.Messaging.Entities;

namespace Business.Core.Messaging.Persistence.Repository
{
    internal interface ISmtpDetailRepository : IRepository<OutboundSmtp>
    {
        IEnumerable<T> GetImapSettings<T>(Expression<Func<OutboundSmtp, T>> action);
    }
}