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
    public abstract class BaseRepository : IRepository 
    {
        protected YSession Session = YSession.GetInstance();
        protected virtual IDbConnection Connection { get; set; }
        protected virtual IDbTransaction Transaction { get; set; }

        public virtual List<TModel> Query<TModel>(string sql, object param = null)
        {
            return DbDapper.Query<TModel>(sql, param, Connection, Transaction);
        }
        public virtual void QueryMultiple( Action<SqlMapper.GridReader> func, string sql, object param = null)
        {
            using (var multi = Connection.QueryMultiple(sql, param))
            {
                func(multi);
            }
        }
        public virtual PagedList<TModel> Query<TModel>(string sql, IPaging input, object param = null)
        {
            return DbDapper.Query<TModel>(sql,input, param, Connection, Transaction);
        }
        public virtual TModel QueryFirst<TModel>(string sql, object param = null)
        {
            return DbDapper.Query<TModel>(sql, param, Connection, Transaction).FirstOrDefault();
        }
        public virtual void Execute(string sql, object param = null)
        {
            DbDapper.ExecuteNonQuery(sql, param, Connection, Transaction);
        }

    }
}