using System.Data.Entity;
using System.Threading.Tasks;
using Business.Core.Entities;
using Business.Core.Persistence.Repository;

namespace Business.Persistence.Repository.Implementation
{
    public class ErrorCodeRepository : Repository<ErrorCode, BusinessDataContext>, IErrorCodeRepository
    {
        public ErrorCodeRepository(BusinessDataContext context) : base(context)
        {
        }

        public Task<ErrorCode> GetErrorByCodeAsync(string errorCode)
        {
            return Context.ErrorCodes.FirstOrDefaultAsync(e => e.Code.ToLower() == errorCode.ToLower());
        }
    }
}
