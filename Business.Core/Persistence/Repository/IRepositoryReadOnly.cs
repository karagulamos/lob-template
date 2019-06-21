using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Business.Core.Common.Helpers;

namespace Business.Core.Persistence.Repository
{
    public interface IRepositoryReadOnly<TEntity> where TEntity : class
    {
        TEntity Get(int entityId);
        Task<TEntity> GetAsync(int entityId);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<PagedList<TEntity>> GetPagedAsync(int page, int size);
        Task<PagedList<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int size);
        Task<int> GetDataCountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
