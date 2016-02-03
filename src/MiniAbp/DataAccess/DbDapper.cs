using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Autofac;
using Dapper;
using MiniAbp.DataAccess.Dapper;
using MiniAbp.Dependency;
using MiniAbp.Runtime;
using Yooya.Bpm.Framework.Domain.Entity;

namespace MiniAbp.DataAccess
{
    public class DbDapper
    {
        private static string _connStr;
        public static string ConnectionString {
            get { return _connStr; } 
            set
            {
                _connStr = AppPath.ConvertFormatConnection(value);
            }
        } 
        //public static DBHelper DbHelper;
        public static DatabaseType DatabaseType;
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
        public static IDbConnection NewDbConnection => IocManager.Instance.ResolveNamed<IDbConnection>(DatabaseType.ToString()
            ,new TypedParameter(typeof (string), ConnectionString));

        public static DataTable RunDataTableSql(string sql)
        {
            DataTable table = new DataTable("MyTable");
            using (var db = NewDbConnection)
            {
                db.Open();
                using (var reader = db.ExecuteReader(sql))
                {
                    table.Load(reader);
                }
                db.Close();
            }
            return table;
        }

//
//        public static DataTable RunDataTableSql(string sql, params SqlParameter[] sqlParas)
//        {
//            return DBHelper.RunDataTableSQL(sql, sqlParas);
//        }

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
        public static T Get<T>(string sql, IDbConnection connection = null, IDbTransaction transation = null)
        {

            IEnumerable<T> dy = null;
            using (IDbConnection db = NewDbConnection)
            {
                db.Open();
                dy = db.Query<T>(sql);
                db.Close();
            }
            return dy.FirstOrDefault();
        }

        public static T Get<T>(string sql, params SqlParameter[] sqlParas)
        {

            IEnumerable<T> dy = null;
            using (IDbConnection db = NewDbConnection)
            {
                db.Open();
                dy = db.Query<T>(sql, sqlParas);
                db.Close();
            }
            return dy.FirstOrDefault();
        }

        //执行删除和更新操作 无需返回值
        public static void ExecuteNonQuery(string sql)
        {
            using (IDbConnection db = NewDbConnection)
            {
                db.Open();
                db.Execute(sql);
                db.Close();
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
            dbConnection.GetList<T>() : 
            dbConnection.GetList<T>(w); ;
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
        public static List<T> GetPagedList<T>(PageInput pageinput, string where, IDbConnection connection = null, IDbTransaction transation = null)
        {
            Func<IDbConnection, string, IEnumerable<T>> action =(dbConnection, w) =>
                    dbConnection.GetListPaged<T>(pageinput.CurrentPage, pageinput.PageSize, where,
                        pageinput.OrderByProperty + " " + (pageinput.Ascending ? "asc" : "desc"), transation);

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
