using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace SmartCommon.SerializerHelper
{
    public sealed class SerializerHelper
    {
        /// <summary> 将object对象序列化成XML</summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ObjectToXml(object o)
        {
            if (o == null) return string.Empty;
            XmlSerializer ser = new XmlSerializer(o.GetType());
            MemoryStream mem = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mem, Encoding.UTF8);
            ser.Serialize(writer, o);
            writer.Close();
            return Encoding.UTF8.GetString(mem.ToArray()).Trim();
        }
        /// <summary> 将XML反序列化成对象</summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object XmlToObject(string s, Type t)
        {
            XmlSerializer mySerializer = new XmlSerializer(t);
            StreamReader mem2 = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(s.Trim())), System.Text.Encoding.UTF8);

            return mySerializer.Deserialize(mem2);
        }



        /// <summary> 将XML反序列化成对象</summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T XmlToObject<T>(string source)
        {
            return XmlToObject<T>(source, Encoding.UTF8);
        }
        /// <summary> 将XML反序列化成对象,带编码</summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T XmlToObject<T>(string source, Encoding Coding)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            XmlTextReader reader = new XmlTextReader(new MemoryStream(Coding.GetBytes(source.Trim())));
            return (T)mySerializer.Deserialize(reader);
        }

        /// <summary> 二进制方式序列化对象</summary>
        /// <param name="testUser"></param>
        public static string Serialize<T>(T obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            StreamReader reader = new StreamReader(ms);

            return reader.ReadToEnd().Trim();
        }

        /// <summary> 二进制方式反序列化对象</summary>
        /// <returns></returns>
        public static T DeSerialize<T>(string str) where T : class
        {
            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(str.Trim()));
            BinaryFormatter formatter = new BinaryFormatter();
            T t = formatter.Deserialize(ms) as T;
            return t;
        }

        /// <summary> 将对象序列化为JSON</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ObjectToJson<T>(T t)
        {
            return new JavaScriptSerializer().Serialize(t);
        }

        /// <summary>
        /// 将"\/Date(673286400000+0800)\/"Json时间格式替换"yyyy-MM-dd HH:mm:ss"格式的字符串
        /// </summary>
        /// <param name="jsonDateTimeString">"\/Date(673286400000+0800)\/"Json时间格式</param>
        /// <returns></returns>
        public static string ConvertToDateTimeString(string jsonDateTimeString)
        {
            string result = string.Empty;
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            result = reg.Replace(jsonDateTimeString, matchEvaluator);
            return result;
        }

        private static string ConvertJsonDateToDateString(Match match)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>
        /// 将"yyyy-MM-dd HH:mm:ss"格式的字符串替换"\/Date(673286400000+0800)\/"Json时间格式
        /// </summary>
        /// <param name="dateTimeString">"yyyy-MM-dd HH:mm:ss"格式的字符串</param>
        /// <returns></returns>
        private static string ConvertToJsonDateTimeString(string dateTimeString)
        {
            string result = string.Empty;

            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            result = reg.Replace(dateTimeString, matchEvaluator);
            return result;
        }

        private static string ConvertDateStringToJsonDate(Match match)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(match.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
        public static T JSONToObject<T>(string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }
    }
}
