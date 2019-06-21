using System;
using System.Linq;
using System.Linq.Expressions;
using Business.Core.Common.Enums;
using Business.Core.Common.Extensions;
using Newtonsoft.Json;

namespace Business.Core.Common.Helpers
{
    public class PagedList<T>
    {
        private readonly int _page;
        private readonly int _size;

        public IQueryable<T> Items { get; set; }

        public int Count { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public string SearchTerm { get; set; }

        public PagedList(int page, int size)
        {
            _page = page <= 0 ? 1 : page;
            _size = size < 0 ? 0 : size;
        }

        [JsonIgnore]
        public IPagedQueryBuilder<T> QueryBuilder => new PagedSearchQueryBuilder(Items, this);

        public PagedList<TEntity> Project<TEntity>(Expression<Func<T, TEntity>> mapping)
        {
            return new PagedList<TEntity>(_page, _size)
            {
                Items = Items.Select(mapping),
                Count = Count,
                Page = _page,
                Size = _size == 0 ? Count : _size
            };
        }

        #region Query Builder
        internal sealed class PagedSearchQueryBuilder : IPagedQueryBuilder<T>
        {
            private IQueryable<T> _query;
            private readonly PagedList<T> _paged;
            private string _searchTerm;

            public PagedSearchQueryBuilder(IQueryable<T> query, PagedList<T> pagedList)
            {
                _query = query;
                _paged = pagedList;
            }

            public IPagedQueryBuilder<T> Search(string searchTerm)
            {
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    _query = (IOrderedQueryable<T>)_query.Search(searchTerm);
                    _searchTerm = searchTerm;
                }

                return this;
            }

            public IPagedQueryBuilder<T> Search(Expression<Func<T, bool>> predicate)
            {
                _query = (IOrderedQueryable<T>)_query.Where(predicate);
                return this;
            }

            public IPagedQueryBuilder<T> OrderByDescendending<TKey>(Expression<Func<T, TKey>> keySelector)
            {
                _query = _query.OrderByDescending(keySelector);
                return this;
            }

            public IPagedQueryBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
            {
                _query = _query.OrderBy(keySelector);
                return this;
            }

            public IPagedQueryBuilder<T> Order<TKey>(Expression<Func<T, TKey>> keySelector, string order = PageOrder.ASC)
            {
                _query = order == PageOrder.DESC
                               ? _query.OrderByDescending(keySelector)
                               : _query.OrderBy(keySelector);
                return this;
            }

            public PagedList<T> Execute()
            {
                var count = _query.Count();

                return new PagedList<T>(_paged._page, _paged._size)
                {
                    Count = count,
                    Items = _query.Skip((_paged._page - 1) * _paged._page)
                                  .Take(_paged._size == 0 ? count : _paged._size),
                    Page = _paged._page,
                    Size = _paged._size == 0 ? count : _paged._size,
                    SearchTerm = _searchTerm
                };
            }
        }
        #endregion
    }
}