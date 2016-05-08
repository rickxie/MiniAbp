using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.DataAccess;
using MiniAbp.Domain;

namespace MiniAbp.Ado.Uow
{
    public class AdoDbContext : IDbContext
    {
        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; } 

        public AdoDbContext()
        {
            
        }
        public AdoDbContext(IDbConnection dbConnection)
        {
            this.DbConnection = dbConnection;
        }
    }
}
