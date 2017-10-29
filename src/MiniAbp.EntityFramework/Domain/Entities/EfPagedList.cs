using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniAbp.Domain.Entities
{
    /// <summary>
    /// Paged list
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    [Serializable]
    public class EfPagedList<T> : IPagedList<T>
    {
        public List<T> Model { get; set; }

        public EfPagedList() {
            this.Model = new List<T>();
        }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public EfPagedList(IQueryable<T> source, int pageIndex, int pageSize): this()
        {
            int total = source.Count();
            this.TotalCount = total;
            this.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.CurrentPage = pageIndex;
            // 如果当前页
            //var takeCount = ((this.CurrentPage - 1) * pageSize + pageSize) > total ? total % pageSize : pageSize;
            Model.AddRange(source.Skip((pageIndex -1) * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public EfPagedList(IList<T> source, int pageIndex, int pageSize) : this()
        {
            TotalCount = source.Count();
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.CurrentPage = pageIndex;
            //var takeCount = ((this.CurrentPage - 1) * pageSize + pageSize) < TotalCount ? TotalCount % pageSize : pageSize;
            Model.AddRange(source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total count</param>
        public EfPagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount, int totalKiaCount = 0) : this()
        {
            TotalCount = totalCount; 
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.CurrentPage = pageIndex;
            Model.AddRange(source);
        }

        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; } 
        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        {
            get { return (CurrentPage > 0); }
        }
        public bool HasNextPage
        {
            get { return (CurrentPage + 1 < TotalPages); }
        }

    }
}
