using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SmartCommon.HttpHelper
{
    public class HttpHelper
    {
        #region  POST请求，参数以XML形式传递
        /// <summary>
        /// 发起HTTP请求
        /// </summary>
        /// <param name="m_Doc"></param>
        /// <param name="m_QuestURL"></param>
        /// <returns></returns>
        public static string GetRequest(XmlDocument m_Doc, string requestUrl)
        {
            string error = string.Empty;
            return PostRequest(requestUrl, m_Doc.OuterXml, "UTF-8", 100000, out error);
        }
        #endregion

        #region  POST请求，参数以XML形式传递
        /// <summary>
        /// 发起HTTP请求
        /// </summary>
        /// <param name="m_Doc"></param>
        /// <param name="m_QuestURL"></param>
        /// <returns></returns>
        public static string PostRequest(string requestXml, string requestUrl, out string error)
        {
            return PostRequest(requestUrl, requestXml, "UTF-8", 100000, out error);
        }

        /// <summary>
        /// 发起HTTP请求,可传编码方式
        /// </summary>
        /// <param name="m_Doc"></param>
        /// <param name="m_QuestURL"></param>
        /// <returns></returns>
        public static string PostRequest(string requestXml, string requestUrl, string CodingType, out string error)
        {
            return PostRequest(requestUrl, requestXml, CodingType, 100000, out error);
        }

        /// <summary>
        /// 发起HTTP请求
        /// </summary>
        /// <param name="m_Doc"></param>
        /// <param name="m_QuestURL"></param>
        /// <returns></returns>
        public static string PostRequest(string requestXml, string requestUrl)
        {
            string error = string.Empty;
            return PostRequest(requestUrl, requestXml, "UTF-8", 100000, out error);
        }

        public static string PostRequest(string requestXml, string requestUrl, string encoding)
        {
            string error = string.Empty;
            return PostRequest(requestUrl, requestXml, encoding, 100000, out error);
        }

        /// <summary>
        /// POST请求，参数以XML形式传递
        /// </summary>
        /// <param name="requestXml">XML请求内容</param>
        /// <param name="m_QuestURL">请求地址</param> 
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public static string PostRequest(string requestXml, string requestUrl, int timeout)
        {
            string error = string.Empty;
            string result = PostRequest(requestUrl, requestXml, "UTF-8", timeout, out error);
            if (!string.IsNullOrWhiteSpace(error))
            {
                result = "error";
            }

            return result;
        }
        
        #endregion

        #region  GET请求
        /// <summary>
        /// 发送 HTTP GET 请求 (编码：UTF-8，超时时间100秒)
        /// </summary>
        /// <param name="m_QuestURL">请求地址</param>
        /// <returns></returns>
        public static string GetRequest(string requestUrl)
        {
            return GetRequest(requestUrl, Encoding.UTF8, 1000000);
        }

        /// <summary>
        /// 发送 HTTP GET 请求 (超时时间100秒)
        /// </summary>
        /// <param name="m_QuestURL">请求地址</param>
        /// <param name="encode">编码，例如：UTF-8</param>
        /// <returns></returns>
        public static string GetRequest(string requestUrl, Encoding encode)
        {
            return GetRequest(requestUrl, encode, 1000000);
        }

        /// <summary>
        /// 发送 HTTP GET 请求
        /// </summary>
        /// <param name="m_QuestURL">请求地址</param>
        /// <param name="encode">编码，例如：UTF-8</param>
        /// <param name="timeout">请求超时时间，单位：MS</param>
        /// <returns></returns>
        public static string GetRequest(string requestUrl, Encoding encode, int timeout)
        {
            string result = string.Empty;
            HttpWebRequest request = null;
            StreamReader avReader = null;
            long elapsed = 0;
            Stopwatch stopWatch = Stopwatch.StartNew();
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
                request.Timeout = timeout;
                request.ServicePoint.Expect100Continue = false;
                avReader = new StreamReader(request.GetResponse().GetResponseStream(), encode);
                result = avReader.ReadToEnd();
                stopWatch.Stop();
                elapsed = stopWatch.ElapsedMilliseconds;
            }
            catch (WebException err)
            {
                switch (err.Status)
                {
                    case WebExceptionStatus.Timeout:
                        result = "请求超时";
                        break;
                    default:
                        result = err.Message;
                        break;
                }
                stopWatch.Stop();
                elapsed = stopWatch.ElapsedMilliseconds;
            }
            catch (Exception err)
            {
                result = err.Message;
                stopWatch.Stop();
                elapsed = stopWatch.ElapsedMilliseconds;
            }
            finally
            {
                if (avReader != null)
                {
                    request.Abort();
                    avReader.Close();
                }
            }

            return Toolkit.ClearSpecialCharForReq(result);
        }
        #endregion

        /// <summary>
        /// 发送HTTP请求（METHOD：POST）
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="requestData">请求参数</param>
        /// <param name="codingType">编码方式</param>
        /// <param name="timeout">请求超时时间，单位毫秒</param>
        /// <param name="error">当请求出现异常时，记录异常消息</param>
        /// <returns>以字符串格式返回响应结果</returns>
        public static string PostRequest(string requestUrl, string requestData, string codingType, int timeout, out string error)
        {
            string result = "";
            error = "";
            long elapsed = 0;
            string responseData = string.Empty;
            //Post请求地址
            Stopwatch stopWatch = Stopwatch.StartNew();
            try
            {
                HttpWebRequest m_Request = (HttpWebRequest)WebRequest.Create(requestUrl);
                //相应请求的参数
                byte[] data = Encoding.GetEncoding(codingType).GetBytes(requestData);
                m_Request.Method = "Post";
                m_Request.ContentType = "application/x-www-form-urlencoded";
                m_Request.ContentLength = data.Length;
                m_Request.Timeout = timeout;
                m_Request.ServicePoint.Expect100Continue = false;
                //请求流
                Stream requestStream = m_Request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                //响应流
                HttpWebResponse m_Response = (HttpWebResponse)m_Request.GetResponse();
                Stream responseStream = m_Response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding(codingType));
                //获取返回的信息
                result = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                stopWatch.Stop();
                elapsed = stopWatch.ElapsedMilliseconds;
            }
            catch (WebException ex)
            {
                result = "";
                stopWatch.Stop();
                elapsed = stopWatch.ElapsedMilliseconds;
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    error = string.Format("请求超时[{0}],请求地址：{1}", elapsed, requestUrl);
                }
                else
                {
                    error = string.Format("{0},请求地址：{1}", ex.Message, requestUrl);
                }
                responseData = ex.ToString();
            }
            catch (Exception ex)
            {
                result = "";
                error = string.Format("请求接口异常,请求地址：{0}", requestUrl);
                stopWatch.Stop();
                elapsed = stopWatch.ElapsedMilliseconds;
                responseData = ex.ToString();
            }
            if (string.IsNullOrWhiteSpace(responseData))
            {
                responseData = result;
            }
            return Toolkit.ClearSpecialCharForReq(result);
        }

    }
}
