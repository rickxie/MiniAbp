using System.Collections.Generic;

namespace MiniAbp.Domain.Entitys
{
    public class PagedList<T> 
    {
        public List<T> Model { get; set; }
        public int TotalCount { get; set; }
    }
}
