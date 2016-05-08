using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Dapper;
using MiniAbp.Ado.Uow;
using MiniAbp.DataAccess;
using MiniAbp.DataAccess.Dapper;
using MiniAbp.Domain;
using MiniAbp.Domain.Entitys;

namespace MiniAbp.Ado.Repository
{
    public class AdoRepositoryBase : BaseRepository
    {
        protected virtual  IDbContext Context => _dbContextProvider.DbContext;
        private readonly IDbContextProvider _dbContextProvider;
        protected override IDbConnection Connection => Context.DbConnection;
        protected override IDbTransaction Transaction => Context.DbTransaction;
        public AdoRepositoryBase(IDbContextProvider dbContextProvider)
        {
            this._dbContextProvider = dbContextProvider;
        }

        public override List<TModel> Query<TModel>(string sql, object param = null)
        {
            return Connection.Query<TModel>(sql, param, Transaction).ToList();
        }

        public override PagedList<TModel> Query<TModel>(string sql, IPaging input, object param = null)
        {
            return Connection.Query<TModel>(sql, input, param, Transaction);
        }

        public override TModel QueryFirst<TModel>(string sql, object param = null)
        {
            return Connection.QueryFirst<TModel>(sql, param, Transaction);
        }

        public override void Execute(string sql, object param = null)
        {
            Connection.Execute(sql, param, Transaction);
        }
    }
}
