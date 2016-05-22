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
    public class AdoRepositoryBase<TEntity, TPrimaryKey> : BaseRepository<TEntity, TPrimaryKey> where TEntity: class , IEntity<TPrimaryKey>
    {
        protected virtual  IDbContext Context => _dbContextProvider.DbContext;
        private readonly IDbContextProvider _dbContextProvider;

        protected override IDbConnection Connection => Context.DbConnection;
        protected override IDbTransaction Transaction => Context.DbTransaction;
        public AdoRepositoryBase(IDbContextProvider dbContextProvider)
        {
            this._dbContextProvider = dbContextProvider;
        }

        public override PagedList<TEntity> GetPagedList(IPaging pageInput, string where = null)
        {
            return Connection.GetListPaged<TEntity>(where, pageInput, Transaction);
        }

        public override List<TEntity> GetAll(object where = null)
        {
            return Connection.GetList<TEntity>(where, Transaction).ToList();
        }

        public override TEntity First(object whereCondition = null)
        {
            return Connection.FirstOrDefault<TEntity>(whereCondition, Transaction);
        }

        public override TEntity First(string where = null)
        {
           return Connection.First<TEntity>(where, Transaction); 
        }

        public override bool Any(object whereCondition)
        {
            return Connection.Count<TEntity>(whereCondition, Transaction) > 0;
        }

        public override bool Any(string where)
        {
            return Connection.Count<TEntity>(where,  Transaction) > 0; ;
        }

        public override int Count(string where)
        {
            return Connection.Count<TEntity>(where,  Transaction);
        }

        public override int Count(string sql, object param)
        {
            return Connection.Count(sql, param, Transaction);
        }

        public override TEntity Get(string id)
        {
            return Connection.Get<TEntity>(id,  Transaction);
        }

        public override int Delete(string id)
        {
            return Connection.Delete<TEntity>(id,  Transaction);
        }

        public override int Delete(TEntity entity)
        {
            return Connection.Delete(entity,  Transaction);
        }

        public override void Insert(TEntity model)
        {
            Connection.Insert<string>(model,  Transaction);
        }

        public override void AddOrUpdate(TEntity model, bool dbCheck = false)
        {
            var isExists = false;
            //check isExist and refresh Id 
            var isDefaultValue = EqualityComparer<TPrimaryKey>.Default.Equals(model.Id, default(TPrimaryKey));
            var idStr = model.Id as string;
            if (!isDefaultValue && dbCheck)
            {
                var entity = Get(idStr);
                if (entity != null)
                {
                    isExists = true;
                }
            }
            else
            {
                if (!isDefaultValue)
                {
                    isExists = true;
                }
                else
                {
                    if (typeof (CreationEntity).IsAssignableFrom(typeof (TEntity)))
                    {
                        var a = model as CreationEntity;
                        a?.RefreshId();
                    }
                    else
                    if (typeof (Entity).IsAssignableFrom(typeof (TEntity)))
                    {
                        var a = model as Entity;
                        a?.RefreshId();
                    }
                    else
                    {
                        model.GetType().GetProperty("Id").SetValue(model, Guid.NewGuid().ToString());
                    }
                }
            }


            if (isExists)
            {
                Update(model);
            }
            else
            {
                Insert(model);
            }
        }

        public override int Update(TEntity entity)
        {
            return Connection.Update(entity, Transaction);
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
