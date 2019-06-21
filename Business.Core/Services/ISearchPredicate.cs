using System;
using System.Linq.Expressions;

namespace Business.Core.Services
{
    public interface ISearchPredicate<TEntity, in TQuery> : IServiceDependencyMarker
    {
        Expression<Func<TEntity, bool>> Apply(TQuery query);
    }
}