using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiniAbp.DataAccess.SqlParser
{
    public class SqlMacher
    {
        /// <summary>
        /// 满足左中右方式自动消除
        /// </summary>
        /// <param name="findStack"></param>
        /// <param name="headPattern"></param>
        /// <param name="endPattern"></param>
        /// <param name="middle"></param>
        public static void ClearCombo(List<StrPoint> findStack, string headPattern, string endPattern, params string[] middle)
        {
            var len = findStack.Count;

            var meetRequirement = len >= 3 && findStack[len - 1].Pattern == endPattern;
            //开心消消乐 '(' 'mainWord' ')'消去 
            if (meetRequirement) //')'
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
                if (hasCondtion && toPosition >= 0 && findStack[toPosition].Pattern == headPattern)
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
                    } while (k >= 0 && k >= toPosition);
                }
            }
        }

        /// <summary>
        /// 消除左右
        /// </summary>
        /// <param name="findStack"></param>
        /// <param name="headPattern"></param>
        /// <param name="endPattern"></param>
        public static void ClearCombo(List<StrPoint> findStack, string headPattern, string endPattern)
        {
            var len = findStack.Count;
            if (len >= 2 && findStack[len - 1].Pattern == endPattern && findStack[len - 2].Pattern == headPattern)
            {
                findStack.Remove(findStack[len - 1]);
                findStack.Remove(findStack[len - 2]);
            }
        }

        /// <summary>
        /// 获取匹配的值 忽略大小写
        /// </summary>
        /// <returns></returns>
        public static string GetMatchedValueIgnoreCase(string str, string pattern)
        {
            Regex rg = new Regex(pattern, RegexOptions.IgnoreCase);
            if (rg.IsMatch(str))
            {
                var matched = rg.Matches(str);
                return matched[0].Groups[1].Value;
            }
            return string.Empty;
        }
    }
}
