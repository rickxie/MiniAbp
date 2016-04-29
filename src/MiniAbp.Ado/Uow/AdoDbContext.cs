using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.DataAccess;

namespace MiniAbp.Ado.Uow
{
    public class AdoDbContext : IDbContext
    {
        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; }
        public string NameOrConnectionString;
        public Dialect Dialect;

        public AdoDbContext()
        {
            
        }
        public AdoDbContext(string connectionString, Dialect dialect)
        {
            this.NameOrConnectionString = connectionString;
            this.Dialect = dialect;
        }
    }
}
