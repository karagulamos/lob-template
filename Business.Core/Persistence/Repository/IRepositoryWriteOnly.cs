using System.Collections.Generic;

namespace Business.Core.Persistence.Repository
{
    public interface IRepositoryWriteOnly<in TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
