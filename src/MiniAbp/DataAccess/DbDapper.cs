using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MiniAbp.Configuration;
using MiniAbp.DataAccess.Dapper; 
using MiniAbp.Dependency;
using MiniAbp.Domain.Entitys;
using MiniAbp.Runtime;

namespace MiniAbp.DataAccess
{
    public class DbDapper
    { 
        public static string ConnectionString => AppPath.ConvertFormatConnection(IocManager.Instance.Resolve<DatabaseSetting>().ConnectionString);
        public static Dialect Dialect => IocManager.Instance.Resolve<DatabaseSetting>().Dialect;


        //private static string ConnStr
        //{
        //    get
        //    {
        //        var cs = ConnectionString;
        //        var scsb = new SqlConnectionStringBuilder(cs)
        //        {
        //            MultipleActiveResultSets = true
        //        };
        //        return scsb.ConnectionString;
        //    }
        //}
        public static IDbConnection NewDbConnection => IocManager.Instance.ResolveNamed<IDbConnection>(Dialect.ToString()
            ,new {ConnectionString });

        public static DataTable RunDataTableSql(string sql, IDbConnection dbConnection = null)
        {
            DataTable table = new DataTable("MyTable");
            if (dbConnection != null)
            {
                dbConnection.ExecuteReader(sql);
            }
            else
            {
                using (var db = NewDbConnection)
                {
                    db.Open();
                    using (var reader = db.ExecuteReader(sql))
                    {
                        table.Load(reader);
                    }
                    db.Close();
                }
            }
            return table;
        }

        //
        //        public static DataTable RunDataTableSql(string sql, params SqlParameter[] sqlParas)
        //        {
        //            return DBHelper.RunDataTableSQL(sql, sqlParas);
        //        }
        public static IEnumerable<T> GetAll<T>(string sql, params SqlParameter[] sqlParas)
        {

            IEnumerable<T> dy = null;
            using (IDbConnection db = NewDbConnection)
            {
                db.Open();
                dy = db.Query<T>(sql, sqlParas);
                db.Close();
            }
            return dy;
        }
        public static IEnumerable<T> GetAll<T>(string sql)
        {
            IEnumerable<T> dy = null;
            using (IDbConnection db = NewDbConnection)
            {
                db.Open();
                dy = db.Query<T>(sql);
                db.Close();
            }
            return dy;
        }

        public static int Count<T>(string condition, IDbConnection connection = null, IDbTransaction transation = null)
        {
            int count;
            if (connection != null)
            {
                count = connection.Count<T>(condition, transation);
            }
            else
            {
                using (var db = NewDbConnection)
                {
                    count = db.Count<T>(condition);
                }
            }
            return count;
        }

        public static int Count(string sql, object param, IDbConnection connection = null,
            IDbTransaction transation = null)
        {
            int count;
            if (connection != null)
            {
                count = connection.Count(sql, param, transation);
            }
            else
            {
                using (var db = NewDbConnection)
                {
                    count = db.Count(sql, param);
                }
            }
            return count;
        }

        public static bool Any<T>(object param, IDbConnection connection = null, IDbTransaction transation = null)
        {
            int count;
            if (connection != null)
            {
                count = connection.Count<T>(param, transation);
            }
            else
            {
                using (var db = NewDbConnection)
                {
                    count = db.Count<T>(param);
                }
            }
            return count > 0;
        }
        public static bool Any<T>(string condition, IDbConnection connection = null, IDbTransaction transation = null)
        {
            var count = Count<T>(condition, connection, transation);
            return count > 0;
        }

        public static T Get<T>(object id, IDbConnection connection = null, IDbTransaction transation = null)
        {
            T dy;
            if (connection != null)
            {
                dy = connection.Get<T>(id, transation);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    dy = db.Get<T>(id);
                    db.Close();
                }
            }
            return dy;
        }


//        public static T Get<T>(string sql, params SqlParameter[] sqlParas)
//        {
//
//            IEnumerable<T> dy = null;
//            using (IDbConnection db = NewDbConnection)
//            {
//                db.Open();
//                dy = db.Query<T>(sql, sqlParas);
//                db.Close();
//            }
//            return dy.FirstOrDefault();
//        }

        //执行删除和更新操作 无需返回值
        public static void ExecuteNonQuery(string sql, object param =null, IDbConnection connection = null, IDbTransaction transation = null)
        {
            if (connection != null)
            {
                connection.Execute(sql, param, transation);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    db.Execute(sql, param);
                    db.Close();
                }
            }
        }
        //执行删除和更新操作 无需返回值
        //        public static int ExecuteNonQuery(string sql, params SqlParameter[] para)
        //        {
        //            return DBHelper.RunInsertOrUpdateOrDeleteSQL(sql, para);
        //        }


        public static T GetSingle<T>(string id, IDbConnection connection = null, IDbTransaction transation = null)
        {
            T dy;
            if (connection != null && transation != null)
            {
                dy = connection.Get<T>(id, transation);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    dy = db.Get<T>(id);
                    db.Close();
                }
            }
            return dy;
        }

        public static List<T> GetList<T>(string where, IDbConnection connection = null, IDbTransaction transation = null)
        {
            Func<IDbConnection,string, IEnumerable<T>> action = (dbConnection, w) =>  
            string.IsNullOrWhiteSpace(w) ? 
            dbConnection.GetList<T>(null, transation) : 
            dbConnection.GetList<T>(w, transation); ;
            IEnumerable<T> result;

            if (connection != null && transation != null)
            {
                result = action(connection, where);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    result = action(db, where);
                    db.Close();
                }
            }

            return result.ToList();
        }
        public static T First<T>(string where, IDbConnection connection = null, IDbTransaction transation = null)
        {
            Func<IDbConnection,string, T> action = (dbConnection, w) =>  
            string.IsNullOrWhiteSpace(w) ? dbConnection.First<T>(null, transation) : dbConnection.First<T>(w, transation); ;
            T result;

            if (connection != null && transation != null)
            {
                result = action(connection, where);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    result = action(db, where);
                    db.Close();
                }
            }

            return result;
        }
        public static PagedList<T> GetPagedList<T>(IPaging pageinput, string where, IDbConnection connection = null, IDbTransaction transation = null)
        {
            Func<IDbConnection, string, PagedList<T>> action =(dbConnection, w) =>
                    dbConnection.GetListPaged<T>( where, pageinput, transation);

            PagedList<T> result;
            if (connection != null && transation != null)
            {
                result = action(connection, where);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    result = action(db, where);
                    db.Close();
                }
            }

            return result;
        }

        public static List<T> Query<T>(string sql, object param = null, IDbConnection dbConnection = null, IDbTransaction tran = null)
        {
            IEnumerable<T> result;
            if (dbConnection != null)
            {
                result = dbConnection.Query<T>(sql, param, tran);
            }
            else
            {
                using (var db = NewDbConnection)
                {
                    result = db.Query<T>(sql, param);
                }
            }
            return result.ToList();
        } 
        public static PagedList<T> Query<T>(string sql, IPaging input, object param = null, IDbConnection dbConnection = null, IDbTransaction tran = null)
        {
            PagedList<T> result;
            if (dbConnection != null)
            {
                result = dbConnection.Query<T>(sql, input, param, tran);
            }
            else
            {
                using (var db = NewDbConnection)
                {
                    result = db.Query<T>(sql, input, param);
                }
            }
            return result;
        } 
        public static List<T> GetList<T>(object whereConditions, IDbConnection connection = null, IDbTransaction transation = null)
        {

            IEnumerable<T> result;
            if (connection != null && transation != null)
            {
                result = connection.GetList<T>(whereConditions, transation);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    result = db.GetList<T>(whereConditions);
                    db.Close();
                }
            }
            return result.ToList();
        }

        public static T First<T>(object whereConditions, IDbConnection connection = null, IDbTransaction transation = null)
        {

            T result;
            if (connection != null && transation != null)
            {
                result = connection.FirstOrDefault<T>(whereConditions, transation);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    result = db.FirstOrDefault<T>(whereConditions);
                    db.Close();
                }
            }
            return result;
        }

        public static int Delete<T>(T whereConditions, IDbConnection connection = null, IDbTransaction transation = null)
        {

            int result;
            if (connection != null && transation != null)
            {
                result = connection.Delete<T>(whereConditions, transation);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    result = db.Delete<T>(whereConditions);
                    db.Close();
                }
            }
            return result;
        }
        public static int Delete<T>(object id, IDbConnection connection = null, IDbTransaction transation = null)
        {

            int result;
            if (connection != null && transation != null)
            {
                result = connection.Delete<T>(id, transation);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    result = db.Delete<T>(id);
                    db.Close();
                }
            }
            return result;
        }
        public static int Update<T>(T entity, IDbConnection connection = null, IDbTransaction transation = null)
        {

            int result;
            if (connection != null && transation != null)
            {
                result = connection.Update(entity, transation);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    result = db.Update(entity);
                    db.Close();
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">返回类型通常为int</typeparam>
        /// <param name="entity"></param>
        /// <param name="connection"></param>
        /// <param name="transation"></param>
        public static string Insert<T>(T entity, IDbConnection connection = null, IDbTransaction transation = null)
        {

            string result;
            if (connection != null && transation != null)
            {
                result = connection.Insert<string>(entity, transation);
            }
            else
            {
                using (IDbConnection db = NewDbConnection)
                {
                    db.Open();
                    result = db.Insert<string>(entity);
                    db.Close();
                }
            }
            return result;
        }

    }
}
