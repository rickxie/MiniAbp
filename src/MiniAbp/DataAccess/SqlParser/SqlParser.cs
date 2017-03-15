using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiniAbp.DataAccess.SqlParser
{
    /// <summary>
    /// SQL 解析器
    /// </summary>
    public class SqlParser
    {
        public string Sql { get; set; } = string.Empty;
        private int FromIndex { get; set; }
        private int WhereIndex { get; set; }
        private int OrderIndex { get; set; }
        private int GroupIndex { get; set; }
        public bool HasOrderBy => OrderIndex > -1;
        public string Select { get; set; } = "";
        public string From { get; set; } = "";
        public string Where { get; set; } = "";
        public string GroupBy { get; set; } = "";
        public string OrderBy { get; set; } = "";
        //WHERE 中的每个项目
        private List<string> SelectQuery { get; set; } = new List<string>();
        public List<string> SelectItems { get; set; } = new List<string>();
        //OrderBy 中的每个项目
        private List<string> OrderbyQuery { get; set; } = new List<string>();
        public List<string> OrderbyItems { get; set; } = new List<string>();
        public SqlParser(string sql)
        {
            Sql = sql;
            AnalyseSql(Sql);
            //获取查询列名
            GetSelectColumnSql(Select);
            //获取查询列名
            GetSelectColumnNames();
            //获取查询列名
            GetOrderbyColumnSql(OrderBy);
            //获取排序列名
            GetOrderByColumnNames();
        }
        /// <summary>
        /// 获得所有Select的列
        /// </summary>
        private void GetSelectColumnNames()
        {
            foreach (var colQuery in SelectQuery)
            {
                var colName = string.Empty;
                if (!string.IsNullOrWhiteSpace(colQuery))
                {
                    var sql = colQuery.Trim();
                    //单纯的 xxxx.xxxx 没有 As 没有 =
                    colName = GetMatchedSelectColumn(sql);
                }
                if (!string.IsNullOrWhiteSpace(colName))
                {
                    SelectItems.Add(colName);
                }
            }
        }
        /// <summary>
        /// 获得所有Select的列
        /// </summary>
        private void GetOrderByColumnNames()
        {
            foreach (var colQuery in OrderbyQuery)
            {
                var colName = string.Empty;
                if (!string.IsNullOrWhiteSpace(colQuery))
                {
                    var sql = colQuery.Trim();
                    //单纯的 xxxx.xxxx 没有 As 没有 =
                    colName = GetMatchedOrderbyColumn(sql);
                }
                if (!string.IsNullOrWhiteSpace(colName))
                {
                    OrderbyItems.Add(colName);
                }
            }
        }


        /// <summary>
        /// 获得初步比较粗的字段语句
        /// </summary>
        /// <param name="select"></param>
        private void GetSelectColumnSql(string select)
        {
            var allPoint = GetAllMatchPoint(select, "^,");
            if (allPoint.Count > 0)
            {
                for (int i = 0; i < allPoint.Count; i++)
                {
                    if (i == 0)
                    {
                        var first = select.Substring(0, allPoint[i].Point);
                        if (string.IsNullOrWhiteSpace(first))
                        {
                            continue;
                        }
                        //移除SELECT相关数据
                        var findSelect = GetAllMatchPoint(first, @"(^\(?\s*SELECT\s+DISTINCT)|(^\(?\s*SELECT\s+TOP\s+[0-9]+)|(^\(?\s*SELECT)").First();
                        var startPoint = findSelect.Point + findSelect.ActualContent.Length;
                        first = select.Substring(startPoint, first.Length - startPoint);
                        SelectQuery.Add(first);
                    }

                    if (i > 0 && i < allPoint.Count)
                    {
                        var newStartPoint = allPoint[i - 1].Point + allPoint[i - 1].ActualContent.Length;
                        SelectQuery.Add(select.Substring(newStartPoint, allPoint[i].Point - newStartPoint));
                    }
                    if (i == allPoint.Count - 1)
                    {
                        var newStartPoint = allPoint[i].Point + allPoint[i].ActualContent.Length;
                        SelectQuery.Add(select.Substring(newStartPoint, select.Length - newStartPoint));
                    }
                }
            }
            else  //仅仅一个select
            {
                var findSelect = GetAllMatchPoint(select, "^\\(?\\s*SELECT").First();
                var startPoint = findSelect.Point + findSelect.ActualContent.Length;
                var first = select.Substring(startPoint, select.Length - startPoint);
                SelectQuery.Add(first);
            }
        }

        /// <summary>
        /// 获得初步比较粗的字段语句
        /// </summary>
        private void GetOrderbyColumnSql(string orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return;
            }
            var allPoint = GetAllMatchPoint(orderBy, "^,");
            if (allPoint.Count > 0)
            {
                for (int i = 0; i < allPoint.Count; i++)
                {
                    if (i == 0)
                    {
                        var first = orderBy.Substring(0, allPoint[i].Point);
                        if (string.IsNullOrWhiteSpace(first))
                        {
                            continue;
                        }
                        //移除SELECT相关数据
                        var findSelect = GetAllMatchPoint(first, "^ORDER\\s+BY\\s*").First();
                        var startPoint = findSelect.Point + findSelect.ActualContent.Length;
                        first = orderBy.Substring(startPoint, first.Length - startPoint);
                        OrderbyQuery.Add(first);
                    }

                    if (i > 0 && i < allPoint.Count)
                    {
                        var newStartPoint = allPoint[i - 1].Point + allPoint[i - 1].ActualContent.Length;
                        SelectQuery.Add(orderBy.Substring(newStartPoint, allPoint[i].Point - newStartPoint));
                    }
                    if (i == allPoint.Count - 1)
                    {
                        var newStartPoint = allPoint[i].Point + allPoint[i].ActualContent.Length;
                        SelectQuery.Add(orderBy.Substring(newStartPoint, orderBy.Length - newStartPoint));
                    }
                }
            }
            else //仅仅一个select
            {
                var findSelect = GetAllMatchPoint(orderBy, "^ORDER\\s+BY\\s*").First();
                var startPoint = findSelect.Point + findSelect.ActualContent.Length;
                var first = orderBy.Substring(startPoint, orderBy.Length - startPoint);
                OrderbyQuery.Add(first);
            }
        }

        /// <summary>
        /// 将SQL 语句拆分为三部分
        /// </summary>
        /// <param name="sql"></param>
        private void AnalyseSql(string sql)
        {
            if (!string.IsNullOrWhiteSpace(sql))
            {
                sql = sql.Trim();
                string meeted = string.Empty;
                if (StartWith(sql, "^\\(?\\s*SELECT", out meeted))
                {
                    FromIndex = FindFrom(sql, "^FROM\\s*\\(?");
                    WhereIndex = FindFrom(sql, "^WHERE\\s+");
                    GroupIndex = FindFrom(sql, "^GROUP\\s+BY\\s*");
                    OrderIndex = FindFrom(sql, "^ORDER\\s+BY\\s*");

                    var endIndex = 0;
                    //获取SELECT 子句
                    if (FromIndex != -1)
                    {
                        endIndex = FromIndex;
                    }
                    else if (WhereIndex != -1)
                    {
                        endIndex = WhereIndex;
                    }
                    else if (GroupIndex != -1)
                    {
                        endIndex = GroupIndex;
                    }
                    else if (OrderIndex != -1)
                    {
                        endIndex = OrderIndex;
                    }
                    else
                    {
                        endIndex = sql.Length;
                    }
                    Select = sql.Substring(0, endIndex);

                    endIndex = 0;
                    if (FromIndex != -1)
                    {

                        //获取FROM 子句 一定会有
                        if (WhereIndex != -1)
                        {
                            endIndex = WhereIndex;
                        }
                        else if (GroupIndex != -1)
                        {
                            endIndex = GroupIndex;
                        }
                        else if (OrderIndex != -1)
                        {
                            endIndex = OrderIndex;
                        }
                        else
                        {
                            endIndex = sql.Length;
                        }
                        From = sql.Substring(FromIndex, endIndex - FromIndex);
                    }
                    //获取WHERE 子句
                    if (WhereIndex != -1)
                    {
                        endIndex = 0;
                        if (GroupIndex != -1)
                        {
                            endIndex = GroupIndex;
                        }
                        else if (OrderIndex != -1)
                        {
                            endIndex = OrderIndex;
                        }
                        else
                        {
                            endIndex = sql.Length;
                        }
                        Where = sql.Substring(WhereIndex, endIndex - WhereIndex);
                    }
                    //Group By 子句
                    if (GroupIndex != -1)
                    {
                        endIndex = 0;
                        if (OrderIndex != -1)
                        {
                            endIndex = OrderIndex;
                        }
                        else
                        {
                            endIndex = sql.Length;
                        }
                        GroupBy = sql.Substring(GroupIndex, endIndex - GroupIndex);
                    }
                    //获取ORDER By 子句
                    if (OrderIndex != -1)
                    {
                        OrderBy = sql.Substring(OrderIndex);
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有匹配的点
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="mainWord"></param>
        /// <returns></returns>
        private List<StrPoint> GetAllMatchPoint(string sql, string mainWord)
        {
            List<StrPoint> findStack = new List<StrPoint>();
            var patterns = new string[] { mainWord, "^\\(", "^\\)", "^'" };
            var curPoint = 0;
            var subSql = sql;
            do
            {
                foreach (var keyWord in patterns)
                {
                    string meetContent;
                    if (StartWith(subSql, keyWord, out meetContent))
                    {
                        findStack.Add(new StrPoint() { Pattern = keyWord, ActualContent = meetContent, Point = curPoint });
                    }
                }
                //开心消消乐 '(' ')'消去 
                SqlMacher.ClearCombo(findStack, patterns[1], patterns[2]); 
                //开心消消乐 ''' '''消去 
                SqlMacher.ClearCombo(findStack, patterns[3], patterns[3]); 
                //开心消消乐 '(' 'mainWord' ')'消去 
                SqlMacher.ClearCombo(findStack, patterns[1], patterns[2], mainWord); 
                //开心消消乐 ''' 'mainWord' '''消去 
                SqlMacher.ClearCombo(findStack, patterns[3], patterns[3], mainWord); 
                ++curPoint;
                subSql = subSql.Substring(1, subSql.Length - 1);
            } while (!string.IsNullOrWhiteSpace(subSql));
            return findStack;
        }

        private int FindFrom(string sql, string mainWord)
        {
            var findStack = GetAllMatchPoint(sql, mainWord);
            if (findStack.Count == 1)
            {
                return findStack[0].Point;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="meetContent"></param>
        /// <returns></returns>
        private bool StartWith(string str, string pattern, out string meetContent)
        {
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            var ches = reg.Matches(str);
            if (ches.Count > 0)
            {
                meetContent = ches[0].Value;
                return true;
            }
            meetContent = null;
            return false;
        }

        /// <summary>
        /// 获取匹配的列.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string GetMatchedSelectColumn(string str)
        {
            var aliasCol = "\\[?\\'?([\\w_#@\\$]+)\\'?\\]?";
            var col = "\\[?[\\w_#@\\$]+\\]?";
            var aliasTable = "\\[?[\\w_#@\\$]+\\]?";
            //"UserNickName"
            var match1 = "^(" + col + ")$";
            //"us.UserNickName"
            var match2 = "^" + aliasTable + "\\.(" + col + ")$";
            //"us.UserNickName UserName"
            var match3 = "^" + col + "\\s+" + aliasCol + "$";
            //"UserNickName UserName"
            var match4 = "^" + aliasTable + "\\." + col + "\\s+" + aliasCol + "$";
            //"UserNickName as UserName"
            var match5 = "^" + col + "\\s+AS\\s+" + aliasCol + "$";
            //"us.UserNickName as UserName"
            var match6 = "^" + aliasTable + "\\." + col + "\\s+AS\\s+" + aliasCol + "$";
            //"UserName = us.UserNickName"
            var match8 = "^" + aliasCol + "\\s*=\\s*" + col + "$";
            //"UserName = us.UserNickName"
            var match7 = "^" + aliasCol + "\\s*=\\s*" + aliasTable + "\\." + col + "$";
            var match9 = "[\\w\\W]+" + "\\s+AS\\s+" + aliasCol + "$";
            var match10 = "^" + aliasCol + "\\s*=\\s*[\\w\\W]+";
            var match11 = "[\\w\\W]+" + "\\s+" + aliasCol + "$";
            var match12 = "^(\\*)$";
             
            var matches = new string[] { match1, match2, match3, match4, match7, match5, match6, match8, match9, match10, match11, match12 };
            var colName = string.Empty;
            var i = 0;
            while (string.IsNullOrWhiteSpace(colName) && i < matches.Length)
            {
                colName = SqlMacher.GetMatchedValueIgnoreCase(str, matches[i]);
                i++;
            }
            return colName;
        }
        /// <summary>
        /// 获取匹配的列.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string GetMatchedOrderbyColumn(string str)
        {
            var col = "\\[?[\\w_#@\\$]+\\]?";
            var aliasTable = "\\[?[\\w_#@\\$]+\\]?";

            //"UserNickName"
            var match1 = "^(" + col + ")$";
            //"UserNickName ASC"
            var match2 = "^(" + col + "\\s+ASC)$";
            //"UserNickName DESC"
            var match3 = "^(" + col + "\\s+DESC)$";
            //"dbc.UserNickName"
            var match4 = "^" + aliasTable + "\\.(" + col + ")$";
            //"us.UserNickName ASC"
            var match5 = "^" + aliasTable + "\\.(" + col + "\\s+ASC)$";
            //"us.UserNickName DESC"
            var match6 = "^" + aliasTable + "\\.(" + col + "\\s+DESC)$";


            var matches = new string[] { match1, match2, match3, match4,  match5, match6};
            var colName = string.Empty;
            var i = 0;
            while (string.IsNullOrWhiteSpace(colName) && i < matches.Length)
            {
                colName = SqlMacher.GetMatchedValueIgnoreCase(str, matches[i]);
                i++;
            }
            return colName;
        }
    }
}
