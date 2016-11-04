using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace MiniAbp.Extension
{
    public static class CollectionExtension
    {
        /// <summary>
        /// Checks whatever given collection object is null or has no item.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection.
        /// </summary>
        /// <param name="source">Collection</param>
        /// <param name="item">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            return FillDataTable<T>(list);
        }

        /// <summary>
        /// 根据实体类得到表结构
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        private static DataTable CreateData<T>(T model)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                //dataTable.Columns.Add(new DataColumn(propertyInfo.Name, typeof(string)));
                //2015-09-11 李典 之前所有数据转化为字符串类型，改为自适应数据类型
                //类型为泛型时做特殊处理
                try
                {
                    if (propertyInfo.PropertyType.IsGenericType)
                    {
                        if (propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType.GetGenericArguments()[0]));
                        }
                        else
                        {
                            dataTable.Columns.Add(new DataColumn(propertyInfo.Name, typeof(string)));
                        }
                    }
                    else
                    {
                        dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
                    }
                }
                catch (Exception)
                {
                    dataTable.Columns.Add(new DataColumn(propertyInfo.Name, typeof(string)));
                }
            }
            return dataTable;
        }
        /// <summary>
        /// Datatable 转换成 List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> To<T>(this DataTable dt) where T : new()
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                //T model = (T)Activator.CreateInstance(typeof(T));  
                T model = new T();
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue(model, dr[i], null);
                }

                modelList.Add(model);
            }
            return modelList;
        }

        /// <summary>  
        /// 填充对象：用DataRow填充实体类
        /// </summary>  
        public static T Convert<T>(DataRow dr) where T : new()
        {
            if (dr == null)
            {
                return default(T);
            }
            T model = new T();

            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                if (propertyInfo != null && dr[i] != DBNull.Value)
                    propertyInfo.SetValue(model, dr[i], null);
            }
            return model;
        }

        /// <summary>
        /// 实体类转换成DataTable
        /// 调用示例：DataTable dt= FillDataTable(Entitylist.ToList());
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        private static DataTable FillDataTable<T>(List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = CreateData(modelList[0]);//创建表结构

            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    object result = propertyInfo.GetValue(model, null);
                    if (result == null)
                    {
                        dataRow[propertyInfo.Name] = DBNull.Value;
                    }
                    else
                    {
                        dataRow[propertyInfo.Name] = result;
                    }
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }


        public static void Foreach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null || action == null)
                return;
            foreach (T obj in list)
                action(obj);
        }

        public static void Foreach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            if (list == null || action == null)
                return;
            int num = 0;
            foreach (T obj in list)
            {
                action(obj, num);
                ++num;
            }
        }

        public static void Foreach<T>(this IList<T> list, Action<T, bool> action)
        {
            if (list == null || action == null)
                return;
            int num = 0;
            foreach (T obj in (IEnumerable<T>)list)
            {
                action(obj, num >= list.Count - 1);
                ++num;
            }
        }
    }
}
