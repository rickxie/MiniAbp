using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Castle.Core.Internal;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Dapper;
using MiniAbp.DataAccess;
using MiniAbp.Domain.Entitys;
using MiniAbp.Extension;
using MiniAbp.Runtime;

namespace MiniAbp.Domain
{
    public abstract class BaseRepository<T, TPrimaryKey> : BaseRepository, IRepository where T : IEntity<TPrimaryKey> 
    {
        public virtual PagedList<T> GetPagedList(IPaging pageInput, string where = null)
        {
            return DbDapper.GetPagedList<T>(pageInput, where, Connection, Transaction);
        }

        public virtual List<T> GetAll(object where = null)
        {
            return DbDapper.GetList<T>(where, Connection, Transaction);
        }
        public virtual T First(object where = null)
        {
            return DbDapper.First<T>(where, Connection, Transaction);
        }
        
        public virtual T First(string where = null)
        {
            return DbDapper.First<T>(where, Connection, Transaction);
        }
        
        public virtual bool Any(object whereCondition)
        {
            return DbDapper.Any<T>(whereCondition, Connection, Transaction);
        }
        public virtual bool Any(string where)
        {
            return DbDapper.Any<T>(where, Connection, Transaction);
        }
        public virtual int Count(string where)
        {
            return DbDapper.Count<T>(where, Connection, Transaction);
        }
        public virtual int Count(string sql, object param)
        {
            return DbDapper.Count(sql, param, Connection, Transaction);
        }

        public virtual T Get(string id)
        {
            return DbDapper.Get<T>(id, Connection, Transaction);
        }


        public virtual int Delete(string id)
        {
            return DbDapper.Delete<T>(id, Connection, Transaction);
        }

        public virtual int Delete(T entity)
        {
            return DbDapper.Delete<T>(entity, Connection, Transaction);
        }

        public virtual int SoftDelete<TModel>(TModel entity) where TModel : CreationAndDeletionEntity
        {
            entity.MarkDeleted();
            return DbDapper.Update(entity, Connection, Transaction);
        }


        public virtual void Insert(T model)
        {
            var creationTime = model.GetType().GetProperty("CreationTime");
            creationTime?.SetValue(model, DateTime.Now);
            DbDapper.Insert<T>(model, Connection, Transaction);
        }

        /// <summary>
        /// if id exists then update, otherwise add.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dbCheck">If has Id, do the Db check for existence</param>
        public virtual void AddOrUpdate(T model, bool dbCheck = false)
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
                    if (typeof (Entity).IsAssignableFrom(typeof (T)))
                    {
                        var a = model as Entity;
                        a?.RefreshId();
                    }else if (typeof (CreationEntity).IsAssignableFrom(typeof (T)))
                    {
                        var a = model as CreationEntity;
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

        public virtual int Update(T cate)
        {
            return DbDapper.Update(cate, Connection, Transaction);
        }
    }
}