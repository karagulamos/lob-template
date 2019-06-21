using System;
using System.Threading.Tasks;
using Business.Core.Common.Exceptions;

namespace Business.Core.Services
{
    public interface IServiceHelper : IServiceDependencyMarker
    {
        Task<BusinessGenericException> GetExceptionAsync(string errorCode);
        T GetOrUpdateCacheItem<T>(string key, Func<T> update, TimeSpan idle = default(TimeSpan));
        void RemoveCachedItem(string key);
    }
}