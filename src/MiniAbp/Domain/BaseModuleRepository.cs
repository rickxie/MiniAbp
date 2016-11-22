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
        protected ISession Session = NullSession.GetInstance();
        protected virtual IDbConnection Connection { get; set; }
        protected virtual IDbTransaction Transaction { get; set; }

        /// <summary>
        /// 跑一个SQL  返回一个Datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param">匿名参数，@id 对应的传 new { id = varId } </param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string sql, object param = null)
        {
            return DbDapper.RunDataTableSql(sql, param, Connection, Transaction);
        }
        /// <summary>
        /// 跑一个SQL  返回一个Datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param">匿名参数，@id 对应的传 new { id = varId } </param>
        /// <returns></returns>
        public virtual DataTable RunSql(string sql, object param = null)
        {
            return DbDapper.RunDataTableSql(sql, param, Connection, Transaction);
        }

        public virtual int Execute(string sql, object param = null)
        {
            return DbDapper.ExecuteNonQuery(sql, param, Connection, Transaction);
        }

    }
}