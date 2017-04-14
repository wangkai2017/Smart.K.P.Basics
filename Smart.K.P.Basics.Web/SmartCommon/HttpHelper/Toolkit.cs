using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SmartCommon.HttpHelper
{
    public class Toolkit
    {
        /// <summary> 清理HTTP请求或响应字符串前后的多余引号</summary>
        /// <param name="xml">待处理的字符串</param>
        /// <returns></returns>
        /// <remarks> add by grs at 2012-10-17 </remarks>
        public static string ClearSpecialCharForReq(string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return string.Empty;
            }

            return sourceString.Trim(new char[] { '"' });
        }
        /// <summary> 清理XML字符串左尖括号前面的多余字符</summary>
        /// <param name="xml">待处理的XML字符串</param>
        /// <returns></returns>
        /// <remarks> add by grs at 2012-10-16 </remarks>
        public static string ClearSpecialCharForXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return string.Empty;
            }

            return System.Text.RegularExpressions.Regex.Replace(xml, "^[^<]*", "");
        }
        /// <summary> 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值</summary>
        /// <param name="strName">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName)
        {
            if ("".Equals(GetQueryString(strName)))
            {
                return GetFormString(strName);
            }
            else
            {
                return GetQueryString(strName);
            }
        }
        /// <summary> 获得指定Url参数的值</summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName)
        {
            if (HttpContext.Current.Request.QueryString[strName] == null)
            {
                return "";
            }
            return HttpContext.Current.Request.QueryString[strName].Trim();
        }
        /// <summary> 获得指定表单参数的值</summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName)
        {
            if (HttpContext.Current.Request.Form[strName] == null)
            {
                return "";
            }
            return HttpContext.Current.Request.Form[strName].Trim();
        }

        /// <summary> 清理字符串中十六进制表示的不可见特殊符号，字符范围[0x00-0x1F]</summary>
        /// <param name="sourceString">要清理的字符串</param>
        /// <param name="replacement">把不可见的特殊符号替换成的字符串</param>
        /// <returns>清理后的字符串</returns>
        /// <remarks>add by grs at 2012-10-26</remarks>
        public static string ClearHexChar(string sourceString, string replacement)
        {
            return Regex.Replace(sourceString, "[\x00-\x1F]+", replacement);
        }

        /// <summary> 判断是不是偶数,是偶数时返回true</summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsEvenNumber(int num)
        {
            if (num < 0)
            {
                num = 0 - num;
            }
            if (num % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
