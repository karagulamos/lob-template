using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Business.Core.Common.Helpers;

namespace Business.Persistence.Repository.Common.Extensions
{
    public static class PagedListExtensions
    {
        public static async Task<PagedList<TEntity>> ToPagedListAsync<TEntity>(this IQueryable<TEntity> entities, int page, int size) where TEntity : class
        {
            var count = await entities.CountAsync();

            if (entities.GetType() != typeof(IOrderedQueryable<>))
            {
                entities = entities.OrderByDescending(e => true);
            }

            return new PagedList<TEntity>(page, size)
            {
                Items = entities.AsNoTracking(),
                Count = count,
                Page = page,
                Size = size == 0 ? count : size
            };
        }
    }
}
