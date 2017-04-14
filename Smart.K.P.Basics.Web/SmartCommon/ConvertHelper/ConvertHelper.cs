using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SmartCommon.ConvertHelper
{
    /// <summary>
    /// 字符串转换
    /// </summary>
    public static class ConvertHelper
    {       
        /// <summary>数据库空时间 </summary>
        public static readonly DateTime NullSqlDateTime = ((DateTime)System.Data.SqlTypes.SqlDateTime.Null);
        public static bool ToBoolean(this string source)
        {
            bool reValue;
            bool.TryParse(source, out reValue);
            return reValue;
        }
        /// <summary>转化为Byte型</summary>
        public static Byte ToByte(this string source)
        {
            Byte reValue;
            Byte.TryParse(source, out reValue);
            return reValue;
        }
        /// <summary> 转化为Short型</summary>
        public static short ToShort(this string source)
        {
            short reValue;
            short.TryParse(source, out reValue);
            return reValue;
        }
        /// <summary>转化为Short型</summary>
        public static short ToInt16(this string source)
        {
            short reValue;
            short.TryParse(source, out reValue);
            return reValue;
        }

        /// <summary>转化为int32型</summary>
        public static int ToInt32(this string source)
        {
            int reValue;
            Int32.TryParse(source, out reValue);
            return reValue;
        }
        /// <summary>转化为int32型</summary>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int ToInt32(this string source, int defaultValue = 0)
        {
            int reValue;
            return Int32.TryParse(source, out reValue) ? reValue : defaultValue;
        }
        /// <summary>转化为int64型</summary>
        public static long ToInt64(this string source)
        {
            long reValue;
            Int64.TryParse(source, out reValue);
            return reValue;
        }
        /// <summary>转化为Float型</summary>
        public static float ToFloat(this string source)
        {
            float reValue;
            float.TryParse(source, out reValue);
            return reValue;
        }
        /// <summary>转化为Double型</summary>
        public static Double ToDouble(this string source)
        {
            Double reValue;
            Double.TryParse(source, out reValue);
            return reValue;
        }
        /// <summary>转化为decimal型</summary>
        public static decimal ToDecimal(this string source)
        {
            decimal reValue;
            decimal.TryParse(source, out reValue);
            return reValue;
        }
        /// <summary>转化为日期为空里返回NullSqlDateTime,byark</summary>
        public static DateTime ToDateTime(this string source)
        {
            DateTime reValue;
            return DateTime.TryParse(source, out reValue) ? reValue : new DateTime(1900, 01, 01);
        }
        /// <summary>转化为日期为空里返回NullSqlDateTime,byark</summary>
        public static DateTime ToDateTimeByNum(this string source)
        {
            DateTime reValue = NullSqlDateTime;
            if (source.Length == 14)
            {
                if (!DateTime.TryParse(source.Substring(0, 4) + "-" + source.Substring(4, 2) + "-" + source.Substring(6, 2) + " "
                    + source.Substring(8, 2) + ":" + source.Substring(10, 2) + ":" + source.Substring(12, 2), out reValue))
                    reValue = NullSqlDateTime;
            }
            return reValue;
        }
        /// <summary>转化为数字类型的日期</summary>
        public static decimal ToDateTimeDecimal(this string source)
        {
            DateTime reValue;
            return DateTime.TryParse(source, out reValue) ? reValue.ToString("yyyyMMddHHmmss").ToDecimal() : 0;
        }
        /// <summary>将时间转换成数字</summary>
        public static decimal ToDateTimeDecimal(this DateTime source)
        {
            return source.ToString("yyyyMMddHHmmss").ToDecimal();
        }
        /// <summary>将时间转换成字串</summary>
        public static string ToISO8601DateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }

        #region 将Dunull字段赋值

        /// <summary> 字符串是否为DateTime类型</summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        public static bool IsDateTime(object dateObj)
        {
            if (dateObj == null)
                return false;
            return IsDateTime(dateObj.ToString());
        }

        /// <summary> 将Dunull字段赋值转为时间</summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ToTime(object time)
        {
            DateTime dt = DateTime.Parse("1900-1-1");
            if (time == null || DBNull.Value == time)
            {
                return dt;
            }
            if (IsDateTime(time))
            {
                dt = DateTime.Parse(time.ToString());
            }
            return dt;
        }
        #endregion

        /// <summary> 将IP转换为long型</summary>
        public static long ToIPLong(this string ip)
        {
            byte[] bytes = IPAddress.Parse(ip).GetAddressBytes();
            return (long)bytes[3] + (((uint)bytes[2]) << 8) + (((uint)bytes[1]) << 16) + (((uint)bytes[0]) << 24);
        }
        /// <summary> 将Int64转换成IPAddress</summary>
        public static IPAddress ToIPAddress(this Int64 source)
        {
            Byte[] b = new Byte[4];
            for (int i = 0; i < 4; i++)
                b[3 - i] = (Byte)(source >> 8 * i & 255);
            return (new IPAddress(b));
        }
        /// <summary>将已经为 HTTP 传输进行过 HTML 编码的字符串转换为已解码的字符串</summary>
        public static string HtmlDecode(this string s)
        {
            return HttpUtility.HtmlDecode(s);
        }
        /// <summary>将字符串转换为 HTML 编码的字符串</summary>
        public static string HtmlEncode(this string s)
        {
            return HttpUtility.HtmlEncode(s);
        }
        /// <summary>对 URL 字符串进行编码</summary>
        public static string UrlEncode(this string s)
        {
            return HttpUtility.UrlEncode(s);
        }
        /// <summary>将已经为在 URL 中传输而编码的字符串转换为解码的字符串</summary>
        public static string UrlDecode(this string s)
        {
            return HttpUtility.UrlDecode(s);
        }
        /// <summary>转义</summary>
        public static string RegexDecode(this string s)
        {
            return Regex.Unescape(s);
        }
        /// <summary>通过将最少量的一组字符（\、*、+、?、|、{、[、(、)、^、$、.、# 和空白）替换为其转义码
        /// ，将这些字符转义。此操作指示正则表达式引擎以字面意义而非按元字符解释这些字符</summary>
        public static string RegexEncode(this string s)
        {
            return Regex.Escape(s);
        }
        public static T ToEnum<T>(this string instance, T defaultValue) where T : struct, IComparable, IFormattable
        {
            T convertedValue = defaultValue;
            if (!string.IsNullOrWhiteSpace(instance) && !Enum.TryParse(instance.Trim(), true, out convertedValue))
                convertedValue = defaultValue;
            return convertedValue;
        }           
        /// <summary> 截取指定字符串长度（自动区分中英文）</summary>
        /// <param name="stringToSub">待截取的字符串</param>
        /// <param name="length">需要截取的长度</param>
        /// <param name="endstring">如果截断则显示的字符（例如：。。。）</param>
        /// <returns></returns>
        public static string GetSubString(string stringToSub, int length, string endstring)
        {
            if (!string.IsNullOrEmpty(stringToSub))
            {
                Regex regex = new Regex("[\u4e00-\u9fa5]+", RegexOptions.Compiled);
                Regex regexq = new Regex("[^\x00-\xff]+", RegexOptions.Compiled);
                char[] stringChar = stringToSub.ToCharArray();
                StringBuilder sb = new StringBuilder();
                int nLength = 0;
                bool isCut = false;
                for (int i = 0; i < stringChar.Length; i++)
                {
                    if (regex.IsMatch((stringChar[i]).ToString()) || regexq.IsMatch((stringChar[i]).ToString()))
                    {
                        sb.Append(stringChar[i]);
                        nLength += 2;
                    }
                    else
                    {
                        sb.Append(stringChar[i]);
                        nLength = nLength + 1;
                    }

                    if (nLength > length)
                    {
                        isCut = true;
                        break;
                    }
                }
                if (isCut)
                    return sb.ToString() + endstring;
                else
                    return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary> 截取客户端指定长度的字符串并对输入的字符串验证</summary>
        /// <param name="text">客户端输入字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns>清理后的字符串</returns>
        public static string ClearInputText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            if (text.Length > maxLength)
                text = text.Substring(0, maxLength);
            text = Regex.Replace(text, "[\\s]{2,}", " ");	//移除两个以上的空格
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	//移除Br
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//移除&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);	//移除其他一些标志
            text = text.Replace("'", "''");//防止注入
            return text;
        }

        /// <summary> 客户端输入字符串验证</summary>
        /// <param name="text">客户端输入字符串</param>
        /// <returns>清理后的字符串</returns>
        public static string ClearInputText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            text = Regex.Replace(text, "[\\s]{2,}", " ");	//移除两个以上的空格
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	//移除Br
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//移除&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);	//移除其他一些标志
            text = text.Replace("'", "''");//防止注入
            return text;
        }

        /// <summary> 检测是否有Sql危险字符</summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary> 替换SQL危险字符</summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SafeSql(string s)
        {
            string str = string.Empty;
            if (s != null)
            {
                str = s.Replace("'", "''");
            }
            return str.Trim();
        }

        /// <summary> 从左边截取字符串,已进行空值判断</summary>
        /// <param name="s">待截取的字符串</param>
        /// <param name="len">要截取的长度</param>
        /// <returns></returns>
        public static string Left(string s, int len)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            else
            {
                if (s.Length <= len)
                    return s;
                else
                    return s.Substring(0, len);
            }
        }     

    }
}
