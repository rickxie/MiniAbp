using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MiniAbp.DataAccess.Dapper;
using MiniAbp.Domain;
using MiniAbp.Domain.Entities;
using MiniAbp.Domain.Uow;

namespace MiniAbp.Ado.Repositories
{
    public class AdoRepositoryBase : BaseRepository
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public AdoRepositoryBase(IDbContextProvider dbContextProvider)
        {
            this._dbContextProvider = dbContextProvider;
        }
        protected virtual  IDbContext Context => _dbContextProvider.DbContext;
        private readonly IDbContextProvider _dbContextProvider;
        protected override IDbConnection DbConnection => Context.DbConnection;
        protected override IDbTransaction DbTransaction => Context.DbTransaction;
 

        public override List<TModel> Query<TModel>(string sql, object param = null)
        {
            return DbConnection.Query<TModel>(sql, param, DbTransaction).ToList();
        }

        public override PagedList<TModel> Query<TModel>(string sql, IPaging input, object param = null)
        {
            return DbConnection.Query<TModel>(sql, input, param, DbTransaction);
        }

        public override TModel QueryFirst<TModel>(string sql, object param = null)
        {
            return DbConnection.QueryFirst<TModel>(sql, param, DbTransaction);
        }

        public override void Execute(string sql, object param = null)
        {
            DbConnection.Execute(sql, param, DbTransaction);
        }
    }
}
