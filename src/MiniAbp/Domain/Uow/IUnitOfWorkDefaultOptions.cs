using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.DataAccess;

namespace MiniAbp.Domain.Uow
{
    public interface IUnitOfWorkDefaultOptions
    {
        bool IsTransactional { get; set; }
        TimeSpan? Timeout { get; set; } 
    }
}
