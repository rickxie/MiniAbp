using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.DataAccess;

namespace MiniAbp.Domain.Uow
{
    public class UnitOfWorkOptions :IUnitOfWorkDefaultOptions
    {
        public bool IsTransactional { get; set; }
        public TimeSpan? Timeout { get; set; } 
    }
}
