using System;
using System.Threading.Tasks;
using Business.Core.Common.Exceptions;
using Business.Core.Persistence;
using Business.Core.Persistence.Cache;
using Business.Core.Services;

namespace Business.Services
{
    public class ServiceHelper : IServiceHelper
    {
        private readonly IUnitOfWork _uow;
        private readonly ICacheManager _cacheManager;

        public ServiceHelper(IUnitOfWork uow, ICacheManager cacheManager)
        {
            _uow = uow;
            _cacheManager = cacheManager;
        }

        public async Task<BusinessGenericException> GetExceptionAsync(string errorCode)
        {
            var error = await GetOrUpdateCacheItem(errorCode, async () => await _uow.ErrorCodes.GetErrorByCodeAsync(errorCode));

            if (error == null)
                throw new BusinessGenericException("Error validating your request. Please try again.", errorCode);

            return new BusinessGenericException(error.DisplayMessage, error.Code);
        }

        public T GetOrUpdateCacheItem<T>(string key, Func<T> update, TimeSpan idle = default(TimeSpan))
        {
            if (_cacheManager.Contains(key))
            {
                return (T)_cacheManager.Get(key);
            }

            var result = update.Invoke();

            if (result != null)
            {
                _cacheManager.Add(key, result, idle);
            }

            return result;
        }

        public void RemoveCachedItem(string key)
        {
            _cacheManager.Remove(key);
        }
    }
}
