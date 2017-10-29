
using System.Collections.Generic;

namespace MiniAbp.Domain.Entities
{
    /// <summary>
    /// Paged list interface
    /// </summary>
    public interface IPagedList<T> 
    {
        List<T> Model { get; set; }
        int CurrentPage { get; }
        int PageSize { get; }
        int TotalCount { get; } 
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}
