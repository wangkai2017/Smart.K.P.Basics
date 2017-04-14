using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace SmartCommon.JsonHelper
{
    public class JSONHelper
    {
        //根据URL获取JSON字符串
        public static string GetJsonByUrl(string url)
        {
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream responseStream = response.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                string responseText = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                responseStream.Close();
                return responseText;
            }
            catch
            {
                return string.Empty;
            }
        }

        //请求一个URL返回一个实体
        public static T LoadDataByUrl<T>(string url)
        {
            T temp = default(T);
            string jsonData = JSONHelper.GetJsonByUrl(url);
            if (!string.IsNullOrEmpty(jsonData))
            {
                temp = JSONHelper.Deserialize<T>(jsonData);
            }
            return temp;
        }

        //请求一个URL，并POST一些参数过去
        public static T LoadDataByUrl<T>(string url, object request)
        {
            T temp = default(T);
            string jsonData = GetStrJsonByUrl(url, request);
            //将响应转为实体
            if (!string.IsNullOrEmpty(jsonData))
            {
                temp = JSONHelper.Deserialize<T>(jsonData);
            }
            return temp;
        }

        /// <summary> 传入一个URL和一个对象，POST数据并获取返回对象</summary>
        /// <typeparam name="T">T(响应实体类型)</typeparam>
        /// <param name="url">POSTUrl</param>
        /// <param name="request">请求数据实体</param>
        /// <param name="resultData">委托变量</param>
        public static void LoadDataByUrl<T>(string url, string cmd, object request, Action<T> resultData)
        {
            string strXML = string.Empty;
            T temp = default(T);
            if (!string.IsNullOrEmpty(cmd))
            {
                strXML = GetStrXMLByUrl(url, request, cmd);
            }
            else
            {
                strXML = GetStrXMLByUrl(url, request, null);
            }


            //将响应转为实体
            if (!string.IsNullOrEmpty(strXML))
            {
                temp = Deserialize<T>(strXML);
            }
            resultData(temp);

        }

        /// <summary> 传入一个URL和一个对象，返回一个对象</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="resultData"></param>
        public static void LoadDataByUrl<T>(string url, object request, Action<T> resultData)
        {
            T temp = default(T);
            string jsonData = GetStrJsonByUrl(url, request);
            //将响应转为实体
            if (!string.IsNullOrEmpty(jsonData))
            {
                temp = JSONHelper.Deserialize<T>(jsonData);
            }
            resultData(temp);

        }

        public static void LoadDataByUrl<T>(string url, Action<T> resultData)
        {
            T temp = default(T);
            string jsonData = GetJsonByUrl(url);
            //将响应转为实体
            if (!string.IsNullOrEmpty(jsonData))
            {
                temp = JSONHelper.Deserialize<T>(jsonData);
            }
            resultData(temp);

        }


        //JSON格式字符转换为T类型的对象
        public static T Deserialize<T>(string json)
        {
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            T obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }

        /// <summary> 将一下对象序列化为一个JSON格式的字符串</summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static string Stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary> 请求一个URL，并POST一些参数过去</summary>
        /// <param name="url"></param>
        /// <param name="request">POST过去的数据</param>
        /// <returns></returns>
        public static string GetStrJsonByUrl(string url, object request)
        {
            string strPost = Stringify(request);


            //发送POST请求
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            byte[] arrB = encode.GetBytes(strPost);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.Method = "POST";
            //myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.ContentType = "application/json";
            myReq.ContentLength = arrB.Length;
            Stream outStream = myReq.GetRequestStream();
            outStream.Write(arrB, 0, arrB.Length);
            outStream.Close();

            HttpWebResponse myResp;
            try
            {
                //接收HTTP做出的响应
                myResp = (HttpWebResponse)myReq.GetResponse();
            }
            catch (WebException ex)
            {
                myResp = (HttpWebResponse)ex.Response;
            }
            Stream ReceiveStream = myResp.GetResponseStream();
            StreamReader readStream = new StreamReader(ReceiveStream, encode);
            Char[] read = new Char[256];
            int count = readStream.Read(read, 0, 256);
            string str = null;
            while (count > 0)
            {
                str += new String(read, 0, count);
                count = readStream.Read(read, 0, 256);
            }
            readStream.Close();
            myResp.Close();
            return str;

        }

        /// <summary> JSON序列化</summary>
        public static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            //替换Json的Date字符串
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        /// <summary> 将Json序列化的时间由/Date(1294499956278+0800)转为字符串</summary>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary> JSON反序列化</summary>
        public static T JsonDeserialize<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式
            string p = @"\d{4}-\d{1,2}-\d{1,2}\s\d{1,2}:\d{1,2}:\d{1,2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary> 转换对象为xml格式数据</summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="obj">T类型的对象</param>
        /// <returns>返回XML格式字符串</returns>
        public static string Serialize<T>(T obj)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }

        }

        /// <summary> 将时间字符串转为Json时间</summary>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
        #region [GetStrXMLByUrl]请求一个URL，并POST一些参数过去

        /// <summary> 请求一个URL，并POST一些参数过去</summary>
        /// <param name="url">京东接口地址</param>
        /// <param name="request">POST过去的数据</param>
        /// <param name="cmd">方法代码</param>
        /// <returns></returns>
        private static string GetStrXMLByUrl(string url, object request, string cmd)
        {
            string strPost = Serialize(request);
            strPost = strPost.Replace("<URL>", "").Replace("</URL>", "");
            strPost = strPost.Replace("&", "");
            strPost = strPost.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "")
                .Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\""
                , "");
            strPost = HttpUtility.UrlEncode(HttpUtility.UrlEncode(strPost, Encoding.UTF8), Encoding.UTF8);
            if (!string.IsNullOrEmpty(cmd))
            {
                strPost = "cmd=" + cmd + "&format=xml&msg=" + strPost;
            }

            //发送POST请求
            byte[] arrB = Encoding.UTF8.GetBytes(strPost);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.Method = "POST";
            myReq.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            myReq.ContentLength = arrB.Length;
            using (Stream outStream = myReq.GetRequestStream())
            {
                outStream.Write(arrB, 0, arrB.Length);
            }

            //接收HTTP做出的响应
            using (WebResponse myResp = myReq.GetResponse())
            {
                using (Stream ReceiveStream = myResp.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(ReceiveStream))
                    {
                        Char[] read = new Char[512];
                        int count = readStream.Read(read, 0, 512);
                        string str = null;
                        while (count > 0)
                        {
                            str += new String(read, 0, count);
                            count = readStream.Read(read, 0, 512);
                        }
                        return str;
                    }
                }
            }
        }

        #endregion
    }
}
