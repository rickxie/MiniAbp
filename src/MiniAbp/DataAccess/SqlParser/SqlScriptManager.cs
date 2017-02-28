using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MiniAbp.DataAccess.SqlParser
{
    public class SqlParser
    {
        public string QueryPart { get; set; } = string.Empty;
        public int FromIndex { get; set; }
        public int WhereIndex { get; set; }
        public int OrderIndex { get; set; }
        public int GroupIndex { get; set; }
        public string QuerySelectPart { get; set; } = "";
        public string QueryFromPart { get; set; } = "";
        public string QueryWherePart { get; set; } = "";
        public string QueryGroupByPart { get; set; } = "";
        public string QueryOrderByPart { get; set; } = "";
        //WHERE 中的每个项目
        private List<string> SelectQuery { get; set; } = new List<string>();
        public List<string> SelectItems { get; set; } = new List<string>();
        public SqlParser(string sql)
        {
            QueryPart = sql;
            SplitSql(QueryPart);
            GetSelectQuery(QuerySelectPart);
            GetSelectItems();
        }

        private void GetSelectItems()
        {
            foreach (var colQuery in SelectQuery)
            {
                var colName = GetColumnName(colQuery);
                if (!string.IsNullOrWhiteSpace(colName))
                {
                    SelectItems.Add(colName);
                }
            }

        }
        /// <summary>
        /// 根据子语句获得列名
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string GetColumnName(string sql)
        {
            if (!string.IsNullOrWhiteSpace(sql))
            {
                sql = sql.Trim();
                //单纯的 xxxx.xxxx 没有 As 没有 =
                return GetMatchedColumn(sql);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获得初步比较粗的字段语句
        /// </summary>
        /// <param name="select"></param>
        private void GetSelectQuery(string select)
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
        /// 将SQL 语句拆分为三部分
        /// </summary>
        /// <param name="sql"></param>
        private void SplitSql(string sql)
        {
            if (!string.IsNullOrWhiteSpace(sql))
            {
                sql = sql.Trim();
                string meeted = string.Empty;
                if (StartWith(sql, "^\\(?\\s*SELECT", out meeted))
                {
                    FromIndex = FindFrom(sql, "^FROM(\\s+|\\()");
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
                    QuerySelectPart = sql.Substring(0, endIndex);

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
                        QueryFromPart = sql.Substring(FromIndex, endIndex - FromIndex);
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
                        QueryWherePart = sql.Substring(WhereIndex, endIndex - WhereIndex);
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
                        QueryGroupByPart = sql.Substring(GroupIndex, endIndex - GroupIndex);
                    }
                    //获取ORDER By 子句
                    if (OrderIndex != -1)
                    {
                        QueryOrderByPart = sql.Substring(OrderIndex);
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
                var len = findStack.Count;
                //开心消消乐 '(' ')'消去 
                if (len >= 2 && findStack[len - 1].Pattern == patterns[2] && findStack[len - 2].Pattern == patterns[1])
                {
                    findStack.Remove(findStack[len - 1]);
                    findStack.Remove(findStack[len - 2]);
                }
                len = findStack.Count;
                //开心消消乐 ''' '''消去 
                if (len >= 2 && findStack[len - 1].Pattern == patterns[3] && findStack[len - 2].Pattern == patterns[3])
                {
                    findStack.Remove(findStack[len - 1]);
                    findStack.Remove(findStack[len - 2]);
                }
                len = findStack.Count;
                //开心消消乐 '(' 'mainWord' ')'消去 
                if (len >= 3 && findStack[len - 1].Pattern == patterns[2] && findStack[len - 2].Pattern == mainWord &&
                    findStack[len - 3].Pattern == patterns[1])
                {
                    findStack.Remove(findStack[len - 1]);
                    findStack.Remove(findStack[len - 2]);
                    findStack.Remove(findStack[len - 3]);
                }
                len = findStack.Count;
                //开心消消乐 ''' 'mainWord' '''消去 
                if (len >= 3 && findStack[len - 1].Pattern == patterns[3] && findStack[len - 2].Pattern == mainWord &&
                    findStack[len - 3].Pattern == patterns[3])
                {
                    findStack.Remove(findStack[len - 1]);
                    findStack.Remove(findStack[len - 2]);
                    findStack.Remove(findStack[len - 3]);
                }
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
        public string GetMatchedColumn(string str)
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
            Func<string, string, string> getName = (s, p) =>
            {
                Regex rg = new Regex(p, RegexOptions.IgnoreCase);
                if (rg.IsMatch(s))
                {
                    var matched = rg.Matches(s);
                    return matched[0].Groups[1].Value;
                }
                return string.Empty;
            };
            var matches = new string[] { match1, match2, match3, match4, match7, match5, match6, match8, match9, match10, match11, match12 };
            var colName = string.Empty;
            var i = 0;
            while (string.IsNullOrWhiteSpace(colName) && i < matches.Length)
            {
                colName = getName(str, matches[i]);
                i++;
            }
            return colName;
        }



    }

    public class SqlScriptManager
    {
        public string CtePart { get; set; } = "";
        public string QueryPart { get; set; } = "";
        public bool HasUnion => UnionOrAll.Count > 0;
        public List<string> UnionOrAll { get; set; } = new List<string>();
        public List<string> SubQuerys { get; set; } = new List<string>();
        public List<SqlParser> Querys { get; set; } = new List<SqlParser>();
        public string WrapAlias = "_$tmp";
        public string WrapPagedAlias = "_$tmpPaged";
        public SqlScriptManager(string sql)
        {
            ParseCteAndQueryPart(sql);
            SplitByUnion(QueryPart);
        }

        /// <summary>
        /// 正则匹配CTE 表达式并加ID过滤
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private void ParseCteAndQueryPart(string sql)
        {
            //处理空格和分号
            sql = PreperationSql(sql);
            string matchedValue;
            if (StartWith(sql, @"\)\s*\(?\s*SELECT", out matchedValue))
            {
                var indexOfSql = sql.IndexOf(matchedValue, StringComparison.Ordinal);
                //CTE部分
                CtePart = sql.Substring(0, indexOfSql + 1).Trim();
                //SELECT部分
                QueryPart = sql.Substring(indexOfSql + 1, sql.Length - indexOfSql - 1).Trim();
            }
            else
            {
                CtePart = "";
                QueryPart = sql.Trim();
            }
        }
        /// <summary>
        /// 删除分号 和 注释
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string PreperationSql(string sql)
        {
            sql = sql.Trim();
            sql = sql.StartsWith(";") ? sql.Substring(1, sql.Length - 1) : sql;
            sql = sql.EndsWith(";") ? sql.Substring(0, sql.Length - 1) : sql;
            Regex rgSingleComma = new Regex("--.*\n", RegexOptions.IgnoreCase);
            Regex rgMultComma = new Regex(@"\/\*([^\*^\/]*|[\*^\/*]*|[^\**\/]*)\*\/", RegexOptions.IgnoreCase);
            sql = rgSingleComma.Replace(sql, "");
            sql = rgMultComma.Replace(sql, "");
            return sql;
        }
        private void SplitByUnion(string sql)
        {
            var allUnion = FindAllUnion(sql);
            UnionOrAll = allUnion.Select(r => r.ActualContent).ToList();
            if (allUnion.Count > 0)
            {

                for (int i = 0; i < allUnion.Count; i++)
                {
                    if (i == 0)
                        SubQuerys.Add(sql.Substring(0, allUnion[i].Point));

                    if (i > 0 && i < allUnion.Count)
                    {
                        var newStartPoint = allUnion[i - 1].Point + allUnion[i - 1].ActualContent.Length;
                        SubQuerys.Add(sql.Substring(newStartPoint, allUnion[i].Point - newStartPoint));
                    }
                    if (i == allUnion.Count - 1)
                    {
                        var newStartPoint = allUnion[i].Point + allUnion[i].ActualContent.Length;
                        SubQuerys.Add(sql.Substring(newStartPoint, sql.Length - newStartPoint));
                    }
                }

                SubQuerys.ForEach(r =>
                {
                    Querys.Add(new SqlParser(r));
                });
            }
            else
            {
                Querys.Add(new SqlParser(QueryPart));
            }
        }

        private List<StrPoint> FindAllUnion(string sql)
        {
            List<StrPoint> findStack = new List<StrPoint>();
            var patterns = new string[] { "^UNION\\s+ALL", "^\\(", "^\\)", "^'", "^UNION" };
            var curPoint = 0;
            var subSql = sql;
            do
            {
                foreach (var keyWord in patterns)
                {
                    var meetContent = "";
                    if (StartWith(subSql, keyWord, out meetContent))
                    {
                        findStack.Add(new StrPoint() { Pattern = keyWord, ActualContent = meetContent, Point = curPoint });
                        break;
                    }
                }
                var len = findStack.Count;
                //开心消消乐 '(' ')'消去 
                if (len >= 2 && findStack[len - 1].Pattern == patterns[2] && findStack[len - 2].Pattern == patterns[1])
                {
                    findStack.Remove(findStack[len - 1]);
                    findStack.Remove(findStack[len - 2]);
                }
                len = findStack.Count;
                //开心消消乐 ''' '''消去 
                if (len >= 2 && findStack[len - 1].Pattern == patterns[3] && findStack[len - 2].Pattern == patterns[3])
                {
                    findStack.Remove(findStack[len - 1]);
                    findStack.Remove(findStack[len - 2]);
                }
                //开心消消乐 '(' 'mainWord' ')'消去 
                ClearCombo(findStack, patterns[1], patterns[2], patterns[0], patterns[4]);
                //开心消消乐 ''' 'mainWord' '''消去 
                ClearCombo(findStack, patterns[3], patterns[3], patterns[0], patterns[4]);

                ++curPoint;
                subSql = subSql.Substring(1, subSql.Length - 1);
            } while (!string.IsNullOrWhiteSpace(subSql));
            return findStack;
        }

        private void ClearCombo(List<StrPoint> findStack, string head, string end, params string[] middle)
        {
            var len = findStack.Count;
            //开心消消乐 '(' 'mainWord' ')'消去 
            if (len >= 3 && findStack[len - 1].Pattern == end) //')'
            {
                var toPosition = len - 2;
                var needClear = false;
                var hasCondtion = false;
                //找到最前一个Union
                for (int j = toPosition; j >= 0; j--)
                {
                    if (middle.Contains(findStack[j].Pattern))
                    {
                        hasCondtion = true;
                        continue;
                    }
                    else
                    {
                        toPosition = j;
                        break;
                    }
                }
                //判断它的前一个是否为 '('
                if (hasCondtion && toPosition >= 0 && findStack[toPosition].Pattern == head)
                {
                    needClear = true;
                }
                if (needClear && toPosition >= 0)
                {
                    var k = len - 1;
                    do
                    {
                        findStack.Remove(findStack[k]);
                        k--;
                    } while (k >= 0);
                }
            }
        }
        private bool StartWith(string str, string pattern, out string matchedContent)
        {
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            var ches = reg.Matches(str);
            if (ches.Count == 1)
            {
                matchedContent = ches[0].Value;
                return true;
            }
            matchedContent = null;
            return false;
        }

        /// <summary>
        /// 获取非CTE字段的信息 包含了OrderBy处理
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="needOrderBy"></param>
        /// <returns></returns>
        public string GetPackagedSql(string orderBy = "", bool needOrderBy = true)
        {
            if (HasUnion)
            {
                string wholeOrderBy = string.Empty;
                if (needOrderBy)
                {
                    //并且最后一个SQL 含Order By 则这个OrderBy是整个查询联合后的查询
                    if (Querys[Querys.Count - 1].OrderIndex > -1)
                    {
                        wholeOrderBy = Querys.Last().QueryOrderByPart;
                    }
                    wholeOrderBy = GetOrderByTemp(wholeOrderBy, orderBy);
                }

                StringBuilder entireQuery = new StringBuilder();
                int i = 0;
                Querys.ForEach(r =>
                {
                    entireQuery.Append(r.QuerySelectPart);
                    entireQuery.Append(" " + r.QueryFromPart);
                    entireQuery.Append(" " + r.QueryWherePart);
                    entireQuery.Append(" " + r.QueryGroupByPart);
                    if (i < UnionOrAll.Count)
                    {
                        entireQuery.Append(" " + UnionOrAll[i++] + " ");
                    }
                });

                var queryPackage = entireQuery.ToString();
                queryPackage = "SELECT * " + wholeOrderBy + "　FROM ( " + queryPackage + ") " + WrapAlias + " ";
                return queryPackage;
            }
            else //没有Union的情况
            {
                bool useSelfOrderBy = false;
                string wholeOrderBy = string.Empty;
                StringBuilder entireQuery = new StringBuilder();
                int i = 0;
                var r = Querys[0];
                entireQuery.Append(r.QuerySelectPart);
                if (needOrderBy)
                {
                    useSelfOrderBy = !string.IsNullOrWhiteSpace(r.QueryOrderByPart) &&
                                     string.IsNullOrWhiteSpace(orderBy);
                    //使用自身提供的默认的Orderby
                    if (useSelfOrderBy)
                    {
                        wholeOrderBy = r.QueryOrderByPart;
                        wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER (" + wholeOrderBy + ")";
                    }
                    entireQuery.Append(" " + wholeOrderBy);
                }
                entireQuery.Append(" " + r.QueryFromPart);
                entireQuery.Append(" " + r.QueryWherePart);
                entireQuery.Append(" " + r.QueryGroupByPart);

                var sql = entireQuery.ToString();
                string queryPackage;
                //使用外部提供的或者是默认的
                if (needOrderBy && !useSelfOrderBy)
                {
                    if (!string.IsNullOrWhiteSpace(orderBy))
                    {
                        wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER (" + orderBy + ")";
                    }
                    else
                    {
                        var columnName =
                            r.SelectItems.FirstOrDefault(w => w.Equals("Id", StringComparison.OrdinalIgnoreCase));
                        if (string.IsNullOrWhiteSpace(columnName))
                        {
                            columnName = r.SelectItems.FirstOrDefault(w => !w.Equals("*"));
                            if (string.IsNullOrWhiteSpace(columnName))
                            {
                                columnName = "Id";
                            }
                        }
                        wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER ( ORDER BY " + columnName + ")";
                    }
                    queryPackage = "SELECT * " + wholeOrderBy + "　FROM ( " + sql + ") " + WrapAlias + " ";
                }
                else
                {
                    queryPackage = "SELECT * FROM ( " + sql + ") " + WrapAlias + " ";
                }
                return queryPackage;
            }
        }

        public string GetPageSql(int startCount, int endCount, string orderBy = "")
        {
            SqlScriptManager sqlMng = this;
            var pagePackage = @"SELECT * FROM ({0}) "+ WrapPagedAlias +" WHERE "+ WrapPagedAlias + "._$tmpRowNum BETWEEN " + startCount +
                              " AND " + endCount;
            var queryPackage = GetPackagedSql(orderBy);

            return sqlMng.CtePart + string.Format(pagePackage, queryPackage);
        }
        public string GetPageSqlWithPackageSql(int startCount, int endCount, string packagedsql)
        {
            SqlScriptManager sqlMng = this;
            var pageWrapFormat = @"SELECT * FROM ({0}) "+ WrapPagedAlias +" WHERE "+ WrapPagedAlias + "._$tmpRowNum BETWEEN " + startCount +
                              " AND " + endCount;

            return sqlMng.CtePart + string.Format(pageWrapFormat, packagedsql);
        }
        public string GetPageCountSqlWithPackageSql(string packagedsql)
        {
            string countPackage = @"SELECT COUNT(1) FROM ( {0} ) " + WrapAlias + " ";
            var s = GetPackagedSql(string.Empty, false);
            return CtePart + string.Format(countPackage, packagedsql);
        }

        /// <summary>
        /// GetCountSql
        /// </summary>
        /// <returns></returns>
        public string GetPageCountSql()
        {
            SqlScriptManager sqlMng = this;
            string countPackage = @"SELECT COUNT(1) FROM ( {0} ) " + WrapAlias + " ";
            var s = GetPackagedSql(string.Empty, false);
            return sqlMng.CtePart + string.Format(countPackage, s);
        }

        /// <summary>
        /// 如果有提供Orderby 则移除原有OrderBy， 如果没提供orderBy 则使用默认OrderBy
        /// </summary>
        /// <param name="wholeOrderBy"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        private string GetOrderByTemp(string wholeOrderBy, string orderBy)
        {
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER (" + orderBy + ")";
            }
            else if (!string.IsNullOrWhiteSpace(wholeOrderBy))
            {
                wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER (" + wholeOrderBy + ")";
            }
            else
            {
                wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER ( ORDER BY Id)";
            }
            return wholeOrderBy;

        }

    }

    public struct StrPoint
    {
        public string ActualContent { get; set; }
        public string Pattern { get; set; }
        public int Point { get; set; }
    }
}
