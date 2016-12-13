using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Dapper;
using MiniAbp.Ado.Uow;
using MiniAbp.Configuration;
using MiniAbp.DataAccess;
using MiniAbp.DataAccess.Dapper;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Domain.Entitys;
using MiniAbp.Runtime;

namespace MiniAbp.Ado.Repository
{
    public class AdoRepositoryBase<TEntity, TPrimaryKey> : BaseRepository<TEntity, TPrimaryKey> where TEntity: class , IEntity<TPrimaryKey>
    {
        protected virtual  IDbContext Context => _dbContextProvider.DbContext;
        private readonly IDbContextProvider _dbContextProvider;

        protected override IDbConnection DbConnection {
            get
            {
                if (Context.DbConnection.State == ConnectionState.Closed)
                {
                    Context.DbConnection.Open();
                }
                return Context.DbConnection;
            }
        }
        protected override IDbTransaction DbTransaction => Context.DbTransaction;
        public AdoRepositoryBase(IDbContextProvider dbContextProvider)
        {
            this._dbContextProvider = dbContextProvider;
        }

        public override PagedList<TEntity> GetPagedList(IPaging pageInput, string where = null)
        {
            return DbConnection.GetListPaged<TEntity>(where, pageInput, DbTransaction);
        }

        public override List<TEntity> GetAll(object where = null)
        {
            return DbConnection.GetList<TEntity>(where, DbTransaction).ToList();
        }

        public override TEntity First(object whereCondition = null)
        {
            return DbConnection.FirstOrDefault<TEntity>(whereCondition, DbTransaction);
        }

        public override TEntity First(string where = null)
        {
           return DbConnection.First<TEntity>(where, DbTransaction); 
        }

        public override bool Any(object whereCondition)
        {
            return DbConnection.Count<TEntity>(whereCondition, DbTransaction) > 0;
        }

        public override bool Any(string where = "")
        {
            return DbConnection.Count<TEntity>(where,  DbTransaction) > 0; ;
        }

        public override int Count(string where = "")
        {
            return DbConnection.Count<TEntity>(where,  DbTransaction);
        }

        public override int Count(string sql, object param)
        {
            return DbConnection.Count(sql, param, DbTransaction);
        }

        public override TEntity Get(string id)
        {
            return DbConnection.Get<TEntity>(id,  DbTransaction);
        }

        public override int Delete(string id)
        {
            return DbConnection.Delete<TEntity>(id,  DbTransaction);
        }

        public override int Delete(TEntity entity)
        {
            return DbConnection.Delete(entity,  DbTransaction);
        }

        public override TEntity Insert(TEntity model)
        {
            DbConnection.Insert<string>(model,  DbTransaction);
            return model;
        }

        public override TEntity AddOrUpdate(TEntity model, bool dbCheck = false)
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
            return model;
        }

        public override TEntity Update(TEntity entity)
        {
            DbConnection.Update(entity, DbTransaction);
            return entity;
        }

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
