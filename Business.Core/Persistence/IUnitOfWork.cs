using System;
using System.Threading.Tasks;
using Business.Core.Persistence.Repository;

namespace Business.Core.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IErrorCodeRepository ErrorCodes { get; set; }

        Task CompleteAsync();
    }
}