using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Castle.Core.Internal;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using MiniAbp.DataAccess;
using MiniAbp.Domain.Entitys;
using MiniAbp.Extension;
using MiniAbp.Runtime;

namespace MiniAbp.Domain
{
    public class BaseRepository<T, TPrimaryKey> where T : IEntity<TPrimaryKey>
    {
        protected YSession Session = YSession.GetInstance();
        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; }

        public List<T> GetPagedList(PageInput pageInput, string where = null)
        {
            return DbDapper.GetPagedList<T>(pageInput, where, DbConnection, DbTransaction);
        }

        public List<T> GetAll(object where = null)
        {
            return DbDapper.GetList<T>(where, DbConnection, DbTransaction);
        }
        public T First(object where = null)
        {
            return DbDapper.First<T>(where, DbConnection, DbTransaction);
        }
        
        public T First(string where = null)
        {
            return DbDapper.First<T>(where, DbConnection, DbTransaction);
        }
        
        public bool Any(string where)
        {
            return DbDapper.Any<T>(where, DbConnection, DbTransaction);
        }
        public int Count(string where)
        {
            return DbDapper.Count<T>(where, DbConnection, DbTransaction);
        }

        public T Get(string id)
        {
            return DbDapper.Get<T>(id, DbConnection, DbTransaction);
        }


        public int Delete(string id)
        {
            return DbDapper.Delete<T>(id, DbConnection, DbTransaction);
        }

        public int Delete(T entity)
        {
            return DbDapper.Delete<T>(entity, DbConnection, DbTransaction);
        }


        public void Insert(T model)
        {
            var creationTime = model.GetType().GetProperty("CreationTime");
            creationTime?.SetValue(model, DateTime.Now);
            DbDapper.Insert<T>(model, DbConnection, DbTransaction);
        }

        /// <summary>
        /// if id exists then update, otherwise add.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dbCheck">If has Id, do the Db check for existence</param>
        public void AddOrUpdate(T model, bool dbCheck = false)
        {
            var isExists = false;
            //check isExist and refresh Id 
            if (model.Id != null)
            {
                var isDefaultValue = EqualityComparer<TPrimaryKey>.Default.Equals(model.Id, default(TPrimaryKey));
                if (model.Id is string)
                {
                    var idStr = model.Id as string;
                    
                    if (!isDefaultValue && dbCheck)
                    {
                        var entity = Get(idStr);
                        if (entity != null)
                        {
                            isExists = true;
                        }
                        else
                        {
                            
                            isExists = false;
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
                            isExists = false;
                            if (typeof(T).IsAssignableFrom(typeof(Entity)))
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
                }
                else
                {
                    throw new NotImplementedException("其他主键类型 没有实现此功能。");
                }
                
            }
            else
            {
                isExists = false;
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
        public int Update(T cate)
        {
            return DbDapper.Update(cate, DbConnection, DbTransaction);
        }
        public List<TModel> Query<TModel>(string sql, object param = null)
        {
            return DbDapper.Query<TModel>(sql, param, DbConnection, DbTransaction);
        }
        public TModel QueryFirst<TModel>(string sql, object param = null)
        {
            return DbDapper.Query<TModel>(sql, param, DbConnection, DbTransaction).FirstOrDefault();
        }
        public void Execute(string sql, object param = null)
        {
            DbDapper.ExecuteNonQuery(sql, param, DbConnection, DbTransaction);
        }

    }
}