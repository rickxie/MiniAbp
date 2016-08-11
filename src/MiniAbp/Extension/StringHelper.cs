using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiniAbp.Extension
{
    public class StringHelper
    {

        #region 特殊字符    

        /// <summary>    
        /// 检测是否有Sql危险字符    
        /// </summary>    
        /// <param name="str">要判断字符串</param>    
        /// <returns>判断结果</returns>    
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|||||\}|\{|%|@|\*|!|\']");
        }

        /// <summary>    
        /// 删除SQL注入特殊字符    
        /// 加入对输入参数sql为Null的判断    
        /// </summary>    
        public static string StripSqlInjection(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                //过滤 ' --    
                const string pattern1 = @"(\%27)|(\')|(\-\-)";

                //防止执行 ' or    
                const string pattern2 = @"((\%27)|(\'))\s*((\%6F)|o|(\%4F))((\%72)|r|(\%52))";

                //防止执行sql server 内部存储过程或扩展存储过程    
                const string pattern3 = @"\s+exec(\s|\+)+(s|x)p\w+";

                sql = Regex.Replace(sql, pattern1, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern2, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern3, string.Empty, RegexOptions.IgnoreCase);
            }
            return sql;
        }

        public static string SqlSafe(string parameter)
        {
            parameter = parameter.ToLower();
            parameter = parameter.Replace("'", "");
            parameter = parameter.Replace(">", ">");
            parameter = parameter.Replace("<", "<");
            parameter = parameter.Replace("\n", "<br>");
            parameter = parameter.Replace("\0", "·");
            return parameter;
        }

        /// <summary>    
        /// 清除xml中的不合法字符    
        /// </summary>    
        /// <remarks>    
        /// 无效字符：    
        /// 0x00 - 0x08    
        /// 0x0b - 0x0c    
        /// 0x0e - 0x1f    
        /// </remarks>    
        public static string CleanInvalidCharsForXml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            else
            {
                StringBuilder checkedStringBuilder = new StringBuilder();
                Char[] chars = input.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    int charValue = Convert.ToInt32(chars[i]);

                    if ((charValue >= 0x00 && charValue <= 0x08) || (charValue >= 0x0b && charValue <= 0x0c) || (charValue >= 0x0e && charValue <= 0x1f))
                        continue;
                    else
                        checkedStringBuilder.Append(chars[i]);
                }

                return checkedStringBuilder.ToString();

                //string result = checkedStringBuilder.ToString();    
                //result = result.Replace("&#x0;", "");    
                //return Regex.Replace(result, @"[\?-\\ \ \-\\?-\?]", delegate(Match m) { int code = (int)m.Value.ToCharArray()[0]; return (code > 9 ? "&#" + code.ToString() : "&#0" + code.ToString()) + ";"; });    
            }
        }


        /// <summary>    
        /// 改正sql语句中的转义字符    
        /// </summary>    
        public static string MashSql(string str)
        {
            return (str == null) ? "" : str.Replace("\'", "'");
        }

        /// <summary>    
        /// 替换sql语句中的有问题符号   
        /// </summary>    
        public static string ChkSql(string str)
        {
            return (str == null) ? "" : str.Replace("'", "''");
        }

        /// <summary>    
        ///  判断是否有非法字符   
        /// </summary>    
        /// <param name="strString"></param>    
        /// <returns>返回TRUE表示有非法字符，返回FALSE表示没有非法字符。</returns>    
        public static bool CheckBadStr(string strString)
        {
            bool outValue = false;
            if (!string.IsNullOrEmpty(strString))
            {
                ArrayList bidStrlist = new ArrayList();
                bidStrlist.Add("xp_cmdshell");
                bidStrlist.Add("truncate");
                bidStrlist.Add("net user");
                bidStrlist.Add("exec");
                bidStrlist.Add("net localgroup");
                bidStrlist.Add("select");
                bidStrlist.Add("asc");
                bidStrlist.Add("char");
                bidStrlist.Add("mid");
                bidStrlist.Add("insert");
                bidStrlist.Add("order");
                bidStrlist.Add("exec");
                bidStrlist.Add("delete");
                bidStrlist.Add("drop");
                bidStrlist.Add("truncate");
                bidStrlist.Add("1=1");
                bidStrlist.Add("1=2");
                string tempStr = strString.ToLower();
                for (int i = 0; i < bidStrlist.Count; i++)
                {
                    if (tempStr.IndexOf(bidStrlist[i].ToString(), StringComparison.Ordinal) > -1)
                    {
                        outValue = true;
                        break;
                    }
                }
            }
            return outValue;
        }

        #endregion
    }
}
