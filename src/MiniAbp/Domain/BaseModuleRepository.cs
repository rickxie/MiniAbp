using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MiniAbp.DataAccess;
using MiniAbp.Domain.Entitys;
using MiniAbp.Runtime;

namespace MiniAbp.Domain
{
    public abstract class BaseModuleRepository : IRepository 
    {
        protected YSession Session = YSession.GetInstance();
        protected virtual IDbConnection Connection { get; set; }
        protected virtual IDbTransaction Transaction { get; set; }
        public virtual DataTable GetDataTable(string sql, object param = null)
        {
            return DbDapper.RunDataTableSql(sql, param, Connection, Transaction);
        }

        public virtual void Execute(string sql, object param = null)
        {
            DbDapper.ExecuteNonQuery(sql, param, Connection, Transaction);
        }

    }
}