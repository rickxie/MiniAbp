using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.DataAccess;

namespace MiniAbp.Ado.Uow
{
    public interface IDbContext
    {
        IDbConnection DbConnection { get; set; }
        IDbTransaction DbTransaction { get; set; } 
    }
}
