using System;

namespace Business.Core.Persistence.Cache
{
    public interface ICacheManager
    {
        void Add(string key, object value, TimeSpan idle = default(TimeSpan));
        object Get(string key);
        void Remove(string key);
        bool Contains(string key);
    }
}