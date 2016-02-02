using System.Collections.Generic;
using System.Data;
using MiniAbp.DataAccess;
using MiniAbp.Runtime;
using Yooya.Bpm.Framework.Domain.Entity;

namespace MiniAbp.Domain
{
    public class BaseRepository<T>
    {
        protected YSession Session = YSession.GetInstance();
        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; }

        public List<T> GetPagedList(PageInput pageInput, string where = null)
        {
            return DbDapper.GetPagedList<T>(pageInput, where, DbConnection, DbTransaction);
        }

        public List<T> GetAll(string where = null)
        {
            return DbDapper.GetList<T>(where, DbConnection, DbTransaction);
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


        public void Create(T cate)
        {
            DbDapper.Insert<T>(cate, DbConnection, DbTransaction);
        }

        public int Update(T cate)
        {
            return DbDapper.Update(cate, DbConnection, DbTransaction);
        }

    }
}