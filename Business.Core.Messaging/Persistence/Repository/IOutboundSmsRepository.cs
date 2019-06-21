using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Core.Messaging.Entities;

namespace Business.Core.Messaging.Persistence.Repository
{
    internal interface IOutboundSmsRepository : IRepository<OutboundSms>
    {
        Task<List<OutboundSms>> GetUnprocessedSmsesAsync();
    }
}