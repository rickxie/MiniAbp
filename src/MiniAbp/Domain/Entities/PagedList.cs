using System;
using System.Collections.Generic;

namespace MiniAbp.Domain.Entities
{
    public class PagedList<T> : IPagedList<T>
    {
        public List<T> Model { get; set; }
        public int TotalCount { get; set; }

        public int CurrentPage { get; }

        public int PageSize { get; }

        public int TotalPages { get; }

        public bool HasPreviousPage { get; }

        public bool HasNextPage { get; }
    }
}
