using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Business.Core.Common.Helpers;

namespace Business.Persistence.Repository
{
    public abstract class Repository<TEntity, TContext>
    where TEntity : class
    where TContext : DbContext, new()
    {
        protected TContext Context;

        private readonly DbSet<TEntity> _entitySet;

        protected Repository(TContext context)
        {
            Context = context;
            _entitySet = context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            _entitySet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _entitySet.AddRange(entities);
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _entitySet.Where(predicate).AsNoTracking();
        }

        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entitySet.Where(predicate).ToListAsync();
        }

        public Task<PagedList<TEntity>> GetPagedAsync(int page, int size)
        {
            return GetPagedAsync(item => true, page, size);
        }

        public async Task<PagedList<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int size)
        {
            var count = await GetDataCountAsync(predicate);

            if (size <= 0)
                size = count;

            var items = _entitySet.Where(predicate)
                                  .AsNoTracking();

            return new PagedList<TEntity>(page, size)
            {
                Items = items,
                Count = count
            };
        }

        public Task<int> GetDataCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entitySet.CountAsync(predicate);
        }

        public Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entitySet.AnyAsync(predicate);
        }

        public TEntity Get(int entityId)
        {
            return _entitySet.Find(entityId);
        }

        public Task<TEntity> GetAsync(int entityId)
        {
            return _entitySet.FindAsync(entityId);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entitySet.SingleOrDefaultAsync(predicate);
        }

        public void Remove(TEntity entity)
        {
            _entitySet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _entitySet.RemoveRange(entities);
        }
    }
}
