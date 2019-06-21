using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Core.Messaging.Entities;
using Business.Core.Messaging.Entities.Proxies;

namespace Business.Core.Messaging.Persistence.Repository
{
    internal interface IOutboundEmailRepository : IRepository<OutboundEmail>
    {
        Task<List<OutboundSmtpProxy>> GetOutboundEmailDetailsAsync();
    }
}
