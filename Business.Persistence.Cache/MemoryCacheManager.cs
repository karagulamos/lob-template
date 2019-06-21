using System;
using System.Runtime.Caching;
using Business.Core.Persistence.Cache;

namespace Business.Persistence.Cache
{
    public class MemoryCacheManager : ICacheManager
    {
        private readonly MemoryCache _memCache = MemoryCache.Default;
        public void Add(string key, object value, TimeSpan idle = new TimeSpan())
        {
            var cachingPolicy = new CacheItemPolicy
            {
                SlidingExpiration = idle == default(TimeSpan) ? TimeSpan.FromHours(12) : idle
            };

            _memCache.Add(new CacheItem(key, value), cachingPolicy);
        }

        public object Get(string key)
        {
            return _memCache.Get(key);
        }

        public void Remove(string key)
        {
            _memCache.Remove(key);
        }

        public bool Contains(string key)
        {
            return _memCache.Contains(key);
        }
    }
}
