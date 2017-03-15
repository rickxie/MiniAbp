using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MiniAbp.DataAccess.SqlParser
{
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
                //开心消消乐 '(' ')'消去 
                SqlMacher.ClearCombo(findStack, patterns[1], patterns[2]);
                //开心消消乐 ''' '''消去 
                SqlMacher.ClearCombo(findStack, patterns[3], patterns[3]);
                //开心消消乐 '(' 'mainWord' ')'消去 
                SqlMacher.ClearCombo(findStack, patterns[1], patterns[2], patterns[0], patterns[4]);
                //开心消消乐 ''' 'mainWord' '''消去 
                SqlMacher.ClearCombo(findStack, patterns[3], patterns[3], patterns[0], patterns[4]);

                ++curPoint;
                subSql = subSql.Substring(1, subSql.Length - 1);
            } while (!string.IsNullOrWhiteSpace(subSql));
            return findStack;
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
        /// <param name="needOrderBy">非求分页后的Count</param>
        /// <returns></returns>
        public string GetPackagedSql(string orderBy = "", bool needOrderBy = true)
        {
            if (HasUnion)
            {
                string wholeOrderBy = string.Empty;
                if (needOrderBy)
                {
                    wholeOrderBy = GetOrderByTemp(orderBy);
                }

                StringBuilder entireQuery = new StringBuilder();
                int i = 0;
                Querys.ForEach(r =>
                {
                    entireQuery.Append(r.Select);
                    entireQuery.Append(" " + r.From);
                    entireQuery.Append(" " + r.Where);
                    entireQuery.Append(" " + r.GroupBy);
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
                string wholeOrderBy = string.Empty;
                StringBuilder entireQuery = new StringBuilder();
                var r = Querys[0];
                entireQuery.Append(r.Select);
                entireQuery.Append(" " + r.From);
                entireQuery.Append(" " + r.Where);
                entireQuery.Append(" " + r.GroupBy);

                var sql = entireQuery.ToString();
                //使用外部提供的或者是默认的
                if(needOrderBy)
                {
                    wholeOrderBy = GetOrderByTemp(orderBy);
                }
                var queryPackage = "SELECT * " + wholeOrderBy + "　FROM ( " + sql + ") " + WrapAlias + " ";
                return queryPackage;
            }
        }
        /// <summary>
        /// 获取分页Sql
        /// </summary>
        /// <param name="startCount"></param>
        /// <param name="endCount"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public string GetPageSql(int startCount, int endCount, string orderBy = "")
        {
            SqlScriptManager sqlMng = this;
            var pagePackage = @"SELECT * FROM ({0}) "+ WrapPagedAlias +" WHERE "+ WrapPagedAlias + "._$tmpRowNum BETWEEN " + startCount +
                              " AND " + endCount;
            var queryPackage = GetPackagedSql(orderBy);

            return sqlMng.CtePart + string.Format(pagePackage, queryPackage);
        }

        /// <summary>
        /// 被打包后的Sql 求分页数据
        /// </summary>
        /// <param name="startCount"></param>
        /// <param name="endCount"></param>
        /// <param name="packagedsql"></param>
        /// <returns></returns>
        public string GetPageSqlWithPackageSql(int startCount, int endCount, string packagedsql)
        {
            SqlScriptManager sqlMng = this;
            var pageWrapFormat = @"SELECT * FROM ({0}) "+ WrapPagedAlias +" WHERE "+ WrapPagedAlias + "._$tmpRowNum BETWEEN " + startCount +
                              " AND " + endCount;

            return sqlMng.CtePart + string.Format(pageWrapFormat, packagedsql);
        }

        /// <summary>
        /// 被打包后的sql求数量
        /// </summary>
        /// <param name="packagedsql"></param>
        /// <returns></returns>
        public string GetPageCountSqlWithPackageSql(string packagedsql)
        {
            string countPackage = @"SELECT COUNT(1) FROM ( {0} ) " + WrapAlias + " ";
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
        /// <param name="passedOrderBy">传入的Order by</param>
        /// <returns></returns>
        private string GetOrderByTemp(string passedOrderBy)
        {
            var query = Querys[0];
            if (query == null)
            {
                throw new ArgumentException("子Query不能不存在");
            }
            string wholeOrderBy = string.Empty;
            //拼接最后一个SQL 含Order By 则这个OrderBy是整个查询联合后的查询
            if (HasUnion)
            {
                if (Querys[Querys.Count - 1].HasOrderBy)
                {
                    wholeOrderBy = Querys.Last().OrderBy;
                }
            }
            else
            {
                if (query.HasOrderBy)
                {
                    var unionedOrderby = string.Join(",", query.OrderbyItems);
                    wholeOrderBy = "ORDER BY " + unionedOrderby;
                }
             
            }
            
            //有先使用传入进来的Orderby
            if (!string.IsNullOrWhiteSpace(passedOrderBy))
            {
                wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER (" + passedOrderBy + ")";
            }
            //使用SQL自带的OrderBy
            else if (!string.IsNullOrWhiteSpace(wholeOrderBy))
            {
                wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER (" + wholeOrderBy + ")";
            }
            //使用默认的OrderBy
            else
            {
                var hasId = query.SelectItems.Any(r => r.Equals("ID", StringComparison.OrdinalIgnoreCase));
                if (hasId || (query.SelectItems.Count == 1 && query.SelectItems[0].Equals("*")))
                {
                    wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER ( ORDER BY [Id])";
                }
                else if (query.SelectItems.Count > 1)
                {
                    var first = query.SelectItems.FirstOrDefault(r => r != "*");
                    wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER ( ORDER BY ["+ first + "])";
                }
                else
                {
                    wholeOrderBy = @", _$tmpRowNum = ROW_NUMBER() OVER ( ORDER BY [Id])";
                }

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
