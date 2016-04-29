using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Domain.Entitys
{
    public interface IPaging
    {
        int CurrentPage { get; set; }
        int PageSize { get; set; }
        string OrderByProperty { get; set; }
        bool Ascending { get; set; }
    }
}
