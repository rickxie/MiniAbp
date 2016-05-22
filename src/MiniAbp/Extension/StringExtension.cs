using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="originalStr"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string Fill(this string originalStr, params object[] para)
        {
            var result = string.Format(originalStr, para);
            return result;
        }
        /// <summary>
        /// isNotNullOrEmptyWhite()
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string str)
        {
            if (str == null)
                return true;
            for (int index = 0; index < str.Length; ++index)
            {
                if (!char.IsWhiteSpace(str[index]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// isNotNullOrEmptyWhite()
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool HasValue(this string str)
        {
            return !IsEmpty(str);
        }

        public static string ToCamelCase(this string str)
        {
            if (str.IsEmpty() || str.Length <= 1)
            {
                throw new ArgumentNullException("str");
            }

            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
    }
}
