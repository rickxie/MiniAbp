using System.Collections.Generic;
using System.Data;

namespace MiniAbp.Domain.Entities
{
    public class PagedDatatable 
    {
        public DataTable Data { get; set; }
        public int TotalCount { get; set; }
    }
}
