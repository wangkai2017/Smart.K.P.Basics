using SmartCommon.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SmartCommon.LogHelper
{
    public class LogService
    {
        private static string controllerName = string.Empty;
        private static string actionName = string.Empty;
        static LogService()
        {
            var routingData = HttpContext.Current.Request.RequestContext.RouteData;
            controllerName = routingData.Values["controller"].ToString();
            actionName = routingData.Values["action"].ToString();
        }

        /// <summary>记录异常日志信息</summary>
        /// <param name="exception">异常</param>
        /// <param name="extContent">拓展内容信息</param>        
        /// <param name="requestParams">查询请求参数</param>
        /// <param name="responseStr">返回值</param>
        /// <param name="noticyType">消息通知的方式None = 0,Mail = 1</param>
        public static void WriteErrorLog(Exception ex, string extContent = "", string requestParams = "", string responseStr = "", NoticyEnum noticyType = NoticyEnum.None)
        {
            WriteLog(ex.Message + "\n" + ex.StackTrace, extContent: extContent, requestParams: requestParams, responseStr: responseStr, noticyType: noticyType, level: LevelEnum.Error);
        }
        /// <summary>记录日志信息</summary>
        /// <param name="logContent">日志内容</param>
        /// <param name="requestParams">查询请求参数</param>        
        /// <param name="extContent">拓展内容信息</param>        
        /// <param name="requestParams">查询请求参数</param>
        /// <param name="responseStr">返回值</param>
        /// <param name="noticyType">消息通知的方式None = 0,Mail = 1</param>
        /// <param name="level">消息级别，默认</param>
        public static void WriteLog(string logContent, string extContent = "", string requestParams = "", string responseStr = "", NoticyEnum noticyType = NoticyEnum.None, LevelEnum level = LevelEnum.Info)
        {
            
            var stackTrace = new StackTrace(true);
            var method = stackTrace.GetFrame(1).GetMethod();
            //方法名
            var serviceName = string.Empty;
            //命名空间
            var fullName = string.Empty;
            if (null != method)
            {
                serviceName = method.Name;
                fullName = method.DeclaringType.FullName;
            }
            
            Task.Factory.StartNew(() =>
            {

            });
        }
    }
}
