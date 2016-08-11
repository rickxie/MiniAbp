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
    public abstract class BaseRepository : ApplicationCommonBase, IRepository
    {

        public virtual List<TModel> Query<TModel>(string sql, object param = null)
        {
            return DbDapper.Query<TModel>(sql, param, DbConnection, DbTransaction);
        }
        public virtual void QueryMultiple( Action<SqlMapper.GridReader> func, string sql, object param = null)
        {
            using (var multi = DbConnection.QueryMultiple(sql, param))
            {
                func(multi);
            }
        }
        public virtual PagedList<TModel> Query<TModel>(string sql, IPaging input, object param = null)
        {
            return DbDapper.Query<TModel>(sql,input, param, DbConnection, DbTransaction);
        }
        public virtual TModel QueryFirst<TModel>(string sql, object param = null)
        {
            return DbDapper.Query<TModel>(sql, param, DbConnection, DbTransaction).FirstOrDefault();
        }
        public virtual void Execute(string sql, object param = null)
        {
            DbDapper.ExecuteNonQuery(sql, param, DbConnection, DbTransaction);
        }
        public virtual DataTable GetDataTable(string sql, object param = null)
        {
            return DbDapper.RunDataTableSql(sql, param, DbConnection, DbTransaction);
        }
        public virtual PagedDatatable GetDataTable(string sql, IPaging input, object param = null)
        {
            return DbDapper.RunDataTableSql(sql, input, param, DbConnection, DbTransaction);
        }
    }
}