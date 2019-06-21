using System.Threading.Tasks;
using Business.Core.Entities;

namespace Business.Core.Persistence.Repository
{
    public interface IErrorCodeRepository : IRepository<ErrorCode>
    {
        Task<ErrorCode> GetErrorByCodeAsync(string errorCode);
    }
}
